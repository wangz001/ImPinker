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
var pageSize = 30;
var apiPath = commonConfig.apiRoot + "/api/Article/GetByPage";
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

var template = $('script[id="article_item"]').html();

function initItem(item, isPullDown) {
	//如果轮播图已显示，跳过。。。暂时不启用
	if($.inArray(item.Id, sliderIds) != -1) {
		//return;
	}
	allArticles.push(item);
	var articleHtmlStr;
	item.CoverImage = item.CoverImage.replace(commonConfig.imgStyle.articlecover_36_24, commonConfig.imgStyle.article_24_20);
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
	var slidePath = commonConfig.apiRoot + '/api/Article/GetSlideArticle';
	var para = {};
	commonUtil.sendRequestGet(slidePath, para, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			initHtml(data.Data);
			//initSwiperSlide();
			//更新本地缓存数据
			storageUtil.setSliderData(data.Data);
		}
	});

	//显示轮播图
	function initHtml(list) {
		$("#sliderContent").html("");
		sliderIds = new Array();
		var template_swiper_slide = $('script[id="swiper-slide"]').html();
		for(var i = 0; i < list.length; i++) {
			var item = list[i];
			item.CoverImage = item.CoverImage.replace(commonConfig.imgStyle.article_900, commonConfig.imgStyle.article_1200_605);
			var tempStr = template_swiper_slide.temp(item);
			$("#sliderContent").append(tempStr);
			sliderIds.push(item.Id);
		}
		//获得slider插件对象
		initSwiperSlide();
	}

	//初始化轮播图
	var mySwiper;

	function initSwiperSlide() {
		//初始化轮播图/////防止重复初始化
		if(mySwiper != null) {
			mySwiper.destroy();
			console.log("2343434");
		}
		mySwiper = new Swiper('.swiper-container', {
			speed: 800,
			direction: 'horizontal',
			autoplay: true, //可选选项，自动滑动
			centeredSlides: true,
			slidesPerView: 'auto',
			spaceBetween: 15, // 中间的距离
			loop: true,
			loopedSlides: 3,
			slidesOffsetBefore: 0, //设定slide与左边框的预设偏移量（单位px）。
			on: {
				slideChangeTransitionEnd: function() {
					//切换结束时，告诉我现在是第几个slide
					var index = this.activeIndex;
					if(index >= 3) {
						index = index - 3;
					}
					document.getElementById("head-page-num").innerText = (index + 1);
				},
			},
		})
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
		var titleNView = commonConfig.titleNView;
		titleNView.titleText = articlename;
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
				articleUrl: commonConfig.mWebRoot + "/Article/Index?id=" + articleid,
				articleid: articleid, //扩展参数
				articlename: articlename
			},
			createNew: true //是否重复创建同样id的webview，默认为false:不重复创建，直接显示
		});
	}
});