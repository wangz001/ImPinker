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

//轮播图滚动事件监听；
document.querySelector('.mui-slider').addEventListener('slide', function(event) {
	//注意slideNumber是从0开始的；
	document.getElementById("head-page-num").innerText = (event.detail.slideNumber + 1);
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
			$("#articlelist").html("");
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

var img_600style = 'style/weibo_600';
var img_24style = 'style/articlecover_36_24';
var img_24_20 = 'style/article_24_20';
var img_1200style = 'style/article_1200_605';
var img_900style = 'style/article_900';
var template = $('script[id="article_item"]').html();
function initItem(item, isPullDown) {
	//如果轮播图已显示，跳过。。。暂时不启用
	if($.inArray(item.Id, sliderIds) != -1) {
		//return;
	}
	allArticles.push(item);
	var articleHtmlStr;
	item.CoverImage = item.CoverImage.replace(img_24style, img_24_20);
	articleHtmlStr = template.temp(item);
	if(isPullDown) {
		//下拉刷新，新纪录插到最前面；(后面实现)
		$("#articlelist").append(articleHtmlStr);
	} else {
		$("#articlelist").append(articleHtmlStr);
	}
}

//轮播图文章id，展示下面的列表时排重
var sliderIds = new Array();
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
		sliderIds = new Array();
		var template_a = $('script[id="slide_item_a"]').html();
		var template_b = $('script[id="slide_item_b"]').html();
		var aTopStr = template_a.temp(list[list.length - 1]);
		$("#sliderContent").append(aTopStr);
		for(var i = 0; i < list.length; i++) {
			var item = list[i];
			item.CoverImage = item.CoverImage.replace(img_900style, img_1200style);
			var tempStr = template_b.temp(item);
			$("#sliderContent").append(tempStr);
			sliderIds.push(item.Id);
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
		var titleNView = { //详情页原生导航配置
				backgroundColor: '#f7f7f7', //导航栏背景色
				titleText: '', //导航栏标题
				titleColor: '#000000', //文字颜色
				titleText:articlename,
				type: 'transparent', //透明渐变样式
				autoBackButton: true, //自动绘制返回箭头
				splitLine: { //底部分割线
					color: '#cccccc'
				}
		}
		var webview_style = {
			"render": "always",
			"popGesture": "hide",
			"bounce": "vertical",
			"bounceBackground": "#efeff4",
			"titleNView": titleNView
		};
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
	}
});