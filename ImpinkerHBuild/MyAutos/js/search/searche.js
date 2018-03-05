mui.init({
	swipeBack: true,
	pullRefresh: {
		container: '#pullrefresh',
		up: {
			contentrefresh: '正在加载...',
			auto: true,
			contentnomore: '没有更多数据了',
			callback: pullupRefresh
		}
	}
});
mui.plusReady(function() {
	$("#btnsearch").bind('click', function() {
		var txt = $("#searchbox").val();
		console.log(txt);
		if(txt != "") {
			param.keyword = txt;
			param.param = 1;
			search();
			$("#cardlist").html("");
			plus.nativeUI.showWaiting('正在加载数据。。。');
		}
	});
});

var url = 'http://api.myautos.cn/api/Search/SearchByKeyword';
var param = {
	userid: 1,
	pageNum: 1,
	pagesize: 10,
	keyword: "自驾游"
};

function pullupRefresh() {
	search();
}
///搜索
function search() {
	commonUtil.sendRequestGet(url, param, function(data) {
		plus.nativeUI.closeWaiting();
		//console.log(JSON.stringify(data));
		if(data.IsSuccess == 1 && data.Data != null) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				if(item.EntityType == 1) {
					var article = item.ArticleVm;
					initCard(1, article);
				}
				if(item.EntityType == 2) {
					var weibo = item.WeiboVm;
					initCard(2, weibo);
				}
			}
			param.pageNum++;
		} else {
			mui('#pullrefresh').pullRefresh().endPullupToRefresh(true);
			return;
		}
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}

var img_1200style = 'style/weibo_1200';
var img_600style = 'style/weibo_600';
var img_24style = 'style/weibo_24_16';
var img_36style = 'style/weibo_36_24';
var articleTemplate = $('script[id="card-article"]').html();
var weiboTemplate = $('script[id="card-weibo"]').html();
///填充内容
function initCard(entityType, item) {
	item.CreateTime = item.CreateTime.substring(0, 10);
	if(entityType == 1) {
		var article = item;
		//console.log(JSON.stringify(article));
		if(article.CoverImage != null && article.CoverImage != "") {
			article.CoverImage = article.CoverImage.replace("style/articlecover_36_24", "style/weibo_600")
		} else {
			article.CoverImage = "http://img.myautos.cn/articlefirstimg/20170717/1_1174_636359314136863157.jpg?x-oss-process=style/articlecover_36_24";
		}
		var imgHtmlStr = articleTemplate.temp(article);
		$("#cardlist").append(imgHtmlStr);
	}
	if(entityType == 2) {
		//console.log(JSON.stringify(item));
		var weibo = item;
		var imgHtmlStr = "";
		var imgs = weibo.ContentValue.split(',');
		imgHtmlStr += "<ul>";
		for(var j = 0; j < imgs.length; j++) {
			imgHtmlStr += '<li><img src="' + imgs[j] + '" data-preview-src="' + imgs[j].replace(img_24style, img_1200style) + '" data-preview-group="' + weibo.Id + '"></li>';
		}
		imgHtmlStr += "</ul>";
		weibo.imglist = imgHtmlStr;
		var cardStr = weiboTemplate.temp(weibo);
		$("#cardlist").append(cardStr);
	}
}

//列表项的点击事件  
mui('#cardlist').on('tap', '.card-article', function(e) {
	var articleid = $(this).attr("articleid");
	var articlename = $(this).attr("articlename");
	if(articleid.indexOf("travels_") != -1) {
		articleid = articleid.replace("travels_", "");
	}
	console.log(articleid);
	var titleNView =commonConfig.titleNView;
	titleNView.titleText=articlename;
	//获得详情页面  
	mui.openWindow({
		id: "preview.html",
		url: "../article/preview.html",
		styles: {
			"titleNView": titleNView
		},
		show: {
			aniShow: "pop-in"
		},
		waiting: {
			autoShow: false
		},
		extras: {
			articleid: articleid, //扩展参数
			articlename: ''
		}
	});
});
//微博
mui('#cardlist').on('tap', '.card-weibo', function(e) {
	var weiboid = this.getAttribute("weiboid");
	if(weiboid.indexOf("travels_") != -1) {
		weiboid = weiboid.replace("travels_", "");
	}
	console.log(weiboid);
	//获得详情页面  
	mui.openWindow({
		url: "../weibo/weibo_preview.html",
		id: "weibo_preview",
		extras: {
			weiboid: weiboid
		},
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});