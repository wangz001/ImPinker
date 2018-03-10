mui.plusReady(function() {
	mui.init({
		swipeBack: true,
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
var previewPage = null;
//添加列表项的点击事件  
mui('.mui-scroll').on('tap', '.article_card_href', function(e) {
	var articleid = $(this).attr("articleid");
	var articlename = this.getAttribute('articlename');
	console.log(articleid);
	var titleNView =commonConfig.titleNView;
	titleNView.titleText=articlename;
	var webview_style = {
		"render": "always",
		"popGesture": "hide",
		"bounce": "vertical",
		"bounceBackground": "#efeff4",
		"titleNView": titleNView
	};
	//获得详情页面  
	mui.openWindow({
		id: "preview.html",
		url: "preview.html",
		styles: webview_style,
		show: {
			aniShow: "pop-in"
		},
		waiting: {
			autoShow: false
		},
		createNew:true,
		extras: {
			articleid: articleid, //扩展参数
			articlename: ''
		}
	});
});
//删除文章
mui('.mui-scroll').on('tap', '.delete_article', function(e) {
	var articleid = $(this).attr("articleid");
	if(articleid > 0) {
		mui.confirm("是否要删除文章？", "我的文章", ["是", "否"], function(event, index) {
			if(event.index == 0) {
				var url = commonConfig.apiRoot+'/api/article/DeleteArticle';
				var data = {
					Id: articleid
				}
				commonUtil.sendRequestWithToken(url, data, true, function(data) {
					if(data.IsSuccess == 1) {
						$("#article_" + articleid).remove();
					} else {
						mui.toast("删除失败");
					}
				});
			}
		});
	}
});

var pageNum = 1;
var pageSize = 30;
var lastTime = "";

/**
 * 上拉加载具体业务实现
 */
function pullupRefresh() {
	var url = commonConfig.apiRoot+'/api/article/GetMyArticle';
	var data = {
		pageNum: pageNum,
		pageSize: pageSize
	};
	commonUtil.sendRequestWithToken(url, data, false, function(data) {
		var articleItemTemplate = $('script[id="articlelistitem"]').html();
		var datetemeTemplate = $('script[id="articlelist_date"]').html();
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				//添加日期栏
				var datetime = new Date(item.CreateTime);
				var dateStr = datetime.getFullYear() + '年' + (datetime.getMonth() + 1) + '月'; //+ '-' + datetime.getDate();
				if(lastTime != dateStr) {
					lastTime = dateStr;
					var obj = {
						"datetime": dateStr
					};
					var htmlStr = datetemeTemplate.temp(obj);
					$("#articlelist").append(htmlStr);
				}
				//添加日期栏结束
				item.CoverImage = item.CoverImage.replace(commonConfig.imgStyle.articlecover_36_24, commonConfig.imgStyle.article_24_20);

				var imgHtmlStr = articleItemTemplate.temp(item);
				$("#articlelist").append(imgHtmlStr);
			}
			pageNum++;
		} else {
			mui('#pullrefresh').pullRefresh().endPullupToRefresh(true);
			return;
		}
		mui('#pullrefresh').pullRefresh().endPullupToRefresh(); //参数为true代表没有更多数据了。
	});
}