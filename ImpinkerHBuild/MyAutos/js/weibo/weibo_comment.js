mui.init();
var cweiboid = 0;
var toCommentId = 0; //会否某人的评论
mui.plusReady(function() {
	//获取参数
	var self = plus.webview.currentWebview();
	var weiboid = self.weiboid;
	cweiboid = weiboid;
	getCommentLists(weiboid);

});

$(function() {
	$('.emotion').qqFace({
		assign: 'comment_text',
		path: '../../js/qqFace/arclist/' //表情存放的路径
	});
});

//查看结果

function replace_em(str) {
	str = str.replace(/\</g, '&lt;');
	str = str.replace(/\>/g, '&gt;');
	str = str.replace(/\n/g, '<br/>');
	str = str.replace(/\[em_([0-9]*)\]/g, '<img src="../../js/qqFace/arclist/$1.gif" border="0" />');
	return str;
}

document.getElementById('comment_submit').addEventListener('tap', function() {
	console.log(cweiboid);
	var textStr = $("#comment_text").val();
	var url = commonConfig.apiRoot + '/api/weibovote/NewWeiBoComment';
	var data = {
		WeiBoId: cweiboid,
		CommentStr: textStr,
		ToCommentId: toCommentId
	}
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			mui.toast("评论成功");
			$("#comment_text").val("");
			getCommentLists(cweiboid);
			//触发自定义事件。评论数+1
			var articlePage = plus.webview.getWebviewById('tab-webview-subpage-weibo.html');
			mui.fire(articlePage, 'weiboComment', {
				weiboid: cweiboid
			});
		} else {
			mui.toast("评论失败");
		}
	});
});

function getCommentLists(weiboId) {
	var url = commonConfig.apiRoot + '/api/WeiBoVote/GetWeiboCommentList';
	var data = {
		weiboid: weiboId,
		pagesize: 10,
		page: 1
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
	var toTemplate = $('script[id="comment_item_to"]').html();
	if(item.ToCommentId > 0 && item.ToCommentItemVm != null) {
		item.ToCommentItemVm.ContentText = replace_em(item.ToCommentItemVm.ContentText);
		var htmlTo = toTemplate.temp(item.ToCommentItemVm);
		item.tocomment = htmlTo;
	}
	//显示qqFace
	item.ContentText = replace_em(item.ContentText);
	var articleHtmlStr = template.temp(item);
	$("#comment").append(articleHtmlStr);
}

//评论 回复  comment_to
mui('#comment ').on('tap', '.comment_to', function() {
	var Id = this.getAttribute('commentid');
	toCommentId = Id;
	console.log(toCommentId);
	$('#comment_text').focus();
});