mui.init({
	swipeBack: false,
	pullRefresh: {
		container: '#pullrefresh',
		down: {
			auto: true, //可选,默认false.自动上拉加载一次
			callback: pulldownRefresh
		},
		up: {
			contentrefresh: "正在加载...", //可选，正在加载状态时，上拉加载控件上显示的标题内容
			contentnomore: '没有更多数据了', //可选，请求完毕若没有更多数据时显示的提醒内容；
			callback: pullupRefresh
		}
	}

});
//图片预览
mui.previewImage();

mui.plusReady(function() {
	//显示微博缓存
	var weiboData = storageUtil.getFirstPageWeibo();
	if(weiboData != null && weiboData.length > 0) {
		for(var i = 0; i < weiboData.length; i++) {
			initWeiBoItemTemplate(weiboData[i]);
		}
	} else {
		mui.toast("网络暂时不给力奥~");
	}
	bindClickEvent();
});

//自定义事件。接受选择微博页传回的数据
window.addEventListener('weiboComment', function(event) {
	//获得事件参数  
	var weiboid = event.detail.weiboid;
	var count = $("#commentCount_" + weiboid).text();
	$("#commentCount_" + weiboid).html(parseInt(count) + 1);
});

var pageNum = 1;
var pageSize = 20;
var weiboApiPath = 'http://api.myautos.cn/api/WeiBo/GetWeiBoList';
/**
 * 下拉刷新具体业务实现
 */
function pulldownRefresh() {
	pageNum = 1;
	var dataPara = {
		pageindex: pageNum,
		pageSize: pageSize
	}
	commonUtil.sendRequestGet(weiboApiPath, dataPara, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			mui.toast("已为您获取最新数据");
			var list = data.Data;
			//清空老数据
			document.body.querySelector('.mui-scroll ul').innerHTML = "";
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItemTemplate(item);
			}
			storageUtil.setFirstPageWeibo(list);
			pageNum++;
		} else {
			mui.toast(data.Description);
		}
		mui('#pullrefresh').pullRefresh().endPulldownToRefresh(); //参数为true代表没有更多数据了。
	});
}
var count = 0;
/**
 * 上拉加载具体业务实现
 */
function pullupRefresh() {
	var dataPara = {
		pageindex: pageNum,
		pageSize: pageSize
	};
	commonUtil.sendRequestGet(weiboApiPath, dataPara, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItemTemplate(item);
			}
			pageNum++;
		} else {
			mui.toast(data.Description);
		}
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}

/**
 * 添加数据
 * @param {Object} table
 * @param {Object} item
 */
function initWeiBoItemTemplate(item) {
	var img_1200style = 'style/weibo_1200';
	var img_24style = 'style/weibo_24_16';
	var img_36style = 'style/weibo_36_24';
	var img_200style = 'style/weibo_200_200';
	if(item.ContentValue == null || item.ContentValue == '') {
		return;
	}
	var templateCard = $('script[id="mui-card-item"]').html();
	var imgHtmlStr = "";
	var imgs = item.ContentValue.split(',');
	if(imgs.length > 1) {
		//多图
		for(var i = 0; i < imgs.length; i++) {
			imgHtmlStr += '<a href="#"><img src="' + imgs[i].replace(img_24style, img_200style) + '" data-preview-src="' + imgs[i].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></a>';
		}
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
	//qq表情
	item.Description = replace_em(item.Description);
	//判断我是否赞过此微博
	var isVote = storageUtil.getWeiboVote(item.Id);
	item.isVote = isVote ? "xin" : "xihuan";
	//判断我是否收藏过此微博
	var isCollect = storageUtil.getWeiboCollect(item.Id);
	item.isCollect = isCollect ? "shoucang" : "favorite_diss";
	var cardStr = templateCard.temp(item);
	$(".mui-scroll ul").append(cardStr);
}

//查看结果
function replace_em(str) {
	str = str.replace(/\</g, '&lt;');
	str = str.replace(/\>/g, '&gt;');
	str = str.replace(/\n/g, '<br/>');
	str = str.replace(/\[em_([0-9]*)\]/g, '<img src="js/qqFace/arclist/$1.gif" border="0" />');
	return str;
}

//----------------------------------------------

function bindClickEvent() {
	//评论  
	mui('.mui-scroll').on('tap', '.icon-compose', function() {
		var weiboid = this.getAttribute("weiboid");
		mui.openWindow({
			url: "view/weibo/weibo_comment.html",
			id: "weibo_comment",
			extras: {
				weiboid: weiboid
			},
			show: {
				aniShow: 'slide-in-right',
				duration: 200
			}
		});
	});
	//点赞
	mui('.mui-scroll').on('tap', '.votebtn', function() {
		var heartType = $(this).attr("isVote");
		var count = $(this).find("span").text();
		var weiboid = this.getAttribute("weiboid");
		if(heartType.indexOf("xin") != -1) {
			mui.toast("您已赞过此微博");
		} else {
			var zan1Str = "<use xlink:href=\"#icon-xin\"></use>";
			$(this).find("svg").html(zan1Str);
			$(this).find("span").html(parseInt(count) + 1);
			$(this).attr("isVote","xin");
			mui.toast("点赞成功！");
			sendVote(weiboid, false)
		}
	});

	//收藏点击事件
	mui('.mui-scroll').on('tap', '.collectbtn', function() {
		var weiboid = this.getAttribute('weiboid');
		var heartType = $(this).attr("isCollect");
		var url = "http://api.myautos.cn/api/UserCollection/AddWeiboCollect";
		var data = {
			"weiboId": weiboid
		}
		if(heartType.indexOf("shoucang") != -1) {
			mui.toast("您已收藏");
		} else {
			var zan1Str = "<use xlink:href=\"#icon-shoucang\"></use>";
			$(this).find("svg").html(zan1Str);
			commonUtil.sendRequestWithToken(url, data, false, function(data) {
				if(data.IsSuccess == 1) {
					mui.toast("收藏成功");
					storageUtil.setWeiboCollect(weiboid);
					//$(this).attr("class", "mui-icon mui-icon-star-filled");
				} else {
					mui.toast("收藏失败");
				}
			});
		}
	});

	//地图点击事件
	mui('.mui-scroll').on('tap', '.card-location', function() {
		var weiboid = this.getAttribute('weiboid');
		mui.openWindow({
			url: "view/map/lbs.html",
			id: "lbs",
			show: {
				aniShow: 'slide-in-right',
				duration: 200
			},
			extras: {
				weiboid: weiboid
			}
		});
	});

	//用户头像点击事件
	mui('.mui-scroll').on('tap', '.head-img', function() {
		var userid = this.getAttribute('userid');
		var username = this.getAttribute('username');
		var headimg = this.getAttribute('src');
		mui.openWindow({
			url: "view/user/userindex.html",
			id: "userindex",
			show: {
				aniShow: 'slide-in-right',
				duration: 200
			},
			extras: {
				userid: userid,
				username: username,
				headimg: headimg
			}
		});
	});
}

function sendVote(weiboid, isVote) {
	var url = "http://api.myautos.cn/api/weibovote/newweibovote";
	var data = {
		"weiboid": weiboid
	}
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		if(data.IsSuccess == 1) {
			storageUtil.setWeiboVote(weiboid);
			console.log("aa");
		} else {
			console.log("bb");
		}
	});
}