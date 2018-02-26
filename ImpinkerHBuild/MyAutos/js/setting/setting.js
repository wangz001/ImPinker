mui.init({
	swipeBack: true //启用右滑关闭功能
});
mui.plusReady(function() {
	setTimeout(function() {
		//避免更新token的时候token过期
		getNotifyCunt();
	}, 10 * 1000);
});

function getNotifyCunt() {
	var url = "http://api.myautos.cn/api/Notify/GetNewNotifyCount";
	var data = {};
	var state = app.getState(); //用户信息
	if(state.account && state.password) {
		commonUtil.sendRequestWithToken(url, data, false, function(data) {
			if(data.IsSuccess == 1 && data.Data != null) {
				$("#notifycount").html(data.Data);
				$("#notifycount").show();
			}
		});
	}
}
document.getElementById("setting").addEventListener('tap', function() {
	mui.openWindow({
		url: "view/setting/setting.html",
		id: "setting",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
document.getElementById("notify").addEventListener('tap', function() {
	//获得主页面的webview
	//var main = plus.webview.currentWebview().parent();
	//触发主页面的gohome事件
	//mui.fire(main, 'gohome');
	mui.openWindow({
		url: "view/setting/setting-notify.html",
		id: "setting-notify",

		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
document.getElementById("my_youji").addEventListener('tap', function() {
	//获得主页面的webview
	//var main = plus.webview.currentWebview().parent();
	//触发主页面的gohome事件
	//mui.fire(main, 'gohome');
	mui.openWindow({
		url: "view/article/youji_my.html",
		id: "youji_my",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
document.getElementById("my_weibo").addEventListener('tap', function() {
	mui.openWindow({
		url: "view/weibo/weibo_my.html",
		id: "weibo_my",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
//收藏
document.getElementById("my_collect").addEventListener('tap', function() {
	mui.openWindow({
		url: "view/user/usercollect.html",
		id: "my_collect",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
document.getElementById("about").addEventListener('tap', function() {
	mui.openWindow({
		url: "view/about.html",
		id: "about",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
//
document.getElementById("feedback").addEventListener('tap', function() {
	mui.openWindow({
		url: "view/setting/feedback.html",
		id: "feedback",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
//
document.getElementById("testindex").addEventListener('tap', function() {
	mui.openWindow({
		url: "view/weibo/index.html",
		id: "test",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});