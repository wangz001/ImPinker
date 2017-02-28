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
		question: document.getElementById('question'),
		contact: document.getElementById('contact'),
		imageList: document.getElementById('image-list'),
		submitBtn: document.getElementById('submit'),
		locationBtn: document.getElementById('geoLocation')
	};
	var url = 'https://service.dcloud.net.cn/weibo';
	weibo.files = [];
	weibo.uploader = null;
	weibo.deviceInfo = null;
	mui.plusReady(function() {
		//设备信息，无需修改
		weibo.deviceInfo = {
				appid: plus.runtime.appid,
				imei: plus.device.imei, //设备标识
				images: weibo.files, //图片文件
				p: mui.os.android ? 'a' : 'i', //平台类型，i表示iOS平台，a表示Android平台。
				md: plus.device.model, //设备型号
				app_version: plus.runtime.version,
				plus_version: plus.runtime.innerVersion, //基座版本号
				os: mui.os.version,
				net: '' + plus.networkinfo.getCurrentType()
			}
			//获取参数
		var self = plus.webview.currentWebview();
		var pathArr = self.path;
		//weibo.addFile(pathArr);
		console.log(pathArr);
		weibo.initImages(pathArr);
		weibo.newPlaceholder();
	});
	/**
	 *提交成功之后，恢复表单项 
	 */
	weibo.clearForm = function() {
		weibo.question.value = '';
		weibo.contact.value = '';
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
					quality: 50
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
		console.log("placeholder-------:");
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
			var name = path.substr(path.lastIndexOf('/') + 1);
			console.log("name:" + name);
			weibo.addFile(path);
			placehold.style.backgroundImage = 'url(' + path + ')';
			console.log("backgroundImage-------:" + path);

			placehold.appendChild(closeButton);
			weibo.imageList.appendChild(placehold);
		}
		weibo.newPlaceholder();
	}

	//获取地理位置
	weibo.locationBtn.addEventListener('tap', function(event) {
		plus.geolocation.getCurrentPosition(function(p) {
			alert("Geolocation\nLatitude:" + p.coords.latitude + "\nLongitude:" +
				p.coords.longitude + "\nAltitude:" + p.addresses);
				document.getElementById('locationtxt').appendChild(p.addresses);
		}, function(e) {
			alert("Geolocation error: " + e.message);
		});
		
	});

	//提交
	weibo.submitBtn.addEventListener('tap', function(event) {
		if(weibo.question.value == '' ||
			(weibo.contact.value != '' &&
				weibo.contact.value.search(/^(\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+)|([1-9]\d{4,9})$/) != 0)) {
			return mui.toast('信息填写不符合规范');
		}
		if(weibo.question.value.length > 200 || weibo.contact.value.length > 200) {
			return mui.toast('信息超长,请重新填写~')
		}
		//判断网络连接
		if(plus.networkinfo.getCurrentType() == plus.networkinfo.CONNECTION_NONE) {
			return mui.toast("连接网络失败，请稍后再试");
		}
		weibo.send(mui.extend({}, weibo.deviceInfo, {
			content: weibo.question.value,
			contact: weibo.contact.value,
			images: weibo.files,
			score: '' + starIndex
		}))
	}, false)
	weibo.send = function(content) {
		weibo.uploader = plus.uploader.createUpload(url, {
			method: 'POST'
		}, function(upload, status) {
			//			plus.nativeUI.closeWaiting()
			console.log("upload cb:" + upload.responseText);
			if(status == 200) {
				var data = JSON.parse(upload.responseText);
				//上传成功，重置表单
				if(data.ret === 0 && data.desc === 'Success') {
					//					mui.toast('反馈成功~')
					console.log("upload success");
					//					weibo.clearForm();
				}
			} else {
				console.log("upload fail");
			}

		});
		//添加上传数据
		mui.each(content, function(index, element) {
			if(index !== 'images') {
				console.log("addData:" + index + "," + element);
				//				console.log(index);
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
		mui.alert("感谢反馈，点击确定关闭", "问题反馈", "确定", function() {
			weibo.clearForm();
			mui.back();
		});
		//		plus.nativeUI.showWaiting();
	};

})();