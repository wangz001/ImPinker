mui.init({
	swipeBack: true //启用右滑关闭功能
});
//			document.getElementById('youjidraft').addEventListener('tap', function() {
//				mui.openWindow({
//					url: "../article/youji_mydraft.html",
//					id: "youji_mydraft",
//					show: {
//						aniShow: 'slide-in-right',
//						duration: 200
//					}
//				});
//			});
/*!
 发微博
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
	var url = commonConfig.apiRoot+'/api/weibo/newweibo';
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
		var pathArr = self.path; //只能接受字符串类型的参数。不能接受数组
		if(pathArr != undefined && pathArr != null) {
			console.log(pathArr);
			var arr = pathArr.split(",");
			weibo.initImages(arr);
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
	//从数组中移除
	weibo.removeFile = function(indexposition) {
		weibo.files.splice(indexposition, 1);
		index--;
	};
	/**
	 * 初始化图片域占位
	 */
	weibo.newPlaceholder = function() {
		if(weibo.files.length > 8) { //最多选9张照片
			return;
		}
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
		//
		var fileInput = document.createElement('div');
		fileInput.setAttribute('class', 'file');
		fileInput.setAttribute('id', 'image-' + imageIndexIdNum);
		fileInput.addEventListener('tap', function(event) {
			var self = this;
			var index = (this.id).substr(-1);
			plus.gallery.pick(function(e) {
				commonUtil.compressImage(e, function(zip) {
					size += zip.size
					console.log("filesize:" + zip.size + ",totalsize:" + size);
					if(size > (10 * 1024 * 1024)) {
						return mui.toast('文件超大,请重新选择~');
					}
					if(!self.parentNode.classList.contains('space')) { //已有图片
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
				});
			}, function(e) {
				mui.toast(e.message);
			}, {});
		});
		placeholder.appendChild(closeButton);
		placeholder.appendChild(up);
		placeholder.appendChild(fileInput);
		weibo.imageList.appendChild(placeholder);
	};
	//删除图片事件
	mui('#image-list').on('tap', '.image-close', function() {
		var placehold = this.parentNode;
		var index = $(placehold).prevAll().length; //判断该节点前还有几个同级节点
		console.log(index);
		weibo.removeFile(index);
		var placehold = this.parentNode;
		weibo.imageList.removeChild(placehold);
		weibo.newPlaceholder();
	});
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
			var path = pathArr[i];
			//删除图标
			var closeButton = document.createElement('div');
			closeButton.setAttribute('class', 'image-close');
			closeButton.innerHTML = 'X';
			weibo.addFile(path);
			placehold.style.backgroundImage = 'url(' + path + ')';
			placehold.appendChild(closeButton);
			weibo.imageList.appendChild(placehold);
		}
		weibo.newPlaceholder();
	}
	//获取地理位置
	weibo.locationBtn.addEventListener("toggle", function(event) {
		if(event.detail.isActive) {
			console.log("你启动了开关");
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
		weibo.send(mui.extend({}, weibo.deviceInfo, weibo.gisinfo, {
			Description: weibo.description.value,
			images: weibo.files,
			score: '' + starIndex
		}))
	}, false)
	weibo.send = function(content) {
		commonUtil.uploadImageWithFomedata(url, weibo.files, content, function(data) {
			//上传成功，重置表单
			if(data.IsSuccess === 1) {
				plus.webview.getWebviewById("tab-webview-subpage-weibo.html").reload();
				//获得主页面的webview
				var main = plus.webview.currentWebview().parent();
				//触发主页面的gohome事件
				mui.fire(main, 'gohome');
				mui.back();
			} else {
				console.log(JSON.stringify(data));
				alert(data.Description);
			}
		})

	};

	//绑定qq表情
	$('.emotion').qqFace({
		assign: 'description',
		path: '../../js/qqFace/arclist/' //表情存放的路径
	});
})();