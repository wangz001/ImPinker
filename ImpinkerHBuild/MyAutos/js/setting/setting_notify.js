mui.init();

mui.plusReady(function() {
	//阻尼系数
	var deceleration = mui.os.ios ? 0.003 : 0.0009;
	mui('.mui-scroll-wrapper').scroll({
		bounce: false,
		indicators: true, //是否显示滚动条
		deceleration: deceleration
	});
	//默认加载第一页
	getNotify(function(data) {});
	getAllNotify(function(data) {});

	mui("#item1mobile .mui-scroll").pullToRefresh({
		up: {
			callback: function() {
				var self = this;
				getNotify(function(data) {
					self.endPullUpToRefresh(true);
				});
			}
		}
	});
	mui("#item2mobile .mui-scroll").pullToRefresh({
		up: {
			callback: function() {
				var self = this;
				getAllNotify(function(data) {
					if(data.IsSuccess == 1 && data.Data != null) {
						self.endPullUpToRefresh();
					} else {
						self.endPullUpToRefresh(true);
					}
				});
			}
		}
	});
});

///获取通知。 isnew=1时，表示获取新通知；isnew=0表示获取全部
function getNotify(callback) {
	callback = callback || $.noop;
	var url = commonConfig.apiRoot + '/api/Notify/GetNewNotifyList';
	var data = {
		isread: 1
	};
	commonUtil.sendRequestWithToken(url, data, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			var list = data.Data;
			var template_notifyitem = $('script[id="notifyitem-noread"]').html();
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				var tempStr = template_notifyitem.temp(item);
				$("#mui-table-view-1").append(tempStr);
			}
		}
		return callback(data);
	});
}

///获取所有通知。分页
var allNotifyPage = 1;
var allNotifyPageSize = 15;

function getAllNotify(callback) {
	callback = callback || $.noop;
	var url = commonConfig.apiRoot + '/api/Notify/GetAllNotifyList';
	var data = {
		page: allNotifyPage,
		pagesize: allNotifyPageSize
	};
	commonUtil.sendRequestWithToken(url, data, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null) {
			var list = data.Data;
			var template_notifyitem = $('script[id="notifyitem"]').html();
			var template_notifyitem_noread = $('script[id="notifyitem-noread"]').html();
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				var tempStr = "";
				if(item.IsRead == 1) {
					tempStr = template_notifyitem.temp(item);
				} else {
					tempStr = template_notifyitem_noread.temp(item);
				}
				$("#mui-table-view-2").append(tempStr);
			}
			allNotifyPage++;
		} else {
			mui.toast("没有更多数据");
		}
		return callback(data);
	});
}

mui('.mui-content .mui-control-content').on('tap', '.info-rows-2', function() {
	var targettype = this.getAttribute('targettype');
	var targetid = this.getAttribute('targetid');
	var notifyid = this.getAttribute('notifyid');
	console.log(targettype);
	//标记为已读
	markRead(notifyid);
	//$(this).remove();
	if(targettype == 1) {
		mui.openWindow({
			id: targetid,
			url: "../article/preview.html",
			show: {
				aniShow: "pop-in"
			},
			waiting: {
				autoShow: false
			},
			extras: {
				articleid: targetid, //扩展参数
				articlename: ""
			}
		});
	}
	if(targettype == 2) {
		mui.openWindow({
			id: targetid,
			url: "../weibo/weibo_preview.html",
			show: {
				aniShow: "pop-in"
			},
			waiting: {
				autoShow: false
			},
			extras: {
				weiboid: targetid //扩展参数
			}
		});
	}
});
//标记为已读
function markRead(targetid) {
	var url = commonConfig.apiRoot + '/api/Notify/UpdateNotifyState';
	var data = {
		NotifyId: targetid,
		IsRead: 1
	};
	commonUtil.sendRequestWithToken(url, data, true, function(data) {
		console.log(JSON.stringify(data));
	});
}