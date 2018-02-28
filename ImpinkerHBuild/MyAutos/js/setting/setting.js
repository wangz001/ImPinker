mui.init({
	swipeBack: true //启用右滑关闭功能
});
mui.plusReady(function() {
	setTimeout(function() {
		//避免更新token的时候token过期
		getNotifyCunt();
	}, 10 * 1000);
	//设置用户头像
	var user = app.getState();
	//console.log(JSON.stringify(user));
	if(user != null && JSON.stringify(user) != '{}') {
		var showName = user.showname == "" ? user.account : user.showname;
		var nameStr = showName; //+ '<p class=\'mui-ellipsis\'>车酷号:' + user.account + '</p>'
		$("#username").html(nameStr);
		if(user.imgurl != "") {
			$("#head-img").attr("src", user.imgurl);
		} else {
			defaultImg();
		}
		$("#userinfo").show();
		$("#btn_login").hide();
	} else {
		defaultImg();
	}
	document.getElementById("head-img").addEventListener('tap', function(e) {
				e.stopPropagation();
	});
	
});

function defaultImg() {
	if(mui.os.plus) {
		plus.io.resolveLocalFileSystemURL("_doc/head.jpg", function(entry) {
			var s = entry.fullPath + "?version=" + new Date().getTime();;
			document.getElementById("head-img").src = s;
		}, function(e) {
			document.getElementById("head-img").src = 'images/head-default.png';
		})
	} else {
		document.getElementById("head-img").src = 'images/head-default.png';
	}
}

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

//跳转到登录  
document.getElementById("btn_login").addEventListener('tap', function() {
	console.log("aaaa");
	mui.openWindow({
		url: "login.html",
		id: "login",
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});

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