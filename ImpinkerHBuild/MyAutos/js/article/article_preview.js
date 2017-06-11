//评论
document.getElementById('composeText').addEventListener('focus', function() {
	$(".mui-bar-footer a").hide();
	$('#sendComposs').show();
});
document.getElementById('composeText').addEventListener('focusout', function() {
	var txt = $("#composeText").val();
	if(txt == null || txt.length == 0) {
		//防止该事件和点击发送事件冲突
		$(".mui-bar-footer a").show();
		$('#sendComposs').hide();
	}
});
$(document).ready(function() {
	$("#sendComposs").bind('click', function() {
		var txt = $("#composeText").val();
		$("#composeText").val('');
		$(".mui-bar-footer a").show();
		$('#sendComposs').hide();
		if(txt != null && txt.length > 0) {
			SendComposs(txt);
		}
	});
});

function SendComposs(txtStr) {
	var userstate = app.getState();
	var url = "http://api.myautos.cn/api/ArticleVote/NewArticleComment";
	mui.ajax(url, {
		data: {
			ArticleId: articleItem.Id,
			CommentStr: txtStr
		},
		dataType: 'json', //服务器返回json格式数据
		type: 'post', //HTTP请求类型
		timeout: 10000, //超时时间设置为10秒；
		headers: {
			'Content-Type': 'application/json',
			'username': userstate.account,
			'usertoken': userstate.token
		},
		success: function(data) {
			if(data.IsSuccess == 1) {
				mui.toast("评论成功！");
			} else {
				console.log("评论失败。" + data.Description);
			}
		},
		error: function(xhr, type, errorThrown) {
			//异常处理；
			console.log(type);
		}
	});
}