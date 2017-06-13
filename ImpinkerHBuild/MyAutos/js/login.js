(function($, doc) {
	$.init({
		statusBarBackground: '#f7f7f7'
	});
	$.plusReady(function() {
		plus.screen.lockOrientation("portrait-primary");
		var settings = app.getSettings(); //配置
		var state = app.getState();//用户信息
		var mainPage = $.preload({
			"id": 'tab-webview-main.html',
			"url": 'tab-webview-main.html'
		});
		var main_loaded_flag = false;
		mainPage.addEventListener("loaded", function() {
			main_loaded_flag = true;
		});
		var toMain = function() {
			//使用定时器的原因：
			//可能执行太快，main页面loaded事件尚未触发就执行自定义事件，此时必然会失败
<<<<<<< HEAD
			//			var id = setInterval(function() {
			//				if(true) {
			//					clearInterval(id);
			//					$.fire(mainPage, 'show', null);
			//					mainPage.show("pop-in");
			//				}
			//			}, 20);
			console.log("to main");
			mui.back();
		};

		if(state.token && 1 == 8) {
=======
			var id = setInterval(function() {
				if(true) {
					clearInterval(id);
					$.fire(mainPage, 'show', null);
					mainPage.show("pop-in");
				}
			}, 20);
			
		};
		//检查 "登录状态/锁屏状态" 开始
//		if(state.token && settings.gestures) {
//			$.openWindow({
//				//手势登录
//				url: 'unlock.html',
//				id: 'unlock',
//				show: {
//					aniShow: 'pop-in'
//				},
//				waiting: {
//					autoShow: false
//				}
//			});
//		} else 
		if(state.token) {
>>>>>>> parent of dba55dc... aa
			//每次打开应用。重新登录，获取token
//			app.login(state, function(err) {
//				if(err) {
//					plus.nativeUI.toast(err);
//					return;
//				}
//				console.log("自动登录。。。tomain");
//				toMain();
//			});
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
		var regButton = doc.getElementById('reg');
		var forgetButton = doc.getElementById('forgetPassword');
		loginButton.addEventListener('tap', function(event) {
			console.log("开始登录。。。");
			var loginInfo = {
				account: accountBox.value,
				password: passwordBox.value
			};
			
			console.log(loginInfo.account+"------"+loginInfo.password)
			app.login(loginInfo, function(err) {
				if(err) {
					plus.nativeUI.toast(err);
					return;
				}
				console.log("开始登录。。。tomain");
				toMain();
			});
		});
		$.enterfocus('#login-form input', function() {
			$.trigger(loginButton, 'tap');
		});
		autoLoginButton.classList[settings.autoLogin ? 'add' : 'remove']('mui-active')
		autoLoginButton.addEventListener('toggle', function(event) {
			setTimeout(function() {
				var isActive = event.detail.isActive;
				settings.autoLogin = isActive;
				app.setSettings(settings);
			}, 50);
		}, false);
		regButton.addEventListener('tap', function(event) {
			$.openWindow({
				url: 'reg.html',
				id: 'reg',
				preload: true,
				show: {
					aniShow: 'pop-in'
				},
				styles: {
					popGesture: 'hide'
				},
				waiting: {
					autoShow: false
				}
			});
		}, false);
		forgetButton.addEventListener('tap', function(event) {
			$.openWindow({
				url: 'forget_password.html',
				id: 'forget_password',
				preload: true,
				show: {
					aniShow: 'pop-in'
				},
				styles: {
					popGesture: 'hide'
				},
				waiting: {
					autoShow: false
				}
			});
		}, false);
	});
}(mui, document));