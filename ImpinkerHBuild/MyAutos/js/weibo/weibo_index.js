$(document).ready(function() {
	bindVote();
	//评论  
	mui('.mui-scroll').on('tap', '.mui-icon-compose', function() {
		mui.toast("评论");
		mui.openWindow({
				url: "view/weibo/weibo_comment.html",
				id: "weibo_comment",
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
			if(heartType.indexOf("mui-icon-extra-heart-filled") != -1) {
				$(this).removeClass("mui-icon-extra-heart-filled");
				$(this).addClass("mui-icon-extra-heart");
				mui.toast("取消点赞");
			} else {
				$(this).removeClass("mui-icon-extra-heart");
				$(this).addClass("mui-icon-extra-heart-filled");
				mui.toast("点赞成功！");
			}
		});
	}
}