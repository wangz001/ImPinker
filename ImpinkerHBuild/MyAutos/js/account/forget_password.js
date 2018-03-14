mui.init();
var dataParam = {
	phonenum: '',
	opreatetype: 3,
	checknum: '',
	password: "",
	password2: ''
}
mui.plusReady(function() {
	var sendCheckButton = document.getElementById('btn_sendchecknum');
	var timecount = 50;
	sendCheckButton.addEventListener('tap', function(event) {
		console.log(timecount);
		if(timecount == 50) {
			var phonenum = $("#accountBox").val();
			console.log(phonenum);
			if(!app.checkPhone(phonenum)) {
				mui.toast("请填写正确的手机号");
				return;
			}
			app.sendCheckNum(phonenum, dataParam.opreatetype, function(data) {
				console.log(JSON.stringify(data));
				if(data.IsSuccess == 1) {
					dataParam.phonenum = phonenum;
					plus.nativeUI.toast('验证码发送成功');
					updateSenBtn();
					return;
				}
				plus.nativeUI.toast(data.Description);
				return;
			});
		} else {
			mui.toast("请稍后再试");
		}

		function updateSenBtn() {
			$("#btn_sendchecknum").html(timecount + "秒后重新发送");
			console.log(timecount);
			if(timecount > 1) {
				//稍后重新发送
				setTimeout(function() {
					timecount--;
					updateSenBtn();
				}, 1000);
			} else {
				setTimeout(function() {
					$("#btn_sendchecknum").html("发送验证码");
					timecount = 50;
				}, 1200);
			}
		}

	});

	//下一步
	document.getElementById("btn-next").addEventListener('tap', function(event) {
		console.log('bbb')
		var checkNum = $("#checknum").val();
		if(checkNum != null && checkNum.length > 0) {
			//验证
			var checkPhoneUrl = commonConfig.apiRoot + '/api/account/FindPassword';
			var paras = {
				phonenum: dataParam.phonenum,
				checknum: checkNum
			}
			//验证手机号跟验证码是否匹配
			commonUtil.sendRequestPost(checkPhoneUrl, paras, function(data) {
				console.log(JSON.stringify(data));
				console.log(data.Data != null && data.Data.step == 1);
				if(data.IsSuccess == 0 && data.Data != null && data.Data.step == 1) {
					dataParam.checknum = checkNum;
					showNewPass();
				} else {
					mui.alert('验证码输入错误');
					$("#checknum").val('');
				}
			});
		} else {
			mui.alert("请输入正确的手机号和验证码")
		}
	});
	//显示输入新密码页面，提交更新密码
	function showNewPass() {
		console.log('aaa');
		$("#div-checkAccount").hide();
		$("#div-newpassword").show();
		// sendNewPassword
		document.getElementById("sendNewPassword").addEventListener('tap', function(event) {
			var pass1 = $("#password1").val();
			var pass2 = $("#password2").val();
			if(pass1 != null && pass1 == pass2) {
				dataParam.password = pass1;
				dataParam.password2 = pass2;
				console.log(JSON.stringify(dataParam));
				var findpassUrl = commonConfig.apiRoot + '/api/account/FindPassword';
				commonUtil.sendRequestPost(findpassUrl, dataParam, function(data) {
					//修改成功，跳转到登录页
					console.log(JSON.stringify(data));
					commonUtil.redirectTologin();
					plus.webview.currentWebview().close();
				});
			} else {
				mui.alert('两次输入的密码不一样~');
			}
		});
	}
});