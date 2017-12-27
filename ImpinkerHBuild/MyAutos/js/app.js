/**
 * 演示程序当前的 “注册/登录” 等操作，是基于 “本地存储” 完成的
 * 当您要参考这个演示程序进行相关 app 的开发时，
 * 请注意将相关方法调整成 “基于服务端Service” 的实现。
 **/
(function($, owner) {
	/**
	 * 发送手机验证码
	 * @param {Object} phonenum
	 * @param {Object} callback
	 */
	owner.sendCheckNum = function(phonenum, callback) {
		if(!owner.checkPhone(phonenum)) {
			return callback('手机号码不正确');
		}
		callback = callback || $.noop;
		mui.ajax('http://api.myautos.cn/api/account/SendCheckNum', {
			data: {
				phonenum: phonenum,
				opreatetype: 1
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post',
			timeout: 10000, //超时时间设置为10秒；
			headers: {
				'Content-Type': 'application/json'
			},
			success: function(data) {
				return callback(data);
			},
			error: function(xhr, type, errorThrown) {
				return callback({
					IsSuccess: 0,
					Data: JSON.stringify(type) + "----" + JSON.stringify(errorThrown),
					Description: JSON.stringify(errorThrown)
				});
			}
		});
	};
	/**
	 * 用户登录
	 **/
	owner.login = function(loginInfo, callback) {
		callback = callback || $.noop;
		loginInfo = loginInfo || {};
		loginInfo.account = loginInfo.account || '';
		loginInfo.password = loginInfo.password || '';
		if(loginInfo.account.length < 5) {
			return callback({
				IsSuccess: 0,
				Data: "",
				Description: '账号最短为 5 个字符'
			});
		}
		if(loginInfo.password.length < 2) {
			return callback({
				IsSuccess: 0,
				Data: "",
				Description: '密码最短为 6 个字符'
			});
		}
		//登录验证
		mui.ajax('http://api.myautos.cn/api/Account/Login', {
			data: {
				username: loginInfo.account,
				password: loginInfo.password
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post', //HTTP请求类型
			timeout: 10000, //超时时间设置为10秒；
			headers: {
				'Content-Type': 'application/json'
			},
			success: function(data) {
				if(data.IsSuccess == 1) {
					var token = data.Data;
					return owner.createState(loginInfo, data, callback);
				} else{
					mui.toast("登录失败，请重新登录~");
				}
//				else {
//					return callback(data);
//				}
			},
			error: function(xhr, type, errorThrown) {
				//异常处理；
				console.log(type);
				return callback({
					IsSuccess: 0,
					Data: "",
					Description: type
				});
			}
		});
	};
	///第三方登录  
	owner.loginOAuth = function(loginInfo, callback) {
		callback = callback || $.noop;
		loginInfo = loginInfo || {};
		loginInfo.openid = loginInfo.openid || '';
		loginInfo.oauthtype = loginInfo.oauthtype || '';
		loginInfo.showname = loginInfo.showname || '';
		loginInfo.headimage = loginInfo.headimage || '';
		//
		loginInfo.account = loginInfo.account || '';
		loginInfo.password = loginInfo.password || '';
		if(loginInfo.account.openid < 5) {
			return callback('账号最短为 5 个字符');
		}
		if(loginInfo.password.oauthtype < 2) {
			return callback('类型不对');
		}
		//登录验证
		mui.ajax('http://api.myautos.cn/api/Account/LoginOAoth', {
			data: {
				OpenId: loginInfo.OpenId,
				OauthType: loginInfo.OauthType,
				ShowName: loginInfo.ShowName,
				HeadImage: loginInfo.HeadImage
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post', //HTTP请求类型
			timeout: 10000, //超时时间设置为10秒；
			headers: {
				'Content-Type': 'application/json'
			},
			success: function(data) {
				if(data.IsSuccess == 1) {
					var token = data.Data;
					console.log("返回的token。。。" + token.Token);
					return owner.createState(loginInfo, token, callback);
				} else {
					return callback(data.Description);
				}
			},
			error: function(xhr, type, errorThrown) {
				//异常处理；
				console.log(type);
			}
		});
	}

	//更新token
	owner.updateToken = function(callback) {
		var state = owner.getState(); //用户信息
		console.log("aaa:" + state.account + ";;" + state.password);
		var mydate = new Date();
		var nowTime = mydate.toLocaleString(); //获取当前时间
		console.log(nowTime);
		console.log(state.lastUpdateTime);
		//console.log(nowTime-state.lastUpdateTime);
		//		if(state.account && state.password&&state.lastUpdateTime&&true) {
		//			//判断最后一次更新时间，小于1小时，不更新
		//			
		//			//return
		//		}
		if(state.account && state.password) {
			//toMain();
			//每次打开应用。重新登录，获取token
			owner.login(state, function(data) {
				return callback(data);
			});
		} else {
			//alert("用户名或密码为空");
			return callback({
				IsSuccess: 0,
				Data: "",
				Description: "用户名和密码为空"
			});
		}
	}

	owner.createState = function(loginInfo, data, callback) {
		var userData=data.Data;
		var state = owner.getState();
		state.account = loginInfo.account;
		state.password = loginInfo.password;
		state.token = userData.Token;
		state.showname=userData.ShowName;
		state.imgurl=userData.ImgUrl;
		state.phonenum=userData.PhoneNum;
		state.email=userData.Email;
		state.sex=userData.Sex;
		state.lastUpdateTime = new Date().toLocaleString();
		owner.setState(state);
		return callback({
			IsSuccess: 1,
			Data: state.token,
			Description: "更新token成功"
		});
	};

	/**
	 * 新用户注册
	 **/
	owner.reg = function(regInfo, callback) {
		callback = callback || $.noop;
		regInfo = regInfo || {};
		regInfo.account = regInfo.account || '';
		regInfo.password = regInfo.password || '';
		if(!owner.checkPhone(regInfo.account)) {
			return callback('手机号不合法');
		}
		if(regInfo.password.length < 6) {
			return callback('密码最短需要 6 个字符');
		}
		if(regInfo.checknum.length != 6) {
			return callback('请输入正确的验证码');
		}
		mui.ajax('http://api.myautos.cn/api/account/Regist', {
			data: {
				Username: regInfo.account,
				PhoneNum: regInfo.account,
				Password: regInfo.password,
				Password2: regInfo.password,
				CheckNum: regInfo.checknum,
				opreatetype: 1
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post',
			timeout: 10000, //超时时间设置为10秒；
			headers: {
				'Content-Type': 'application/json'
			},
			success: function(data) {
				console.log(data.IsSuccess);
				if(data.IsSuccess == 1) {
					var users = JSON.parse(localStorage.getItem('$users') || '[]');
					users.push(regInfo);
					localStorage.setItem('$users', JSON.stringify(users));
					//设置登录名和密码
					owner.setState(regInfo);
					return callback();
				} else {
					console.log(data.Description);
					return callback(data.Description);
				}
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify({"aa":type,"bb":xhr,"cc":errorThrown}));
			}
		});
	};

	/**
	 * 获取当前状态
	 **/
	owner.getState = function() {
		var stateText = localStorage.getItem('$state') || "{}";
		//var stateText = plus.storage.getItem('$state') || "{}";
		return JSON.parse(stateText);
	};

	/**
	 * 设置当前状态
	 **/
	owner.setState = function(state) {
		state = state || {};
		localStorage.setItem('$state', JSON.stringify(state)); //h5本地存储
		//plus.storage.setItem('$state', JSON.stringify(state));  //app本地存储（速度慢，但可跨域）
		var settings = owner.getSettings();
		settings.gestures = '';
		owner.setSettings(settings);
	};

	var checkEmail = function(email) {
		email = email || '';
		return(email.length > 3 && email.indexOf('@') > -1);
	};

	owner.checkPhone = function(phonenum) {
		phonenum = phonenum || '';
		if(!(/^1[34578]\d{9}$/.test(phonenum))) {
			return false;
		}
		return true;
	}

	/**
	 * 找回密码
	 **/
	owner.forgetPassword = function(email, callback) {
		callback = callback || $.noop;
		if(!checkEmail(email)) {
			return callback('邮箱地址不合法');
		}
		return callback(null, '新的随机密码已经发送到您的邮箱，请查收邮件。');
	};

	/**
	 * 获取应用本地配置
	 **/
	owner.setSettings = function(settings) {
		settings = settings || {};
		localStorage.setItem('$settings', JSON.stringify(settings));
	}

	/**
	 * 设置应用本地配置
	 **/
	owner.getSettings = function() {
		var settingsText = localStorage.getItem('$settings') || "{}";
		return JSON.parse(settingsText);
	}
	/**
	 * 获取本地是否安装客户端
	 **/
	owner.isInstalled = function(id) {
		if(id === 'qihoo' && mui.os.plus) {
			return true;
		}
		if(mui.os.android) {
			var main = plus.android.runtimeMainActivity();
			var packageManager = main.getPackageManager();
			var PackageManager = plus.android.importClass(packageManager)
			var packageName = {
				"qq": "com.tencent.mobileqq",
				"weixin": "com.tencent.mm",
				"sinaweibo": "com.sina.weibo"
			}
			try {
				return packageManager.getPackageInfo(packageName[id], PackageManager.GET_ACTIVITIES);
			} catch(e) {}
		} else {
			switch(id) {
				case "qq":
					var TencentOAuth = plus.ios.import("TencentOAuth");
					return TencentOAuth.iphoneQQInstalled();
				case "weixin":
					var WXApi = plus.ios.import("WXApi");
					return WXApi.isWXAppInstalled()
				case "sinaweibo":
					var SinaAPI = plus.ios.import("WeiboSDK");
					return SinaAPI.isWeiboAppInstalled()
				default:
					break;
			}
		}
	}

}(mui, window.app = {}));