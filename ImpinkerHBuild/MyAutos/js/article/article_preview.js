//评论
$('#composeText').bind('focus', function() {
	$(".mui-bar-footer a").hide();
	$('#sendComposs').show();
});
$('#composeText').bind('focusout', function() {
	toCommentId = 0;
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
	var url = "http://api.myautos.cn/api/ArticleVote/NewArticleComment";
	var data = {
		ArticleId: articleItem.Id,
		CommentStr: txtStr,
		ToCommentId: toCommentId
	};
	console.log(toCommentId);
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		if(data.IsSuccess == 1) {
			getArticleComment(articleItem.Id);
			mui.toast("评论成功！");
		} else {
			console.log("评论失败。" + data.Description);
		}
	});
}

var toCommentId=0;
//评论 回复  comment_to
mui('#comment ').on('tap', '.comment_to', function() {
	var tocommentId = this.getAttribute('commentid');
	toCommentId = tocommentId;
	console.log(toCommentId);
	$('#composeText').focus();
});

function getArticle(articleid) {
	var url = 'http://api.myautos.cn/api/article/GetArticleWithContent';
	var data = {
		articleid: articleid,
	};
	plus.nativeUI.showWaiting('正在加载');
	commonUtil.sendRequestGet(url, data, function(data) {
		plus.nativeUI.closeWaiting();
		if(data.IsSuccess == 1 && data.Data != null) {
			var articleinfo = data.Data;
			articleItem = articleinfo;
			var contentStr = articleinfo.Content;
			$("#articlename").html(articleinfo.ArticleName);
			$("#articlecontent").html(contentStr);
			$("#user_headimg").attr('src', articleItem.UserHeadUrl);
			$("#user_name").html(articleItem.UserName);
			$(".thread-info .publish-time").html(articleItem.CreateTime);
		}
	});
}

function getArticleComment(articleid) {
	var url = 'http://api.myautos.cn/api/ArticleVote/GetArticleComments';
	var data = {
		articleid: articleid,
		pagesize: 10,
		pagenum: 1
	};
	commonUtil.sendRequestGet(url, data, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			$("#comment").html("");
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initComment(item);
			}
		}
	});
}

function initComment(item) {
	var template = $('script[id="comment_item"]').html();
	var articleHtmlStr = template.temp(item);
	if(item.ToCommentId > 0 && item.ListToComment.length > 0) {
		var toTemplate = $('script[id="comment_item_to"]').html();
		var htmlTo = toTemplate.temp(item.ListToComment[0]);
		articleHtmlStr = articleHtmlStr.replace('</dl>', htmlTo + '</dl>'); // 暂时处理方法
	}
	$("#comment").append(articleHtmlStr);
}