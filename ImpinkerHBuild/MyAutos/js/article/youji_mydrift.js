mui.plusReady(function() {
	mui.init({
		swipeBack: false,
		pullRefresh: {
			container: '#pullrefresh',
			up: {
				auto: true, //可选,默认false.自动上拉加载一次
				contentrefresh: '正在加载...',
				callback: pullupRefresh
			}
		}

	});
});
//添加列表项的点击事件  
mui('.mui-scroll').on('tap', '.draftitem', function(e) {
	var articleid = $(this).attr("articleid");
	var selectedItem = {};
	for(var i = 0; i < allDraftList.length; i++) {
		var item = allDraftList[i];
		if(item.Id.toString() == articleid) {
			selectedItem = item;
			break;
		}
	}
	mui.openWindow({
		id: 'youji_new',
		url: 'youji_new.html',
		show: {
			aniShow: "pop-in"
		},
		extras: {
			articleid: articleid,
			articlename: selectedItem.ArticleName,
			coverimage: selectedItem.CoverImage,
			description: selectedItem.Description,
			content: selectedItem.Content
		}
	});
});
(function(mm) {
	var btnArray = ['确认', '取消'];
	$('#draftlist').on('tap', '.mui-btn', function(event) {
		var elem = this;
		var li = elem.parentNode.parentNode;
		mui.confirm('确认删除该条记录？', 'Hello MUI', btnArray, function(e) {
			if(e.index == 0) {
				var articleId = $(elem).attr("articleid");
				console.log(articleId);
				deleteArticle(articleId, function(err) {
					console.log(err);
				})
				li.parentNode.removeChild(li);
			} else {
				setTimeout(function() {
					mm.swipeoutClose(li);
				}, 0);
			}
		});
	});
})(mui);
//删除草稿
var deleteArticle = function(articleid, callback) {
	callback = callback || $.noop;
	var url = commonConfig.apiRoot + '/api/article/DeleteArticle';
	var data = {
		Id: articleid
	}
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		console.log(JSON.stringify(data));
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			return callback();
		} else {
			return callback(data.Description);
		}
	});
}

var allDraftList = new Array();
/**
 * 上拉加载具体业务实现
 */
function pullupRefresh() {
	var url = commonConfig.apiRoot + '/api/article/GetMyDraft';
	commonUtil.sendRequestWithToken(url, {}, true, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			allDraftList = list;
			var ul = $('#draftlist');
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				var imgTemplate = $('script[id="draftlistitem"]').html();
				var imgHtmlStr = imgTemplate.temp(item);
				ul.append(imgHtmlStr);
			}
		} else {
			mui('#pullrefresh').pullRefresh().endPullupToRefresh(true);
			return;
		}
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}