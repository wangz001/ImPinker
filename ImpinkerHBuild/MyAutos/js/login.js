(function(mui, doc) {
	mui.init({
		statusBarBackground: '#f7f7f7'
	});
	mui.plusReady(function() {
		plus.screen.lockOrientation("portrait-primary");
		var settings = app.getSettings(); //配置
		var state = app.getState(); //用户信息
		//默认显示用户名和密码
		if(state.account) {
			var username = state.account;
			document.getElementById("account").value = username;
		}
		if(state.password) {
			var password = state.password;
			document.getElementById("password").value = password;
		}
		//		var mainPage = mui.preload({
		//			"id": 'tab-webview-main.html',
		//			"url": 'tab-webview-main.html'
		//		});
		//		var main_loaded_flag = false;
		//		mainPage.addEventListener("loaded", function() {
		//			main_loaded_flag = true;
		//		});
		//获取上一个页面的id
		var self = plus.webview.currentWebview();
		var lastViewid = self.lastviewid;
		console.log(lastViewid);
		var toMain = function() {
			//使用定时器的原因：
			//可能执行太快，main页面loaded事件尚未触发就执行自定义事件，此时必然会失败
			//			var id = setInterval(function() {
			//				if(true) {
			//					clearInterval(id);
			//					mui.fire(mainPage, 'show', null);
			//					mainPage.show("pop-in");
			//				}
			//			}, 20);
			var topView = plus.webview.getWebviewById(lastViewid);
			//console.log(topView.id);
			//topView.reload();
			//登录成功后，刷新下‘我的’页面
			var myView = plus.webview.getWebviewById("tab-webview-subpage-setting.html");
			myView.reload();
			setTimeout(function() {
				plus.webview.currentWebview().close();
			}, 200);
		};

		if(state.account && 1 > 2) {
			//每次打开应用。重新登录，获取token
			app.login(state, function(data) {
				if(data.IsSuccess == 0) {
					plus.nativeUI.toast(data);
					return;
				}
				toMain();
			});
		} else {
			app.setState(null);
			//第三方登录
			//var authBtns = ['weixin', 'sinaweibo', 'qq']; //配置业务支持的第三方登录
			var authBtns = []; //配置业务支持的第三方登录
			var auths = {};
			var oauthArea = doc.querySelector('.oauth-area');
			plus.oauth.getServices(function(services) {
				for(var i in services) {
					var service = services[i];
					auths[service.id] = service;
					if(~authBtns.indexOf(service.id)) {
						var isInstalled = app.isInstalled(service.id);
						var btn = document.createElement('div');
						//如果微信未安装，则为不启用状态
						btn.setAttribute('class', 'oauth-btn' + (!isInstalled && service.id === 'weixin' ? (' disabled') : ''));
						btn.authId = service.id;
						btn.style.backgroundImage = 'url("images/' + service.id + '.png")'
						oauthArea.appendChild(btn);
					}
				}
				mui(oauthArea).on('tap', '.oauth-btn', function() {
					if(this.classList.contains('disabled')) {
						plus.nativeUI.toast('您尚未安装微信客户端');
						return;
					}
					var auth = auths[this.authId];
					var waiting = plus.nativeUI.showWaiting();
					auth.login(function() {
						waiting.close();
						plus.nativeUI.toast("登录认证成功");
						console.log(JSON.stringify(auth));
						//
						var authType = auth.id;
						var openid = auth.authResult.openid;
						var nickname = auth.userInfo.nickname || auth.userInfo.name;
						var headimage = auth.userInfo.headimgurl || auth.userInfo.profile_image_url;
						var loginInfo = {
							account: openid,
							password: authType,
							OpenId: openid,
							OauthType: authType,
							ShowName: nickname,
							HeadImage: headimage
						};
						app.loginOAuth(loginInfo, function(date) {
							if(date) {
								console.log(date);
							}
							toMain();
						})
						//						auth.getUserInfo(function() {
						//							plus.nativeUI.toast("获取用户信息成功");
						//
						//							var name = auth.userInfo.nickname || auth.userInfo.name;
						//							app.createState2(name, function() {
						//								toMain();
						//							});
						//						}, function(e) {
						//							plus.nativeUI.toast("获取用户信息失败：" + e.message);
						//						});
					}, function(e) {
						waiting.close();
						plus.nativeUI.toast("登录认证失败：" + e.message);
					});
				});
			}, function(e) {
				oauthArea.style.display = 'none';
				plus.nativeUI.toast("获取登录认证失败：" + e.message);
			});
		}
		// close splash
		setTimeout(function() {
			//关闭 splash
			plus.navigator.closeSplashscreen();
		}, 600);
		//检查 "登录状态/锁屏状态" 结束
		var loginButton = doc.getElementById('login');
		var accountBox = doc.getElementById('account');
		var passwordBox = doc.getElementById('password');
		var autoLoginButton = doc.getElementById("autoLogin");
		//var regButton = doc.getElementById('reg');
		//var forgetButton = doc.getElementById('forgetPassword');
		loginButton.addEventListener('tap', function(event) {
			var loginInfo = {
				account: accountBox.value,
				password: passwordBox.value
			};

			console.log(loginInfo.account + "------" + loginInfo.password)
			app.login(loginInfo, function(data) {
				if(data.IsSuccess == 1) {
					console.log("开始登录。。。tomain");
					toMain();
				} else {
					plus.nativeUI.toast(data.Description);
					return;
				}
			});
		});
		mui.enterfocus('#login-form input', function() {
			mui.trigger(loginButton, 'tap');
		});
//		autoLoginButton.classList[settings.autoLogin ? 'add' : 'remove']('mui-active')
//		autoLoginButton.addEventListener('toggle', function(event) {
//			setTimeout(function() {
//				var isActive = event.detail.isActive;
//				settings.autoLogin = isActive;
//				app.setSettings(settings);
//			}, 50);
//		}, false);
	});
}(mui, document));