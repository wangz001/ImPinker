/*!
 * ======================================================
 * weibo Template For MUI (http://dev.dcloud.net.cn/mui)
 * =======================================================
 * @version:1.0.0
 * @author:cuihongbao@dcloud.io
 */
(function() {
	var index = 1;
	var size = null;
	var imageIndexIdNum = 0;
	var starIndex = 0;
	var weibo = {
		description: document.getElementById('description'),
		locationtxt: document.getElementById('locationtxt'),
		imageList: document.getElementById('image-list'),
		submitBtn: document.getElementById('submit'),
		locationBtn: document.getElementById('mySwitch')
	};
	var url = 'http://api.myautos.cn/api/weibo/newweibo';
	weibo.files = [];
	weibo.uploader = null;
	weibo.deviceInfo = null;
	weibo.gisinfo = null;
	mui.plusReady(function() {
		//设备信息，无需修改
		weibo.deviceInfo = {
			appid: plus.runtime.appid,
			imei: plus.device.imei, //设备标识
			images: weibo.files, //图片文件
			p: mui.os.android ? 'a' : 'i', //平台类型，i表示iOS平台，a表示Android平台。
			HardWareType: plus.device.model, //设备型号
			app_version: plus.runtime.version,
			plus_version: plus.runtime.innerVersion, //基座版本号
			os: mui.os.version,
			net: '' + plus.networkinfo.getCurrentType()
		}
		//地理位置信息
		weibo.gisinfo = {
			LocationText: "",
			Longitude: "",
			Latitude: "",
			Height: ""
		}
		//获取参数
		var self = plus.webview.currentWebview();
		var pathArr = self.path;
		if(pathArr != undefined && pathArr != null) {
			console.log(pathArr);
			weibo.initImages(pathArr);
		}

		weibo.newPlaceholder();
	});
	/**
	 *提交成功之后，恢复表单项 
	 */
	weibo.clearForm = function() {
		weibo.description.value = '';
		weibo.locationtxt.value = '';
		weibo.imageList.innerHTML = '';
		weibo.newPlaceholder();
		weibo.files = [];
		index = 0;
		size = 0;
		imageIndexIdNum = 0;
		starIndex = 0;
		//清除所有星标
		mui('.icons i').each(function(index, element) {
			if(element.classList.contains('mui-icon-star-filled')) {
				element.classList.add('mui-icon-star')
				element.classList.remove('mui-icon-star-filled')
			}
		})
	};
	weibo.getFileInputArray = function() {
		return [].slice.call(weibo.imageList.querySelectorAll('.file'));
	};
	weibo.addFile = function(path) {
		weibo.files.push({
			name: "images" + index,
			path: path
		});
		index++;
	};
	/**
	 * 初始化图片域占位
	 */
	weibo.newPlaceholder = function(pathArr) {
		var fileInputArray = weibo.getFileInputArray();
		if(fileInputArray &&
			fileInputArray.length > 0 &&
			fileInputArray[fileInputArray.length - 1].parentNode.classList.contains('space')) {
			return;
		};
		imageIndexIdNum++;
		var placeholder = document.createElement('div');
		placeholder.setAttribute('class', 'image-item space');
		var up = document.createElement("div");
		up.setAttribute('class', 'image-up')
		//删除图片
		var closeButton = document.createElement('div');
		closeButton.setAttribute('class', 'image-close');
		closeButton.innerHTML = 'X';
		//小X的点击事件
		closeButton.addEventListener('tap', function(event) {
			setTimeout(function() {
				console.log(placeholder.attribute('id'));
				weibo.imageList.removeChild(placeholder);
			}, 0);
			return false;
		}, false);

		//
		var fileInput = document.createElement('div');
		fileInput.setAttribute('class', 'file');
		fileInput.setAttribute('id', 'image-' + imageIndexIdNum);
		fileInput.addEventListener('tap', function(event) {
			var self = this;
			var index = (this.id).substr(-1);

			plus.gallery.pick(function(e) {
				//				console.log("event:"+e);
				var name = e.substr(e.lastIndexOf('/') + 1);
				console.log("name:" + name);

				plus.zip.compressImage({
					src: e,
					dst: '_doc/' + name,
					overwrite: true,
					quality: 70
				}, function(zip) {
					size += zip.size
					console.log("filesize:" + zip.size + ",totalsize:" + size);
					if(size > (10 * 1024 * 1024)) {
						return mui.toast('文件超大,请重新选择~');
					}
					if(!self.parentNode.classList.contains('space')) { //已有加号图片
						weibo.files.splice(index - 1, 1, {
							name: "images" + index,
							path: e
						});
					} else { //加号
						placeholder.classList.remove('space');
						weibo.addFile(zip.target);
						weibo.newPlaceholder();
					}
					up.classList.remove('image-up');
					placeholder.style.backgroundImage = 'url(' + zip.target + ')';
				}, function(zipe) {
					mui.toast('压缩失败！')
				});
			}, function(e) {
				mui.toast(e.message);
			}, {});
		}, false);
		placeholder.appendChild(closeButton);
		placeholder.appendChild(up);
		placeholder.appendChild(fileInput);
		weibo.imageList.appendChild(placeholder);
	};

	//接受页面跳转传递过来的图片，并显示
	weibo.initImages = function(pathArr) {
		if(pathArr == null || pathArr.length == 0) {
			weibo.newPlaceholder();
			return;
		}
		for(var i = 0; i < pathArr.length; i++) {
			imageIndexIdNum = i + 1;
			var placehold = document.createElement('div');
			placehold.setAttribute('class', 'image-item');
			placehold.setAttribute('id', 'image-' + imageIndexIdNum);
			//删除图片
			var closeButton = document.createElement('div');
			closeButton.setAttribute('class', 'image-close');
			closeButton.innerHTML = 'X';
			//小X的点击事件
			closeButton.addEventListener('tap', function(event) {
				setTimeout(function() {
					weibo.imageList.removeChild(placehold);
					weibo.newPlaceholder();
				}, 0);
				return false;
			}, false);
			var path = pathArr[i];
			console.log("path1111:" + path);
			weibo.addFile(path);
			placehold.style.backgroundImage = 'url(' + path + ')';
			console.log("backgroundImage-------:" + path);
			placehold.appendChild(closeButton);
			weibo.imageList.appendChild(placehold);
		}
		weibo.newPlaceholder();
	}
	//获取地理位置
	weibo.locationBtn.addEventListener("toggle", function(event) {
		if(event.detail.isActive) {
			plus.geolocation.getCurrentPosition(function(p) {
				weibo.gisinfo.LocationText = p.addresses;
				weibo.gisinfo.Longitude = p.coords.longitude.toString();
				weibo.gisinfo.Latitude = p.coords.latitude.toString();
				weibo.gisinfo.Height = p.coords.altitude.toString();
				document.getElementById('locationtxt').innerHTML = p.addresses;
			}, function(e) {
				alert("Geolocation error: " + e.message);
			});
		} else {
			weibo.gisinfo.LocationText = "";
			weibo.gisinfo.Longitude = 0.0;
			weibo.gisinfo.Latitude = 0.0;
			weibo.gisinfo.Height = 0.0;
			document.getElementById('locationtxt').innerHTML = "";
		}
	});

	//提交
	var userstate = app.getState();
	weibo.submitBtn.addEventListener('tap', function(event) {
		if(weibo.description.value == '' || weibo.files.length == 0) {
			return mui.toast('信息填写不符合规范');
		}
		if(weibo.description.value.length > 200) {
			return mui.toast('信息超长,请重新填写~')
		}
		//判断网络连接
		if(plus.networkinfo.getCurrentType() == plus.networkinfo.CONNECTION_NONE) {
			return mui.toast("连接网络失败，请稍后再试");
		}
		//mui.extend(0)  合并参数
		var token = userstate.token;
		console.log("send......token:" + token);
		weibo.send(mui.extend({}, weibo.deviceInfo, weibo.gisinfo, {
			Description: weibo.description.value,
			images: weibo.files,
			score: '' + starIndex
		}))
	}, false)
	weibo.send = function(content) {
		weibo.uploader = plus.uploader.createUpload(url, {
			method: 'POST'
		}, function(upload, status) {
			plus.nativeUI.closeWaiting()
			if(status == 200 || status == "200") {
				var data = JSON.parse(upload.responseText);
				//上传成功，重置表单
				if(data.IsSuccess === 1) {
					plus.webview.getLaunchWebview().reload();
					setTimeout(function() {
						mui.back();
					}, 500);
				}
			} else {
				console.log("upload fail");
			}
		});
		//设置头信息
		weibo.uploader.setRequestHeader("username", userstate.account);
		weibo.uploader.setRequestHeader("usertoken", userstate.token);
		//添加上传数据
		mui.each(content, function(index, element) {
			if(index !== 'images') {
				console.log("addData:" + index + "," + element);
				weibo.uploader.addData(index, element)
			}
		});
		//添加上传文件
		mui.each(weibo.files, function(index, element) {
			var f = weibo.files[index];
			console.log("addFile:" + JSON.stringify(f));
			weibo.uploader.addFile(f.path, {
				key: f.name
			});
		});
		//开始上传任务
		weibo.uploader.start();
		plus.nativeUI.showWaiting('正在发布');
	};

})();