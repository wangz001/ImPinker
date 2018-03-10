mui.init({
	swipeBack: true
});
var cweiboid = 0;
var toCommentId = 0; //会否某人的评论
mui.plusReady(function() {
	var self = plus.webview.currentWebview();
	var weiboid = self.weiboid;
	cweiboid = weiboid;
	getWeibo(weiboid);
	getCommentLists(weiboid);
	mui.previewImage();
});

function getWeibo(weiboid) {
	var url = commonConfig.apiRoot+'/api/WeiBo/GetWeiBoById';
	var data = {
		weiboid: weiboid
	};
	commonUtil.sendRequestGet(url, data, function(data) {
		//console.log(JSON.stringify(data))
		if(data.IsSuccess == 1 && data.Data != null) {
			var weiboTemplate = $('script[id="card-weibo"]').html();
			var weibo = data.Data;
			var imgHtmlStr = "";
			var imgs = weibo.ContentValue.split(',');
			if(imgs.length > 1) {
				//多图
				for(var j = 0; j < imgs.length; j++) {
					imgHtmlStr += '<a href="#"><img src="' + imgs[j].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_200_200) + '" data-preview-src="' + imgs[j].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_1200) + '" data-preview-group="' + weibo.Id + '"></a>';
				}
			} else {
				imgHtmlStr = '<a href="#"><img class="bigimage" src="' + imgs[0].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_60_34) + '" class="bigimage" data-preview-src="' + imgs[0].replace(commonConfig.imgStyle.weibo_24_16, commonConfig.imgStyle.weibo_1200) + '" data-preview-group="' + weibo.Id + '"></a>';
			}
			weibo.imglist = imgHtmlStr;
			//qq表情
			weibo.Description = replace_em(weibo.Description);
			//显示地理位置
			weibo.cardlocation = "";
			if(weibo.LocationText != "" && weibo.Longitude != "" && weibo.Lantitude != "") {
				var lbsStr = "<div weiboid='" + weibo.Id + "' class='card-location'><span class='mui-icon mui-icon-location'></span><span>" + weibo.LocationText + "</span></div>";
				weibo.cardlocation = lbsStr;
			}
			var cardStr = weiboTemplate.temp(weibo);
			$("#content").append(cardStr);
		}
	});
}

function getCommentLists(weiboId) {
	var url = commonConfig.apiRoot+'/api/WeiBoVote/GetWeiboCommentList';
	//console.log("11--" + weiboId)
	var data = {
		weiboid: weiboId,
		pagesize: 10,
		page: 1
	};
	commonUtil.sendRequestGet(url, data, function(data) {
		//console.log(JSON.stringify(data))
		if(data.IsSuccess == 1 && data.Data != null) {
			$("#comment").html("");
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initComment(item);
			}
		} else {
			$("#comment").append("<p>暂无更多数据</p>");
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

//提交评论
document.getElementById('comment_submit').addEventListener('tap', function() {
	console.log(cweiboid);
	var textStr = $("#comment_text").val();
	var url = commonConfig.apiRoot+'/api/weibovote/NewWeiBoComment';
	var data = {
		WeiBoId: cweiboid,
		CommentStr: textStr,
		ToCommentId: toCommentId
	}
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		//console.log(JSON.stringify(data));
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

//评论 回复  comment_to
mui('#comment ').on('tap', '.comment_to', function() {
	var Id = this.getAttribute('commentid');
	toCommentId = Id;
	console.log(toCommentId);
	$('#comment_text').focus();
});