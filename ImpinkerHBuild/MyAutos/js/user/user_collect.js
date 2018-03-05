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
	var self = plus.webview.currentWebview();
	var userid = self.userid;
});

var pageNum = 1;
var pageSize = 30;
function pullupRefresh() {
	//console.log(JSON.stringify(currentParamEntity));
	var url = 'http://api.myautos.cn/api/usercollection/GetMyCollect';
	var data = {
		pageNum: pageNum,
		pagesize: pageSize
	};
	commonUtil.sendRequestWithToken(url, data, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				if(item.EntityType == 1) {
					var article = item.ArticleVm;
					article.collectTime = item.CreateTime.substring(0, 10);
					initCard(1, article);
				}
				if(item.EntityType == 2) {
					var weibo = item.WeiboVm;
					weibo.collectTime = item.CreateTime.substring(0, 10);
					initCard(2, weibo);
				}
			}
			pageNum++;
		} else {
			mui('#pullrefresh').pullRefresh().endPullupToRefresh(true);
			return;
		}
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}

var img_1200style = 'style/weibo_1200';
var img_24style = 'style/weibo_24_16';
var img_200style = 'style/weibo_200_200';
var img_60style = 'style/weibo_60_34';
var articleTemplate = $('script[id="card-article"]').html();
var weiboTemplate = $('script[id="card-weibo"]').html();
///填充内容
function initCard(entityType, item) {
	if(entityType == 1) {
		var article = item;
		article.CoverImage = article.CoverImage.replace("style/articlecover_36_24", "style/article_24_20")
		var imgHtmlStr = articleTemplate.temp(article);
		$("#cardlist").append(imgHtmlStr);
	}

	if(entityType == 2) {
		var weibo = item;
		var imgHtmlStr = "";
		var imgs = weibo.ContentValue.split(',');
		if(imgs.length > 1) {
			//多图
			for(var j = 0; j < imgs.length; j++) {
				imgHtmlStr += '<a href="#"><img src="' + imgs[j].replace(img_24style, img_200style) + '" data-preview-src="' + imgs[j].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
			}
		} else {
			imgHtmlStr = '<a href="#"><img class="bigimage" src="' + imgs[0].replace(img_24style, img_60style) + '" class="bigimage" data-preview-src="' + imgs[0].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
		}
		weibo.imglist = imgHtmlStr;
		var cardStr = weiboTemplate.temp(weibo);
		$("#cardlist").append(cardStr);
	}
}


//跳转 
mui('#cardlist').on('tap', 'li', function() {
	var cardType = $(this).attr("class");
	if(cardType.indexOf("weibo-card") != -1) {
		var weiboid = $(this).attr("weiboid");
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
	}
	if(cardType.indexOf("article_card") != -1) {
		var articleid = $(this).attr("articleid");
		var articleName=$(this).attr("articlename");
		var titleNView =commonConfig.titleNView;
		titleNView.titleText=articleName;
		var webview_style = {
			"render": "always",
			"popGesture": "hide",
			"bounce": "vertical",
			"bounceBackground": "#efeff4",
			"titleNView": titleNView
		};
		
		mui.openWindow({
			url: "../article/preview.html",
			id: "preview",
			styles: webview_style,
			extras: {
				articleid: articleid
			},
			show: {
				aniShow: 'slide-in-right',
				duration: 200
			}
		});
	}
});

//删除  
mui('#cardlist').on('tap', '.delete_weibo', function(e) {
	var weiboid=$(this).attr("weiboid");
	alert(weiboid);
	e.stopPropagation();
});