//mui初始化
mui.init({
	statusBarBackground: '#f7f7f7'
});

var subpages = ['tab-webview-subpage-article.html', 'tab-webview-subpage-weibo.html', 'tab-webview-subpage-setting.html'];
var subpage_style = {
	top: '45px',
	bottom: '50px'
};
//原生到航头
var titleNView = {
	backgroundColor: '#f7f7f7', //导航栏背景色
	titleText: '游记', //导航栏标题
	titleColor: '#000000', //文字颜色
	type: 'transparent', //透明渐变样式
	autoBackButton: false, //自动绘制返回箭头
	titleSize: "20px",//标题字体大小
	splitLine: { //底部分割线
		color: '#cccccc'
	}
}

var aniShow = {};
//登录页跳转过来的事件
window.addEventListener('show', function() {
	var defaultTab = document.getElementById("defaultTab");
	//模拟首页点击
	mui.trigger(defaultTab, 'tap');
	//切换选项卡高亮
	var current = document.querySelector(".mui-bar-tab>.mui-tab-item.mui-active");
	if(defaultTab !== current) {
		current.classList.remove('mui-active');
		defaultTab.classList.add('mui-active');
	}
}, false);
//创建子页面，首个选项卡页面显示，其它均隐藏；
mui.plusReady(function() {
	//读取本地存储，检查是否为首次启动
	var showGuide = plus.storage.getItem("lauchFlag");
	//仅支持竖屏显示
	plus.screen.lockOrientation("portrait-primary");
	if(showGuide) {
		//有值，说明已经显示过了，无需显示；
		//显示广告
		var adShow = false;
		if(adShow) {
			//显示启动导航
			mui.openWindow({
				id: 'ad',
				url: 'index.html',
				styles: {
					popGesture: "none"
				},
				show: {
					aniShow: 'none'
				},
				waiting: {
					autoShow: false
				}
			});
		} else {
			//关闭splash页面；
			plus.navigator.closeSplashscreen();
			plus.navigator.setFullscreen(false);
		}
		setTimeout(function() {
			//预加载
			preload()
		}, 200);
	} else {
		//显示启动导航
		mui.openWindow({
			id: 'guide',
			url: 'view/guide.html',
			styles: {
				popGesture: "none"
			},
			show: {
				aniShow: 'none'
			},
			waiting: {
				autoShow: false
			}
		});
		//延迟的原因：优先打开启动导航页面，避免资源争夺
		setTimeout(function() {
			//预加载
			preload()
		}, 200);
	}
	setTimeout(function() {
		$(".zhezhaoDiv").hide();
	}, 500)

});

function preload() {
	//关闭login页
	var self = plus.webview.currentWebview();
	for(var i = 0; i < 3; i++) {
		var temp = {};
		//var sub = plus.webview.create(subpages[i], subpages[i], subpage_style);
		var sub;
		//不使用mui的窗口打开方式
		if(i == 1) {
			titleNView.titleText="途迹";
			sub = plus.webview.create(subpages[i], subpages[i], {
				titleNView: titleNView,
				top: '0px',
				bottom: '50px'
			});
		} else {
			if(i==0){
				titleNView.titleText="游记";
			}else{
				titleNView.titleText="个人中心";
			}
			sub = plus.webview.create(subpages[i], subpages[i], {
				titleNView: titleNView,
				top: '0px',
				bottom: '50px'
			});
		}

		//webview.show("slide-in-right", 300);

		if(i > 0) {
			sub.hide();
		} else {
			temp[subpages[i]] = "true";
			mui.extend(aniShow, temp);
		}
		self.append(sub);
	}
	//更新token
	setTimeout(function() {
		app.updateToken(function(data) {
			console.log(JSON.stringify(data));
			if(data.IsSuccess == 1) {
				console.log('更新token成功：' + data.Data);
			} else {
				mui.toast('更新token失败:' + data.Description);
			}
		});
	}, 1000 * 5);
	//--双击退出
	mui.oldBack = mui.back;
	var backButtonPress = 0;
	mui.back = function(event) {
		backButtonPress++;
		if(backButtonPress > 1) {
			plus.runtime.quit();
		} else {
			plus.nativeUI.toast('再按一次退出应用');
		}
		setTimeout(function() {
			backButtonPress = 0;
		}, 1000);
		return false;
	};
}

//当前激活选项
var activeTab = subpages[0];
var title = document.getElementById("title");
//选项卡点击事件
mui('.mui-bar-tab').on('tap', 'a', function(e) {
	var tabId = this.getAttribute("id");
	//显示添加按钮或微博按钮
	if(tabId != null && tabId == "newweibo") {
		newWeibo();
		return;
	}
	//tab 选中状态
	changeTabStates(tabId);

	//--------------------
	var targetTab = this.getAttribute('href');
	if(targetTab == activeTab) {
		return;
	}
	//更换标题
	//title.innerHTML = this.querySelector('.mui-tab-label').innerHTML;
	//显示目标选项卡
	//若为iOS平台或非首次显示，则直接显示
	if(mui.os.ios || aniShow[targetTab]) {
		plus.webview.show(targetTab);
	} else {
		//否则，使用fade-in动画，且保存变量
		var temp = {};
		temp[targetTab] = "true";
		mui.extend(aniShow, temp);
		plus.webview.show(targetTab, "fade-in", 300);
	}
	//隐藏当前;
	plus.webview.hide(activeTab);
	//更改当前活跃的选项卡
	activeTab = targetTab;
	///显示隐藏切换
	if(tabId != null && tabId == "weiboTab") {
		$("#weiboTab").hide();
		$("#newweibo").show();
	} else {
		$("#weiboTab").show();
		$("#newweibo").hide();
	}
});

