//评论
$('.mui-icon-compose').bind('click', function() {
	showComment();
});

function showComment() {
	location.hash = "#sendComposs";
	//$(".mui-bar-footer a").hide();
	//$('#composeText').show();
	$('#comment_text').focus();
	//$('#sendComposs').show();
	//$('#mui-footer').hide();
	
}

function hideComment() {
	var txt = $("#comment_text").val();
	//$('#mui-footer').show();
	if(txt == null || txt.length == 0) {
		//防止该事件和点击发送事件冲突
		//$(".mui-bar-footer a").show();
		//$('#sendComposs').hide();
		//$('#composeText').hide();
	}
	setTimeout(function() {
		//防止和提交按钮冲突
		toCommentId = 0;
	}, 200);
}
$('#composeText').bind('focusout', function() {
	hideComment();
});

$(document).ready(function() {
	$("#sendComposs").bind('click', function() {
		var txt = $("#comment_text").val();
		$("#comment_text").val('');
		hideComment();
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
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		if(data.IsSuccess == 1) {
			getArticleComment(articleItem.Id);
			mui.toast("评论成功！");
			$("html,body").animate({
				scrollTop: $("#comment").offset().top
			}, 100);
		} else {
			console.log("评论失败。" + data.Description);
		}
	});
}

var toCommentId = 0;
//评论 回复  comment_to
mui('#comment ').on('tap', '.comment_to', function() {
	var Id = this.getAttribute('commentid');
	toCommentId = Id;
	console.log(toCommentId);
	showComment();
});

function getArticle(articleid) {
	var url = 'http://api.myautos.cn/api/article/GetArticleWithContent';
	var data = {
		articleid: articleid,
	};
	//plus.nativeUI.showWaiting('正在加载...');
	commonUtil.sendRequestGet(url, data, function(data) {
		//plus.nativeUI.closeWaiting();
		if(data.IsSuccess == 1 && data.Data != null) {
			var articleinfo = data.Data;
			articleItem = articleinfo;
			var contentStr = articleinfo.Content;
			$("#articlename").html(articleinfo.ArticleName);
			$("#articlecontent").html(contentStr);
			//console.log(articleItem.UserHeadUrl);
			$("#user_headimg").attr('src', articleItem.UserHeadUrl);
			$("#user_name").html(articleItem.UserName);
			$(".thread-info .publish-time").html(articleItem.CreateTime);
			setTimeout(function() {
				$(".zhezhaoDiv").hide();
				$("#mui-progressbar").hide();
			}, 500)

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
		console.log(JSON.stringify(data));
		if(data.IsSuccess == 1 && data.Data != null&& data.Data.length>0) {
			$("#comment").html("");
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				//console.log(JSON.stringify(item));
				initComment(item);
			}
		}
	});
}

function initComment(item) {
	var template = $('script[id="comment_item"]').html();
	var toTemplate = $('script[id="comment_item_to"]').html();
	if(item.ToCommentId > 0 && item.ListToComment.length > 0) {
		var htmlTo = toTemplate.temp(item.ListToComment[0]);
		item.tocomment = htmlTo;
		//articleHtmlStr = articleHtmlStr.replace('</dl>', htmlTo + '</dl>'); // 暂时处理方法
	}
	var articleHtmlStr = template.temp(item);
	$("#comment").append(articleHtmlStr);
}

///点赞
$('#vote').bind('click', function() {
	var heartType = $(this).attr("class");
	if(heartType.indexOf("mui-icon-extra-heart-filled") != -1) {
		mui.toast("您已赞过此文章~");
		return;
	}
	var url = "http://api.myautos.cn/api/ArticleVote/NewArticleVote";
	var data = {
		articleid: articleItem.Id
	}
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		if(data.IsSuccess == 1) {
			mui.toast("谢谢支持~");
			storageUtil.setArticleVote(articleItem.Id);
		}else{
			mui.toast("谢谢支持~");
			storageUtil.setArticleVote(articleItem.Id);
		}
		$('#vote').removeClass("mui-icon-extra-heart");
		$('#vote').addClass("mui-icon-extra-heart-filled");
	});
});