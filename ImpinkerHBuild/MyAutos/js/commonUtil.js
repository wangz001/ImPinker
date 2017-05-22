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
			success: function(data) {
				return callback(data);
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
				return callback();
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
			}
			return callback();
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

}(mui, window.commonUtil = {}));

//html模板替换方法
String.prototype.temp = function (obj) {
    return this.replace(/\$\w+\$/gi, function (matchs) {
        var returns = obj[matchs.replace(/\$/g, "")];
        return (returns + "") === "undefined" ? "" : returns;
    });
};