function changeTabStates(tabId) {
	$("#articleTab").find("use").attr("xlink:href", "#icon-youji-moren");
	$("#weiboTab").find("use").attr("xlink:href", "#icon-tujimoren");
	$("#mineTab").find("use").attr("xlink:href", "#icon-wode-moren");
	if(tabId != null && tabId == "articleTab") {
		$("#articleTab").find("use").attr("xlink:href", "#icon-youji-moren-copy");
	}
	if(tabId != null && tabId == "weiboTab") {
		$("#weiboTab").find("use").attr("xlink:href", "#icon-tujimoren-copy");
	}
	if(tabId != null && tabId == "mineTab") {
		$("#mineTab").find("use").attr("xlink:href", "#icon-wode-moren-copy");
	}
}

//自定义事件，模拟点击“首页选项卡”
document.addEventListener('gohome', function(event) {
	console.log("******************:gohome_weibo");
	var defaultTab = document.getElementById("articleTab");
	//模拟首页点击
	mui.trigger(defaultTab, 'tap');
	//切换选项卡高亮
	var current = document.querySelector(".mui-bar-tab>.mui-tab-item.mui-active");
	if(defaultTab !== current) {
		current.classList.remove('mui-active');
		defaultTab.classList.add('mui-active');
	}
});
//自定义事件，模拟点击“首页选项卡”
document.addEventListener('gohome_weibo', function(event) {
	console.log("******************:gohome_weibo");
	var weiboTab = document.getElementById("weiboTab");
	//模拟首页点击
	mui.trigger(weiboTab, 'tap');
	//切换选项卡高亮
	var current = document.querySelector(".mui-bar-tab>.mui-tab-item.mui-active");
	if(weiboTab !== current) {
		current.classList.remove('mui-active');
		weiboTab.classList.add('mui-active');
	}
});

function newWeibo() {
	var btnArray = [{
			title: '拍照'
		}, {
			title: '从相册选择'
		}, {
			title: '一键发布游记'
		}
		//				{
		//					title: '摄像'
		//				}
	];
	plus.nativeUI.actionSheet({
		title: "选择文件",
		cancel: '取消',
		buttons: btnArray
	}, function(e) {
		var index = e.index; // 
		switch(index) {
			case 1:
				//拍照
				commonUtil.captureImage(function(path) {
					var arr = new Array();
					arr.push(path);
					//toNewWeiBo(arr);
					//清空数组
					compressimgArr = new Array();
					compressImage(arr);
				});
				break;
			case 2:
				//从相册选择图片
				plus.gallery.pick(function(e) {
					console.log(JSON.stringify(e.files));
					//toNewWeiBo(e.files);
					//清空数组
					compressimgArr = new Array();
					compressImage(e.files);
				}, function(e) {
					console.log("取消选择图片");
				}, {
					filter: "image",
					multiple: true,
					maximum: 6,
					system: false,
					onmaxed: function() {
						plus.nativeUI.alert('最多只能选择6张图片');
					}
				});
				break;
			case 3:
				var btn = ['是', '否'];
				mui.confirm("选择途迹快速生成游记", "是否选择？", btn, function(event) {
					console.log(JSON.stringify(event));
					if(event.index == 0) {
						mui.openWindow({
							url: "view/article/youji_new_selectweibo.html",
							id: "youji_new_selectweibo",
							createNew: true,
							show: {
								aniShow: 'slide-in-right',
								duration: 200
							}
						});
					} else {
						mui.openWindow({
							url: "view/article/youji_new.html",
							id: "youji_new",
							show: {
								aniShow: 'slide-in-right',
								duration: 100
							}
						});
					}
				});

				break;
			case 4:
				//摄像
				commonUtil.videoCapture(10000, function(path) {
					console.log("Capture video success: " + path);
					mui.openWindow({
						url: "view/weibo/weibo_new_video.html",
						id: "newweibovideo",
						extras: {
							path: path //扩展参数
						},
						show: {
							aniShow: 'slide-in-right',
							duration: 200
						}
					});
				});
				break;
		}
	});
}

//新建微博页面
//处理右上角关于图标的点击事件；
document.getElementById('newweibo').addEventListener('tap', function() {
	//newWeibo();
});

var compressimgArr = new Array();

function compressImage(filesArr) {
	if(compressimgArr.length == filesArr.length) {
		//所有图片都压缩完成
		toWeiboPage(compressimgArr);
	} else {
		var imgpath = filesArr[compressimgArr.length];
		commonUtil.compressImage(imgpath, function(zip) {
			if((zip.size / 1024) > 1024) {
				//如果大于1M，继续压缩(ios拍照时会出现此问题)
				console.log('sssss---' + zip.size);
				commonUtil.compressImageByHeightWeight(zip.target, zip.width, zip.height, function(zip2) {
					alert('sssss222---' + (zip2.size / 1024));
					compressimgArr.push(zip2.target);
					if(compressimgArr.length == filesArr.length) {
						//所有图片都压缩完成
						toWeiboPage(compressimgArr);
					} else {
						compressImage(filesArr);
					}
				});
			} else {
				compressimgArr.push(zip.target);
				compressImage(filesArr);
			}
		});
	}
}

function toWeiboPage(imgArr) {
	mui.openWindow({
		url: "view/weibo/weibo_new.html",
		id: "newweibo",
		extras: {
			path: imgArr.join(",") //扩展参数
		},
		createNew: true,
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
}

