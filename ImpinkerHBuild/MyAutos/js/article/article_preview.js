var articleItem = null;
var shares = null,
	sharewx = null,
	shareweibo = null;
mui.plusReady(function() {
	//获取参数
	var self = plus.webview.currentWebview();
	var articleid = self.articleid;
	var articlename = self.articlename;
	$("#articlename").html(articlename);
	//判断是否赞过文章
	var isVote = storageUtil.getArticleVote(articleid);
	if(isVote) {
		$('#vote').removeClass("mui-icon-extra-heart");
		$('#vote').addClass("mui-icon-extra-heart-filled");
	}
	//判断是否收藏过文章
	var isVote = storageUtil.getArticleCollect(articleid);
	if(isVote) {
		$('#collect').removeClass("mui-icon-star");
		//$('#collect').addClass("mui-icon-star-filled");
	}
	getArticle(articleid);
	getArticleComment(articleid);
	// 扩展API加载完毕，现在可以正常调用扩展API
	plus.share.getServices(function(s) {
		shares = s;
		for(var i in s) {
			if('weixin' == s[i].id) {
				sharewx = s[i];
			}
			if('sinaweibo' == s[i].id) {
				shareweibo = s[i];
			}
		}
	}, function(e) {
		alert("获取分享服务列表失败：" + e.message);
	});

});
mui.previewImage();

$(function() {
	$('.emotion').qqFace({
		assign: 'comment_text',
		path: '../../js/qqFace/arclist/' //表情存放的路径
	});
});

//查看结果

function replace_em(str) {
	//str = str.replace(/\</g, '&lt;');
	//str = str.replace(/\>/g, '&gt;');
	//str = str.replace(/\n/g, '<br/>');
	str = str.replace(/\[em_([0-9]*)\]/g, '<img src="../../js/qqFace/arclist/$1.gif" border="0" />');
	return str;
}

document.getElementById('share').addEventListener('tap', function() {
	var btnArray = [{
		title: '分享给微信好友'
	}, {
		title: '分享到微信朋友圈'
	}, {
		title: '分享到新浪微博'
	}];
	plus.nativeUI.actionSheet({
		title: "分享",
		cancel: '取消',
		buttons: btnArray
	}, function(e) {
		var index = e.index; // 
		switch(index) {
			case 1:
				sharewx.send({
					title: articleItem.ArticleName,
					content: articleItem.Description,
					href: "http://m.myautos.cn/Article/Index?id=" + articleItem.Id,
					thumbs: [
						articleItem.CoverImage.replace("articlecover_36_24", "articlecover_100")
					],
					extra: {
						scene: "WXSceneSession"
					}
				}, function() {
					mui.toast("分享给好友成功");
				}, function(e) {
					mui.toast("取消分享");
				});
				break;
			case 2:
				sharewx.send({
					title: articleItem.ArticleName,
					content: articleItem.Description,
					href: "http://m.myautos.cn/Article/Index?id=" + articleItem.Id,
					thumbs: [
						articleItem.CoverImage.replace("articlecover_36_24", "articlecover_100")
					],
					extra: {
						scene: "WXSceneTimeline"
					}
				}, function() {
					mui.toast("分享成功");
				}, function(e) {
					mui.toast("取消分享");
				});
				break;
			case 3:
				shareweibo.send({
					title: articleItem.ArticleName,
					content: articleItem.Description,
					href: "http://m.myautos.cn/Article/Index?id=" + articleItem.Id,
					thumbs: [
						articleItem.CoverImage.replace("articlecover_36_24", "articlecover_100")
					]
				}, function() {
					mui.toast("分享成功");
				}, function(e) {
					mui.toast("取消分享");
				});
				break;
		}
	});
});

//评论
$('#compose').bind('click', function() {
	if(commonUtil.checkToken()) {
		showComment();
	}
});

function showComment() {
	location.hash = "#sendComposs";
	$('#comment_text').focus();
	$('#mui-footer').hide();
	//-----------------

}

