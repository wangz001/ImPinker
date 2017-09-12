$(document).ready(function() {
	bindVote();
	//评论  
	mui('.mui-scroll').on('tap', '.mui-icon-compose', function() {
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
});

function bindVote() {
	//点赞
	var footers = $(".mui-card-footer");
	for(var i = 0; i < footers.length; i++) {
		var footer = footers[i];
		var span = $(footer).find("span:eq(0)");
		$(span).unbind("click"); //加载多页的时候防止重复绑定
		$(span).bind("click", function() {
			var heartType = $(this).attr("class");
			var count = $(this).text();
			var weiboid = this.getAttribute("weiboid");
			console.log(count);
			if(heartType.indexOf("mui-icon-extra-heart-filled") != -1) {
//				$(this).removeClass("mui-icon-extra-heart-filled");
//				$(this).addClass("mui-icon-extra-heart");
//				if(count > 0) {
//					$(this).find("em").html(parseInt(count) - 1);
//				}
				mui.toast("您已赞过此微博");
			} else {
				$(this).removeClass("mui-icon-extra-heart");
				$(this).addClass("mui-icon-extra-heart-filled");
				$(this).find("em").html(parseInt(count) + 1);
				mui.toast("点赞成功！");
				
				sendVote(weiboid, false)
			}
		});
	}
}

function sendVote(weiboid, isVote) {
	var url = "http://api.myautos.cn/api/weibovote/newweibovote";
	var data = {
		"weiboid": weiboid
	}
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		if(data.IsSuccess == 1 ) {
			storageUtil.setWeiboVote(weiboid);
			console.log("aa");
		} else {
			console.log("bb");
		}
	});
}