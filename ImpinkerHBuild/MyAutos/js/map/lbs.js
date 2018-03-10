mui.init({
	swipeBack: true,
	pullRefresh: {
		container: '#pullrefresh',
		down: {
			callback: pullupRefresh
		},
		up: {
			contentrefresh: '正在加载...',
			auto: false,
			contentnomore: '没有更多数据了',
			callback: pullupRefresh
		}
	}
});
mui.plusReady(function() {
	var self = plus.webview.currentWebview();
	var weiboid = self.weiboid;
	//	console.log(weiboid);
	getweibo(weiboid);
});

var pageNum = 1;
var allCount = 0;
var pageSize = 30;
var lan = "";
var lng = "";
var solrQueryData = {
	pagenum: 1,
	allCount: 0,
	pagesize: 10,
	lat: '',
	lng: '',
	weiboId: 0,
	distance: 10,
	userid: 1
}

function getweibo(weiboid) {
	var url = commonConfig.apiRoot+'/api/weibo/GetWeiBoById';
	var data = {
		weiboid: weiboid,
	};
	plus.nativeUI.showWaiting('正在加载');
	commonUtil.sendRequestGet(url, data, function(data) {
		plus.nativeUI.closeWaiting();
		if(data.IsSuccess == 1 && data.Data != null) {
			var weiboitem = data.Data;
			//console.log(JSON.stringify(weiboitem));
			$("#lbs-title").html(weiboitem.LocationText);
			solrQueryData.lat = weiboitem.Lantitude;
			solrQueryData.lng = weiboitem.Longitude;
			solrQueryData.weiboId = weiboid;
			pullupRefresh();
			setPoint(weiboitem.Longitude, weiboitem.Lantitude, weiboitem.LocationText);
		}
	});
}

function pullupRefresh() {
	var weiboApiPath = commonConfig.apiRoot+'/api/Search/GetWeiboByGeo';
	var dataPara = solrQueryData;
	commonUtil.sendRequestGet(weiboApiPath, dataPara, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.WeiboList.length > 0) {
			var list = data.Data.WeiboList;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItem(item);
			}
			solrQueryData.pagenum = solrQueryData.pagenum + 1;
			mui('#pullrefresh').pullRefresh().endPullupToRefresh();
		} else {
			//没有更多数据
			mui.toast("没有找到附近的途迹");
			mui('#pullrefresh').pullRefresh().endPullupToRefresh(true);
			return;
		}
	});
}

var img_1200style = 'style/weibo_1200';
var img_200style = 'style/weibo_200_200';
var img_24style = 'style/weibo_24_16';
var img_60style = 'style/weibo_60_34';
var weiboTemplate = $('script[id="weiboitem"]').html();
/**
 * 添加数据
 * @param {Object} table
 * @param {Object} item
 */
function initWeiBoItem(item) {
	if(item.ContentValue == null || item.ContentValue == '') {
		return;
	}
	var imgHtmlStr = "";
	var imgs = item.ContentValue.split(',');
	if(imgs.length > 1) {
		//多图
		for(var j = 0; j < imgs.length; j++) {
			imgHtmlStr += '<a href="javascript:;"><img src="' + imgs[j].replace(img_24style, img_200style) + '" data-preview-src="' + imgs[j].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
		}
	} else {
		imgHtmlStr = '<a href="javascript:;"><img class="bigimage" src="' + imgs[0].replace(img_24style, img_60style) + '" class="bigimage" data-preview-src="' + imgs[0].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
	}
	item.imglist = imgHtmlStr;
	//显示地理位置
	item.cardlocation = "";
		if(item.LocationText != "" && item.Longitude != "" && item.Lantitude != "") {
			var lbsStr = "<div class='card-location'><span class='mui-icon mui-icon-location'></span><span>距离3km  " + item.LocationText + "</span></div>";
			item.cardlocation = lbsStr;
		}
	var cardStr = weiboTemplate.temp(item);
	$(".mui-scroll ul").append(cardStr);

}

//跳转 
mui('#pullrefresh').on('tap', 'li', function() {
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
		mui.openWindow({
			url: "../article/preview.html",
			id: "preview",
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


//地图-----------------------------------------------------
var em = null,
	map = null;
var mapHeight = 0;
// H5 初始化地图
function setPoint(lon, lan, locationtext) {
	em = document.getElementById("map");
	// 确保DOM解析完成
	if(!em || !window.plus || map) {
		return
	};
	var point = new plus.maps.Point(lon, lan);
	map = new plus.maps.Map("map", {
		position: "absolute"
	});
	map.centerAndZoom(point, 12);
	//显示气泡图标
	var marker = new plus.maps.Marker(new plus.maps.Point(lon, lan));
	marker.setIcon("../../images/icon-location.png");
	//marker.setLabel("HBuilder");
	var bubble = new plus.maps.Bubble(locationtext);
	marker.setBubble(bubble);
	map.addOverlay(marker);
}

//获得滚动事件
var wh = $(window).height();
//console.log(wh);
var mapSizeSmall = true;
$(window).scroll(function() {
	var top = $(window).scrollTop();
	//console.log(top);
	if(top > 100 && !mapSizeSmall && em && map) {
		em.style.height = "100px";
		map.resize();
		$(".mui-scroll").css("margin-top", "100px");
		mapSizeSmall = true;
		console.log("small");
	}
	if(top < 50 && mapSizeSmall && em && map) {
		em.style.height = "300px";
		map.resize();
		$(".mui-scroll").css("margin-top", "300px");
		mapSizeSmall = false;
		console.log("big");
	}
});