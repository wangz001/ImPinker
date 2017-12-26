$(document).ready(function() {
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
		var count = $(this).find("em").text();
		var weiboid = this.getAttribute("weiboid");
		if(heartType.indexOf("zan1") != -1) {
			mui.toast("您已赞过此微博");
		} else {
			var zan1Str="<use xlink:href=\"#icon-zan1\"></use>";
			$(this).find("svg").html(zan1Str);
			$(this).find("em").html(parseInt(count) + 1);
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
			var zan1Str="<use xlink:href=\"#icon-shoucang\"></use>";
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
});

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