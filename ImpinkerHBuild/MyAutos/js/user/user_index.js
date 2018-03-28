mui.init();
mui.plusReady(function() {
	//阻尼系数
	var deceleration = mui.os.ios ? 0.003 : 0.0009;
	mui('.mui-scroll-wrapper').scroll({
		bounce: false,
		indicators: true, //是否显示滚动条
		deceleration: deceleration
	});
	//获取参数
	var self = plus.webview.currentWebview();
	currentParamEntity.userid = self.userid;
	var username = self.username;
	var headimg = self.headimg;
	setUserinfo(username, headimg);
	//默认加载第一页
	getWeiboList(function(data) {});
	getArticleList(function(data) {});

	mui("#item1mobile .mui-scroll").pullToRefresh({
		up: {
			callback: function() {
				var self = this;
				getWeiboList(function(data) {
					if(data.IsSuccess == 1 && data.Data != null) {
						self.endPullUpToRefresh();
					} else {
						self.endPullUpToRefresh(true);
					}
				});
			}
		}
	});
	mui("#item2mobile .mui-scroll").pullToRefresh({
		up: {
			callback: function() {
				var self = this;
				getArticleList(function(data) {
					if(data.IsSuccess == 1 && data.Data != null) {
						self.endPullUpToRefresh();
					} else {
						self.endPullUpToRefresh(true);
					}
				});
			}
		}
	});

	//监听滚动事件
	var wh = $(window).height();
	//console.log(wh);
	var isTabShow=false;
	$("#scroll1").scroll(function() {
		var top = $(window).scrollTop();
		console.log(top);
		if(top > 100 && isTabShow) {
			$("#sliderSegmentedControl").show();
			isTabShow = true;
			
			console.log("small");
		}
	});

});
mui.previewImage();
var currentParamEntity = {
	userid: 0,
	all: {
		url: commonConfig.apiRoot + '/api/Article/GetUserArticleAndWeiboListByPage',
		pageindex: 1,
		pagesize: 10
	},
	article: {
		url: commonConfig.apiRoot + '/api/Article/GetUsersArticleByPage',
		pageindex: 1,
		pagesize: 10
	},
	weibo: {
		url: commonConfig.apiRoot + '/api/weibo/GetUsersListByPage',
		pageindex: 1,
		pagesize: 10
	},
	current: 1,
	articleids: new Array(),
	weiboids: new Array()

}

function getArticleList(callback) {
	callback = callback || $.noop;
	var url = currentParamEntity.article.url;
	var data = {
		userid: currentParamEntity.userid,
		pageindex: currentParamEntity.article.pageindex,
		pagesize: currentParamEntity.article.pagesize
	};
	commonUtil.sendRequestGet(url, data, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initCard(1, item);
			}
			currentParamEntity.article.pageindex++;
		}
		return callback(data);
	});
}

function getWeiboList(callback) {
	callback = callback || $.noop;
	var url = currentParamEntity.weibo.url;
	var data = {
		userid: currentParamEntity.userid,
		pageindex: currentParamEntity.weibo.pageindex,
		pagesize: currentParamEntity.weibo.pagesize
	};
	commonUtil.sendRequestGet(url, data, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initCard(2, item);
			}
			currentParamEntity.weibo.pageindex++;
		}
		return callback(data);
	});
}

var articleTemplate = $('script[id="card-article"]').html();
var weiboTemplate = $('script[id="card-weibo"]').html();
///填充内容
function initCard(entityType, item) {
	if(entityType == 1) {
		var article = item;
		if($.inArray(article.Id, currentParamEntity.articleids) == -1) {
			article.CoverImage = article.CoverImage.replace(commonConfig.imgStyle.articlecover_36_24, commonConfig.imgStyle.article_24_20)
			var imgHtmlStr = articleTemplate.temp(article);
			$("#mui-table-view-2").append(imgHtmlStr);
			currentParamEntity.articleids.push(article.Id);
		}
	}

	if(entityType == 2) {
		var weibo = item;
		if($.inArray(weibo.Id, currentParamEntity.weiboids) == -1) {
			var imgHtmlStr = "";
			var imgs = weibo.ContentValue.split(',');
			if(imgs.length > 1) {
				//多图
				for(var j = 0; j < imgs.length; j++) {
					imgHtmlStr += '<div class="img-box"><img src="' + imgs[j].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_200_200) + '" data-preview-src="' + imgs[j].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_1200) + '" data-preview-group="' + item.Id + '"></div>';
				}
			} else {
				imgHtmlStr = '<a href="#"><img class="bigimage" src="' + imgs[0].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_60_34) + '" class="bigimage" data-preview-src="' + imgs[0].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_1200) + '" data-preview-group="' + item.Id + '"></a>';
			}
			weibo.imglist = imgHtmlStr;
			item.Description = replace_em(item.Description);
			//显示地理位置
			weibo.cardlocation = "";
			if(weibo.LocationText != "" && weibo.Longitude != "" && weibo.Lantitude != "") {
				var lbsStr = "<div weiboid='" + weibo.Id + "' class='card-location'><span class='mui-icon mui-icon-location'></span><span>" + weibo.LocationText + "</span></div>";
				weibo.cardlocation = lbsStr;
			}
			var cardStr = weiboTemplate.temp(weibo);
			$("#mui-table-view-1").append(cardStr);
			currentParamEntity.weiboids.push(weibo.Id);
		}
	}
}

function replace_em(str) {
	str = str.replace(/\</g, '&lt;');
	str = str.replace(/\>/g, '&gt;');
	str = str.replace(/\n/g, '<br/>');
	str = str.replace(/\[em_([0-9]*)\]/g, '<img src="../../js/qqFace/arclist/$1.gif" border="0" />');
	return str;
}

//
function setUserinfo(username, headimg) {
	$("#header .mui-title").html(username);
	$(".personal-top .head-img img").attr("src", headimg);
}

//跳转 
mui('.mui-table-view').on('tap', 'li', function() {
	var cardType = $(this).attr("class");
	//	if(cardType.indexOf("weibo-card") != -1) {
	//		var weiboid = $(this).attr("weiboid");
	//		return; //暂时不跳转
	//		mui.openWindow({
	//			url: "../weibo/weibo_preview.html",
	//			id: "weibo_preview",
	//			extras: {
	//				weiboid: weiboid
	//			},
	//			show: {
	//				aniShow: 'slide-in-right',
	//				duration: 200
	//			}
	//		});
	//	}
	if(cardType.indexOf("article_card") != -1) {
		var articleid = $(this).attr("articleid");
		var articleName = $(this).attr("articlename");
		var titleNView = commonConfig.titleNView;
		titleNView.titleText = articleName;
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
//跳转到微博
mui('#mui-table-view-1').on('tap', '.user_info,.user_detail .text', function() {
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
});