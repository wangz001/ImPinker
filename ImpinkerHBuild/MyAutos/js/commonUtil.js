/**
 * 公用方法
 **/
(function(apputil, owner) {
	//发请求，不用身份验证
	owner.sendRequestGet = function(url, data, callback) {
		callback = callback || $.noop;
		mui.ajax(url, {
			data: data,
			dataType: 'json', //服务器返回json格式数据
			type: 'get', //HTTP请求类型
			success: function(data) {
				return callback(data);
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
				return callback();
			}
		});
	}

	//发起请求。需要身份验证
	owner.sendRequestWithToken = function(url, data, isPost, callback) {
		var typeStr = isPost ? 'post' : 'get'
		callback = callback || $.noop;
		var userstate = app.getState();
		mui.ajax(url, {
			data: data,
			headers: {
				"username": userstate.account,
				"usertoken": userstate.token
			},
			dataType: 'json', //服务器返回json格式数据
			type: typeStr, //HTTP请求类型
			success: function(data, status) {
				console.log(status == 200);
				console.log(JSON.stringify(data));
				return callback(data);
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
				if("Unauthorized" == errorThrown) {
					//无权限，重新登录
					mui.toast("身份验证失败，请重新登陆！");
					var currentview = plus.webview.currentWebview().id;
					mui.openWindow({
						//手势登录
						url: "/login.html",
						id: 'login',
						show: {
							aniShow: 'pop-in'
						},
						extras: {
							lastviewid: currentview
						},
						waiting: {
							autoShow: false
						}
					});
				}
				//return callback();
			}
		});
	}

	///上传图片公用方法
	owner.uploadImageWithFomedata = function(url, imgFiles, params, callback) {
		callback = callback || $.noop;
		var userstate = app.getState();
		var uploader = plus.uploader.createUpload(url, {
			method: 'POST'
		}, function(upload, status) {
			plus.nativeUI.closeWaiting();
			if(status == 200 || status == "200") {
				var data = JSON.parse(upload.responseText);
				if(data.IsSuccess === 1) {
					return callback(data);
				}
			} else {
				mui.toast("您未登陆，请重新登陆！");
				var currentview = plus.webview.currentWebview().id;
				mui.openWindow({
					//手势登录
					url: "/login.html",
					id: 'login',
					show: {
						aniShow: 'pop-in'
					},
					extras: {
						lastviewid: currentview
					},
					waiting: {
						autoShow: false
					}
				});
			}
		});
		//设置头信息
		uploader.setRequestHeader("username", userstate.account);
		uploader.setRequestHeader("usertoken", userstate.token);
		//添加上传数据
		mui.each(params, function(index, element) {
			if(index !== 'images') {
				console.log("addData:" + index + "," + element);
				uploader.addData(index, element.toString());
			}
		});
		//添加上传文件
		mui.each(imgFiles, function(index, element) {
			var f = imgFiles[index];
			console.log("addFile:" + JSON.stringify(f));
			uploader.addFile(f.path, {
				key: f.name
			});
		});
		//开始上传任务
		uploader.start();
		plus.nativeUI.showWaiting('正在上传图片');
	}
	//拍照
	owner.captureImage = function(callback) {
		var cmr = plus.camera.getCamera();
		var res = cmr.supportedImageResolutions[0];
		var fmt = cmr.supportedImageFormats[0];
		cmr.captureImage(function(path) {
				plus.io.resolveLocalFileSystemURL(path, function(entry) {
					console.log("真实路径：" + entry.fullPath);
					return callback(entry.fullPath);
				}, function(e) {
					mui.toast(e.message);
				});
			},
			function(error) {
				alert("Capture image failed: " + error.message);
			}, {
				resolution: res,
				format: fmt
			}
		);
	}

	//摄像
	owner.videoCapture = function(time, callback) {
		var cmr = plus.camera.getCamera();
		var res = cmr.supportedVideoResolutions[0];
		var fmt = cmr.supportedVideoFormats[0];
		console.log("Resolution: " + res + ", Format: " + fmt);
		cmr.startVideoCapture(function(path) {
				return callback(path);
			},
			function(error) {
				console.log("Capture video failed: " + error.message);
			}, {
				resolution: res,
				format: fmt
			}
		);
		// 拍摄10s后自动完成 。android 暂不支持
		setTimeout(stopCapture, time);
		// 停止摄像
		function stopCapture() {
			console.log("stopCapture");
			cmr.stopVideoCapture();
		}
	}

	//压缩图片
	owner.compressImage = function(imgpath, callback) {
		callback = callback || $.noop;
		var name = imgpath.substr(imgpath.lastIndexOf('/') + 1);
		plus.zip.compressImage({
			src: imgpath,
			dst: '_doc/' + name,
			overwrite: true,
			width: "1200px",
			quality: 90
		}, function(zip) {
			return callback(zip);
		}, function(zipe) {
			mui.toast('压缩失败！' + JSON.stringify(zipe))
		});
	}

}(mui, window.commonUtil = {}));

//html模板替换方法
String.prototype.temp = function(obj) {
	return this.replace(/\$\w+\$/gi, function(matchs) {
		var returns = obj[matchs.replace(/\$/g, "")];
		return(returns + "") === "undefined" ? "" : returns;
	});
};