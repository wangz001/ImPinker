
$(document).ready(function(){
	//点赞
	$(".mui-card-footer span:first").bind("click",function(){
		var heartType=$(this).attr("class");
		if(heartType.indexOf("mui-icon-extra-heart-filled")!=-1){
			$(this).removeClass("mui-icon-extra-heart-filled");
			$(this).addClass("mui-icon-extra-heart");
			mui.toast("取消点赞");
		}else{
			$(this).removeClass("mui-icon-extra-heart");
			$(this).addClass("mui-icon-extra-heart-filled");
			mui.toast("点赞成功！");
		}
	});
	//评论
	$(".mui-card-footer .mui-icon-compose").bind("click",function(){
		mui.toast("评论");
	});
});

