mui.init();
document.getElementById("head-img").addEventListener('tap', function(e) {
	e.stopPropagation();
});

mui.plusReady(function() {
	setTimeout(function() {
		initImgPreview();
	}, 300);
	//显示头像及用户名等
	//username
	var user = app.getState();
	var showName = user.showname == "" ? user.account : user.showname;
	$("#username").html(showName);
	if(user.imgurl != "") {
		$("#head-img").attr("src", user.imgurl);
	} else {
		defaultImg();
	}
	$("#phonenum").html(user.phonenum);
	$("#emailstr").html(user.email);
});

function defaultImg() {
	if(mui.os.plus) {
		plus.io.resolveLocalFileSystemURL("_doc/head.jpg", function(entry) {
			var s = entry.fullPath + "?version=" + new Date().getTime();;
			document.getElementById("head-img").src = s;
			document.querySelector("#__mui-imageview__group .mui-slider-item img").src = s;
		}, function(e) {
			document.getElementById("head-img").src = '../../images/logo.png';
		})
	} else {
		document.getElementById("head-img").src = '../../images/logo.png';
		document.querySelector("#__mui-imageview__group .mui-slider-item img").src = '../../images/logo.png';
	}
}
//头像预览
function initImgPreview() {
	var imgs = document.querySelectorAll("img.mui-action-preview");
	imgs = mui.slice.call(imgs);
	if(imgs && imgs.length > 0) {
		var slider = document.createElement("div");
		slider.setAttribute("id", "__mui-imageview__");
		slider.classList.add("mui-slider");
		slider.classList.add("mui-fullscreen");
		slider.style.display = "none";
		slider.addEventListener("tap", function() {
			slider.style.display = "none";
		});
		slider.addEventListener("touchmove", function(event) {
			event.preventDefault();
		})
		var slider_group = document.createElement("div");
		slider_group.setAttribute("id", "__mui-imageview__group");
		slider_group.classList.add("mui-slider-group");
		imgs.forEach(function(value, index, array) {
			//给图片添加点击事件，触发预览显示；
			value.addEventListener('tap', function() {
				slider.style.display = "block";
				_slider.refresh();
				_slider.gotoItem(index, 0);
			})
			var item = document.createElement("div");
			item.classList.add("mui-slider-item");
			var a = document.createElement("a");
			var img = document.createElement("img");
			img.setAttribute("src", value.src);
			a.appendChild(img)
			item.appendChild(a);
			slider_group.appendChild(item);
		});
		slider.appendChild(slider_group);
		document.body.appendChild(slider);
		var _slider = mui(slider).slider();
	}
}

//更换头像
mui(".mui-table-view-cell").on("tap", "#head", function(e) {
	if(mui.os.plus) {
		var a = [{
			title: "拍照"
		}, {
			title: "从手机相册选择"
		}];
		plus.nativeUI.actionSheet({
			title: "修改头像",
			cancel: "取消",
			buttons: a
		}, function(b) {
			switch(b.index) {
				case 0:
					break;
				case 1:
					getImage();
					break;
				case 2:
					galleryImg();
					break;
				default:
					break
			}
		})
	}
});

var testimg = "headtest.jpg";

function getImage() {
	var c = plus.camera.getCamera();
	c.captureImage(function(e) {
		plus.io.resolveLocalFileSystemURL(e, function(entry) {
			var s = entry.toLocalURL() + "?version=" + new Date().getTime();
			console.log(s);
			toCutImg(s);
		}, function(e) {
			console.log("读取拍照文件错误：" + e.message);
		});
	}, function(s) {
		console.log("error" + s);
	}, {
		filename: "_doc/" + testimg
	})
}

function galleryImg() {
	plus.gallery.pick(function(a) {
		plus.io.resolveLocalFileSystemURL(a, function(entry) {
			plus.io.resolveLocalFileSystemURL("_doc/", function(root) {
				root.getFile(testimg, {}, function(file) {
					//文件已存在
					file.remove(function() {
						console.log("file remove success");
						entry.copyTo(root, testimg, function(e) {
								var e = e.fullPath + "?version=" + new Date().getTime();
								toCutImg(e);
							},
							function(e) {
								console.log('copy image fail:' + e.message);
							});
					}, function() {
						console.log("delete image fail:" + e.message);
					});
				}, function() {
					//文件不存在
					entry.copyTo(root, testimg, function(e) {
							var path = e.fullPath + "?version=" + new Date().getTime();
							toCutImg(path);
						},
						function(e) {
							console.log('copy image fail:' + e.message);
						});
				});
			}, function(e) {
				console.log("get _www folder fail");
			})
		}, function(e) {
			console.log("读取拍照文件错误：" + e.message);
		});
	}, function(a) {}, {
		filter: "image"
	})
};

function toCutImg(path) {
	mui.openWindow({
		url: "setting-imgcut.html",
		id: "setting-imgcut",
		extras: {
			path: path //扩展参数
		},
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
}

$("#username").bind("click", function(e) {
	mui.prompt('请输入新的昵称', '', '修改昵称', ['确认', '取消'], function(event, index) {
		console.log(JSON.stringify(event));
		if(event.index == 0) {
			var url = commonConfig.apiRoot + '/api/UserCenter/GetNewShowName';
			var dataPara = {
				showname: event.value
			}
			commonUtil.sendRequestWithToken(url, dataPara, false, function(data) {
				console.log(JSON.stringify(data));
				if(data.IsSuccess == 1) {
					//plus.webview.currentWebview().reload();
					//修改本地存储
					var state = app.getState();
					state.showname=event.value;
					app.setState(state);
					$("#username").html(event.value);
				} else {
					mui.toast("修改失败");
				}
			});
		}
	}, 'div');

});