function hideComment() {
	var txt = $("#comment_text").val();
	$('#mui-footer').show();
	if(txt == null || txt.length == 0) {
		//防止该事件和点击发送事件冲突
		//$(".mui-bar-footer a").show();
		//$('#sendComposs').hide();
		//$('#composeText').hide();
		setTimeout(function() {
			//防止和提交按钮冲突
			toCommentId = 0;
			//$("#comment_text").val("");
			$("#comment_text").attr("placeholder", "请输入评论内容....");
		}, 200);
	}

}

$('#comment_text').focus(function() {
	$('#mui-footer').hide();
});
$('#comment_text').bind('focusout', function() {
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
			//触发自定义事件。评论数+1
			var articlePage = plus.webview.getWebviewById('tab-webview-subpage-article.html');
			mui.fire(articlePage, 'articleComment', {
				articleid: articleItem.Id
			});
		} else {
			console.log("评论失败。" + data.Description);
		}
	});
}

var toCommentId = 0;
//评论 回复  comment_to
mui('#comment ').on('tap', '.comment_to', function() {
	var Id = this.getAttribute('commentid');
	var username = this.getAttribute('username');
	$("#comment_text").val('');
	$("#comment_text").attr("placeholder", "回复:" + username);
	toCommentId = Id;
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
			$("#coverimage").attr("src", articleItem.CoverImage.replace("style/articlecover_36_24", "style/article_900"));
			$("#article_description").html("简介：" + articleItem.Description);
			articleinfo.Content = replace_em(articleinfo.Content);
			var contentStr = articleinfo.Content;
			//替换成图片懒加载的地址
			var reg = new RegExp("img src=","g");//g,表示全部替换。
			contentStr=contentStr.replace(reg,"img data-lazyload=");
			//console.log("aaa:"+contentStr);
			$(".head-title").html(articleinfo.ArticleName);
			$("#articlecontent").html(contentStr);
			//console.log(articleItem.UserHeadUrl);
			$("#user_headimg").attr('src', articleItem.UserHeadUrl);
			$("#user_name").html(articleItem.UserName);
			$("#article-createtime").html(articleItem.CreateTime.substring(0, 11));
			setTimeout(function() {
				$(".zhezhaoDiv").hide();
				$("#mui-progressbar").hide();
			}, 500)
			//图片懒加载
			mui(document).imageLazyload({
				placeholder: '../../images/60x60.gif'
			});
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
		//console.log(JSON.stringify(data));
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
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
		var toItem = item.ListToComment[0];
		toItem.Content = replace_em(toItem.Content);
		var htmlTo = toTemplate.temp(toItem);
		item.tocomment = htmlTo;
	}
	//显示qqFace
	item.Content = replace_em(item.Content);
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
			//触发自定义事件。评论数+1
			var articlePage = plus.webview.getWebviewById('tab-webview-subpage-article.html');
			mui.fire(articlePage, 'articleVote', {
				articleid: articleItem.Id
			});

			storageUtil.setArticleVote(articleItem.Id);
		} else {
			mui.toast("谢谢支持~~");
			//storageUtil.setArticleVote(articleItem.Id);
		}
		$('#vote').removeClass("mui-icon-extra-heart");
		$('#vote').addClass("mui-icon-extra-heart-filled");
	});
});

//收藏点击事件
$('#collect').bind('click', function() {
	var heartType = $(this).attr("class");
	//mui.alert("收藏到我的微博");
	var url = "http://api.myautos.cn/api/UserCollection/AddArticleCollect";
	var para = {
		"articleId": articleItem.Id
	}
	if(heartType.indexOf("mui-icon-star-filled") != -1) {
		mui.toast("您已收藏过");
	} else {
		//$(this).attr("class", "mui-icon mui-icon-star-filled");
		commonUtil.sendRequestWithToken(url, para, false, function(data) {
			console.log(JSON.stringify(data));
			if(data.IsSuccess == 1) {
				mui.toast("收藏成功");
				//$(this).attr("class", "mui-icon mui-icon-star-filled");
				storageUtil.setArticleCollect(articleItem.Id);
			} else {
				//mui.toast("收藏失败");
			}
		});
	}
});