mui.init({
	swipeBack: true,
	pullRefresh: {
		container: '#pullrefresh',
		up: {
			auto: true, //可选,默认false.自动上拉加载一次
			contentrefresh: '正在加载...',
			callback: pullupRefresh
		}
	}

});

//添加列表项的点击事件  
mui('.mui-content').on('tap', 'a', function(e) {
	var id = this.getAttribute('id');
});
mui.previewImage();
var pageNum = 1;
var pageSize = 30;
/**
 * 上拉加载具体业务实现
 */
function pullupRefresh() {
	var data = {
		pageindex: pageNum,
		pageSize: pageSize
	}
	commonUtil.sendRequestWithToken('http://api.myautos.cn/api/weibo/GetMyWeiBoList', data, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			var table = document.body.querySelector('.mui-scroll');
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItem(table, item);
			}
		} else {
			mui('#pullrefresh').pullRefresh().endPullupToRefresh(true);
			return;
		}
		pageNum++;
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}

var img_1200style = 'style/weibo_1200';
var img_600style = 'style/weibo_600';
var img_24style = 'style/weibo_24_16';
var img_36style = 'style/weibo_36_24';
var weiboTemplate = $('script[id="weiboitem"]').html();
/**
 * 添加数据
 * @param {Object} table
 * @param {Object} item
 */
function initWeiBoItem(table, item) {
	if(item.ContentValue == null || item.ContentValue == '') {
		return;
	}
	var imgHtmlStr = "";
	var imgs = item.ContentValue.split(',');
	if(imgs.length > 1) {
		//多图
		//imgHtmlStr += "<ul>";
		for(var j = 0; j < imgs.length; j++) {
			imgHtmlStr += '<a href="#"><img src="' + imgs[j] + '" data-preview-src="' + imgs[j].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
		}
		//imgHtmlStr += "</ul>";
	} else {
		imgHtmlStr = '<a href="#"><img class="bigimage" src="' + imgs[0].replace(img_24style, img_36style) + '" class="bigimage" data-preview-src="' + imgs[0].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
	}
	item.imglist = imgHtmlStr;
	//显示地理位置
	item.cardlocation = "";
	if(item.LocationText != "" && item.Longitude != "" && item.Lantitude != "") {
		var lbsStr = "<div weiboid='" + item.Id + "' class='card-location'><span class='mui-icon mui-icon-location'></span><span>" + item.LocationText + "</span></div>";
		item.cardlocation = lbsStr;
	}
	var cardStr = weiboTemplate.temp(item);
	$(".mui-scroll ul").append(cardStr);

}

//删除点击事件
mui('.mui-scroll').on('tap', '.mui-icon-trash', function() {
	var weiboid = this.getAttribute('weiboid');
	mui.confirm("是否要删除微博？", "我的微博", ["是", "否"], function(event, index) {
		console.log(JSON.stringify(event));
		if(event.index == 0) {
			var url = 'http://api.myautos.cn/api/weibo/WeiboDelete';
			var dataPara = {
				id: weiboid
			}
			commonUtil.sendRequestWithToken(url, dataPara, true, function(data) {
				console.log(JSON.stringify(data));
				if(data.IsSuccess == 1) {
					//plus.webview.currentWebview().reload();
					$("#card_" + weiboid).remove();
				} else {
					mui.toast("删除失败");
				}
			});
		}
	});
});
//评论  
mui('.mui-scroll').on('tap', '.mui-icon-compose', function() {
	var weiboid = this.getAttribute("weiboid");
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