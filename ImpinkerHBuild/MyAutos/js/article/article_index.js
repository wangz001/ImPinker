mui.init({
	swipeBack: false,
	pullRefresh: {
		container: '#pullrefresh',
		down: {
			auto: true, //可选,默认false.自动上拉加载一次
			callback: pulldownRefresh
		},
		up: {
			contentrefresh: '正在加载...',
			callback: pullupRefresh
		}
	}
});
mui.plusReady(function() {
	//填充轮播图内容
	initSlide();
	//显示文章缓存
	var articleData = storageUtil.getFirstPageArticle();
	if(articleData != null && articleData.length > 0) {
		for(var i = 0; i < articleData.length; i++) {
			initItem(articleData[i], true);
		}
	}

});

//自定义事件。接受预览页传回的数据
window.addEventListener('articleComment', function(event) {
	//获得事件参数  
	var articleid = event.detail.articleid;
	var count = $("#commentCount_" + articleid).text();
	$("#commentCount_" + articleid).html(parseInt(count) + 1);
});
window.addEventListener('articleVote', function(event) {
	//获得事件参数  
	var articleid = event.detail.articleid;
	var count = $("#voteCount_" + articleid).text();
	$("#voteCount_" + articleid).html(parseInt(count) + 1);
});

var allArticles = new Array();
var pageNum = 1;
var allCount = 0;
var pageSize = 30;
var apiPath = "http://api.myautos.cn/api/Article/GetByPage";
/**
 * 下拉刷新具体业务实现
 */
function pulldownRefresh() {
	allArticles = [];
	pageNum = 1;
	var data = {
		pageNum: pageNum,
		pageSize: pageSize
	}
	commonUtil.sendRequestGet(apiPath, data, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			mui.toast("已为您获取最新数据");
			//$("#articlelist").html("");
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initItem(item, true);
			}
			storageUtil.setFirstPageArticle(list);
			pageNum++;
		} else {
			mui.toast(data.Description);
		}
		mui('#pullrefresh').pullRefresh().endPulldownToRefresh(); //参数为true代表没有更多数据了。
	});
}
/**
 * 上拉加载具体业务实现。设置时间戳。只加载最新更新的文章
 */
function pullupRefresh() {
	var data = {
		pageNum: pageNum,
		pageSize: pageSize
	}
	commonUtil.sendRequestGet(apiPath, data, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initItem(item, false);
			}
			pageNum++;
		} else {
			mui.toast(data.Description);
		}
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}

function initItem(item, isPullDown) {
	var img_600style = 'style/weibo_600';
	var img_24style = 'style/articlecover_36_24';
	allArticles.push(item);
	var template = $('script[id="article_item"]').html();
	var templateCard = $('script[id="article_item_card"]').html();
	var articleHtmlStr;
//	if(allCount % 10 == 5) {
//		item.CoverImage = item.CoverImage.replace(img_24style, img_600style);
//		//console.log(JSON.stringify(item));
//		articleHtmlStr = templateCard.temp(item);
//	} else {
		articleHtmlStr = template.temp(item);
//	}
	if(isPullDown) {
		//下拉刷新，新纪录插到最前面；
		$("#articlelist").append(articleHtmlStr);
	} else {
		$("#articlelist").append(articleHtmlStr);
	}
	allCount++;
}

//显示轮播图内容
function initSlide() {
	//先显示本地缓存的数据
	var sliderData = storageUtil.getSliderData();
	if(sliderData != null && sliderData.length > 0) {
		initHtml(sliderData);
	}

	//获取最新数据
	var slidePath = 'http://api.myautos.cn/api/Article/GetSlideArticle';
	var para = {};
	commonUtil.sendRequestGet(slidePath, para, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			initHtml(data.Data);
			//更新本地缓存数据
			storageUtil.setSliderData(data.Data);
		}
	});

	//显示轮播图
	function initHtml(list) {
		$("#sliderContent").html("");
		var template_a = $('script[id="slide_item_a"]').html();
		var template_b = $('script[id="slide_item_b"]').html();
		var aTopStr = template_a.temp(list[list.length - 1]);
		$("#sliderContent").append(aTopStr);
		for(var i = 0; i < list.length; i++) {
			var item = list[i];
			var tempStr = template_b.temp(item);
			$("#sliderContent").append(tempStr);
		}
		var aBottomStr = template_a.temp(list[0]);
		$("#sliderContent").append(aBottomStr);
		//获得slider插件对象
		var gallery = mui('.mui-slider');
		gallery.slider({
			interval: 5000 //自动轮播周期，若为0则不自动播放，默认为0；
		});
	}
}

/*
 * 列表点击事件
 * */
var aniShow = "pop-in";
var previewPage = null;
mui('#articlelist,#sliderContent').on('tap', 'a', function() {
	var id = this.getAttribute('href');
	var href = this.href;
	var type = "common"; // this.getAttribute("open-type");
	var articleid = this.getAttribute('articleid');
	var articlename = this.getAttribute('articlename');
	//不使用父子模板方案的页面
	if(type == "common") {
		var webview_style = {
			popGesture: "close",
		};
		//侧滑菜单需动态控制一下zindex值；
		if(~id.indexOf('offcanvas-')) {
			webview_style.zindex = 9998;
			webview_style.popGesture = ~id.indexOf('offcanvas-with-right') ? "close" : "none";
		}
		//图标界面需要启动硬件加速
		if(~id.indexOf('icons.html')) {
			webview_style.hardwareAccelerated = true;
		}
		//打开详情页面            
		mui.openWindow({
			id: id,
			url: this.href,
			styles: webview_style,
			show: {
				aniShow: aniShow
			},
			waiting: {
				autoShow: false
			},
			extras: {
				articleUrl: "http://m.myautos.cn/Article/Index?id=" + articleid,
				articleid: articleid, //扩展参数
				articlename: articlename
			}
		});
	} else if(id && ~id.indexOf('.html')) {
		if(!mui.os.plus || (!~id.indexOf('popovers.html') && mui.os.ios)) {
			mui.openWindow({
				id: id,
				url: this.href,
				styles: {
					popGesture: 'close'
				},
				show: {
					aniShow: aniShow
				},
				waiting: {
					autoShow: false
				}
			});
		} else {
			//TODO  by chb 当初这么设计，是为了一个App中有多个模板，目前仅有一个模板的情况下，实际上无需这么复杂
			//使用父子模板方案打开的页面
			//获得共用模板组
			var template = getTemplate('default');
			//判断是否显示右上角menu图标；
			var showMenu = ~href.indexOf('popovers.html') ? true : false;
			//获得共用父模板
			var headerWebview = template.header;
			//获得共用子webview
			var contentWebview = template.content;
			var title = this.innerText.trim();
			//通知模板修改标题，并显示隐藏右上角图标；
			mui.fire(headerWebview, 'updateHeader', {
				title: title,
				showMenu: showMenu,
				target: href,
				aniShow: aniShow
			});

			if(mui.os.ios || (mui.os.android && parseFloat(mui.os.version) < 4.4)) {
				var reload = true;
				if(!template.loaded) {
					if(contentWebview.getURL() != this.href) {
						contentWebview.loadURL(this.href);
					} else {
						reload = false;
					}
				} else {
					reload = false;
				}
				(!reload) && contentWebview.show();

				headerWebview.show(aniShow, 150);
			}
		}
	}
});