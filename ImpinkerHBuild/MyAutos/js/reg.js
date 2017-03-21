(function($, doc) {
	$.init();
	$.plusReady(function() {
		var settings = app.getSettings();
		var regButton = doc.getElementById('reg');
		var accountBox = doc.getElementById('account');
		var passwordBox = doc.getElementById('password');
		var passwordConfirmBox = doc.getElementById('password_confirm');
		var checknumBox = doc.getElementById('checknum');
		var sendCheckButton = doc.getElementById('btn_sendchecknum');
		regButton.addEventListener('tap', function(event) {
			var regInfo = {
				account: accountBox.value,
				password: passwordBox.value,
				checknum: checknumBox.value
			};
			var passwordConfirm = passwordConfirmBox.value;
			if(passwordConfirm != regInfo.password) {
				plus.nativeUI.toast('密码两次输入不一致');
				return;
			}
			if(regInfo.account == "" || regInfo.password == "" || regInfo.checknum == "") {
				plus.nativeUI.toast('信息填写不完整');
				return;
			}
			app.reg(regInfo, function(err) {
				if(err) {
					plus.nativeUI.toast(err);
					return;
				}
				plus.nativeUI.toast('注册成功');
				/*
				 * 注意：
				 * 1、因本示例应用启动页就是登录页面，因此注册成功后，直接显示登录页即可；
				 * 2、如果真实案例中，启动页不是登录页，则需修改，使用mui.openWindow打开真实的登录页面
				 */
				plus.webview.getLaunchWebview().show("pop-in", 200, function() {
					plus.webview.currentWebview().close("none");
				});
				//若启动页不是登录页，则需通过如下方式打开登录页
				$.openWindow({
					url: 'login.html',
					id: 'login',
					show: {
						aniShow: 'pop-in'
					}
				});
			});
		});
		sendCheckButton.addEventListener('tap', function(event) {
			var phonenum = accountBox.value;
			console.log(phonenum);
			app.sendCheckNum(phonenum, function(err) {
				if(err) {
					plus.nativeUI.toast(err);
					return;
				}
				plus.nativeUI.toast('验证码发送成功');
				return;
			});
		});
	});
}(mui, document));