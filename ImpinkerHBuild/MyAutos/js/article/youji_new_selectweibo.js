mui.init({
	swipeBack: true,
	preloadPages: [{
		url: "youji_new.html",
		id: "youji_new"
	}],
	beforeback: function() {
		//获得列表界面的webview
		var currentview = plus.webview.currentWebview();
		//触发列表界面的自定义事件（refresh）,从而进行数据刷新
		plus.webview.close(currentview);
		//返回true，继续页面关闭逻辑
		return true;
	}
});
mui.plusReady(function() {
	//阻尼系数
	var deceleration = mui.os.ios ? 0.003 : 0.0009;
	mui('.mui-scroll-wrapper').scroll({
		bounce: false,
		indicators: true, //是否显示滚动条
		deceleration: deceleration
	});
	//默认加载第一页
	getFirstPage(function(data) {});

	mui("#pullrefresh .mui-scroll").pullToRefresh({
		down: {
			callback: function() {
				var self = this;
				//alert("down");
				pulldownRefresh(function(data) {
					if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
						self.endPullDownToRefresh();
					} else {
						mui.alert("已加载完全部数据~");
						self.endPullDownToRefresh(true);
					}
				});
			}
		},
		up: {
			callback: function() {
				var self = this;
				pullupRefresh(function(data) {
					if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
						self.endPullUpToRefresh();
					} else {
						self.endPullUpToRefresh(true);
					}
				});
			}
		}
	});
});

//图片放大预览
mui.previewImage();

var selectedItems = new Array(); // 选中的对象集合
var allItems = new Array(); //全部微博对象
var youjiPage = null;
document.getElementById('submit').addEventListener('tap', function() {
	if(selectedItems.length > 0) {
		//获得详情页面  
		if(!youjiPage) {
			youjiPage = plus.webview.getWebviewById('youji_new');
		}
		//触发详情页面的weiboitems事件  
		mui.fire(youjiPage, 'weiboitems', {
			items: selectedItems
		});
		//打开详情页面            
		mui.openWindow({
			id: 'youji_new',
			url: "youji_new.html"
		});
		//关闭当前页面，防止返回的时候，又返回该页面
		plus.webview.currentWebview().close();
	} else {
		mui.confirm("您还未选择", "是否继续？", ['是', '否'], function(event) {
			console.log(JSON.stringify(event));
			if(event.index === 1) {
				return;
			} else {
				mui.openWindow({
					id: 'youji_new',
					url: "youji_new.html"
				});
				//关闭当前页面，防止返回的时候，又返回该页面
				plus.webview.currentWebview().close();
			}
		});
	}
});


// 绑定复选框点击事件
function bindCheckBoxTab() {
	$("#pullrefresh [name = checkbox1]:checkbox").unbind("click"); //防止重复绑定
	$("#pullrefresh [name = checkbox1]:checkbox").bind("click", function() {
		var weiboid = $(this).val();
		var isChecked = $(this).is(':checked');
		console.log(isChecked + "id:" + weiboid);
		if(isChecked) {
			//往selectedItems 数组添加数据，并改变编号显示
			for(var i = 0; i < allItems.length; i++) {
				var item = allItems[i];
				if(item.Id.toString() == weiboid) {
					selectedItems.push(item);
					break;
				}
			}
			$(this).next('label').html("已选择：" + (selectedItems.length));
		} else {
			//从selectedItems 数组移除数据，并改变编号显示
			selectedItems.splice(jQuery.inArray(item, selectedItems), 1);
			$(this).next('label').html("");
		}
	});
}

var pageSize = 10;
var count = 0;
var dateFirstId = 0;
var dateLastId = 0;
/**
 * 获取第一页数据
 * @param {Object} callback
 */
function getFirstPage(callback) {
	callback = callback || $.noop;
	var url = 'http://api.myautos.cn/api/weibo/GetMyWeiBoList';
	var datapara = {
		pageindex: 1,
		pageSize: pageSize
	};
	commonUtil.sendRequestWithToken(url, datapara, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			allItems = allItems.concat(list);
			for(var i = list.length - 1; i > -1; i--) {
				//返回的list 是倒叙排序，所以这么处理
				var item = list[i];
				initWeiBoItem(item, true);
			}
			bindCheckBoxTab();
		}
		return callback(data);
	});
}

/**
 * 下拉刷新
 * @param {Object} callback
 */
function pulldownRefresh(callback) {
	callback = callback || $.noop;
	//  //根据时间向前或向后取数据
	var url = 'http://api.myautos.cn/api/weibo/GetListFromIdForPage';
	var datapara = {
		startId: dateFirstId,
		pageSize: pageSize,
		isdown: 0
	};
	//console.log(JSON.stringify(datapara));
	commonUtil.sendRequestWithToken(url, datapara, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			allItems = allItems.concat(list);
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItem(item, false);
			}
			bindCheckBoxTab();
		} else {
			mui.toast("没有更多数据~");
		}
		return callback(data);
	});
}

/**
 * 上拉加载具体业务实现
 */
function pullupRefresh(callback) {
	callback = callback || $.noop;
	var url = 'http://api.myautos.cn/api/weibo/GetListFromIdForPage';
	var datapara = {
		startId: dateLastId,
		pageSize: pageSize,
		isdown: 1
	};
	//console.log(JSON.stringify(datapara));
	commonUtil.sendRequestWithToken(url, datapara, false, function(data) {
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			allItems = allItems.concat(list); 
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItem(item, true);
			}
			bindCheckBoxTab();
		}
		return callback(data);
	});
}
/**
 * 添加数据
 * @param {Object} table
 * @param {Object} item
 */
var img_1200style = 'style/weibo_1200';
var img_600style = 'style/weibo_600';
var img_24style = 'style/weibo_24_16';
var img_36style = 'style/weibo_36_24';
var weiboTemplate = $('script[id="weiboitem"]').html();

function initWeiBoItem(item, isPullUp) {
	if(item.ContentValue == null || item.ContentValue == '') {
		return;
	}
	var imgHtmlStr = "";
	var imgs = item.ContentValue.split(',');
	if(imgs.length > 1) {
		//多图
		imgHtmlStr += "<ul>";
		for(var j = 0; j < imgs.length; j++) {
			imgHtmlStr += '<li><img src="' + imgs[j] + '" data-preview-src="' + imgs[j].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '"></li>';
		}
		imgHtmlStr += "</ul>";
	} else {
		imgHtmlStr = '<img src="' + imgs[0].replace(img_24style, img_36style) + '" class="bigimage" data-preview-src="' + imgs[0].replace(img_24style, img_1200style) + '" data-preview-group="' + item.Id + '">';
	}
	item.imglist = imgHtmlStr;
	var cardStr = weiboTemplate.temp(item);
	//console.log("first:" + dateFirstId + "--last:" + dateLastId);
	if(isPullUp) {
		if(dateLastId < item.Id) {
			dateLastId = item.Id;
		}
		if(dateFirstId > item.Id || dateFirstId == 0) {
			dateFirstId = item.Id;
		}
		$("#weibo-content").append(cardStr);
	} else {
		if(dateLastId < item.Id) {
			dateLastId = item.Id;
		}
		if(dateFirstId > item.Id || dateFirstId == 0) {
			dateFirstId = item.Id;
		}
		$("#weibo-content").prepend(cardStr);
	}
}

//时间选择框

$("#btn_closePop").bind("click", function() {
	mui('#popover').popover('hide');
});
var date1, date2;
$("#btn_submitTime").bind("click", function() {
	if(date1 != null && date2 != null) {
		mui.alert(date1 + "~" + date2);
		mui('#popover').popover('hide');
		getListByRange(date1, date2);
	} else {
		mui.alert("请选择时间范围~");
	}
});

var dtpicker1;
$("#datePicker1").bind("click", function() {
	if(dtpicker1 == null) {
		dtpicker1 = new mui.DtPicker({
			type: "date", //设置日历初始视图模式 
		});
	}
	dtpicker1.show(function(rs) {
		$("#datePicker1").html(rs.text);
		date1 = rs.value;
	});
});
var dtpicker2; //防止重复加载
$("#datePicker2").bind("click", function() {
	if(dtpicker2 == null) {
		dtpicker2 = new mui.DtPicker({
			type: "date", //设置日历初始视图模式 
		});
	}
	dtpicker2.show(function(rs) {
		$("#datePicker2").html(rs.text);
		date2 = rs.value;
	});
});

function getListByRange(date1, date2) {
	dateFirstId=0;
	dateLastId=0;
	plus.nativeUI.showWaiting('正在获取数据。。。');
	var url = 'http://api.myautos.cn/api/weibo/GetMyWeiBoByDateRange';
	var datapara = {
		dateStart: date1,
		dateEnd: date2
	};
	//console.log(JSON.stringify(datapara));
	commonUtil.sendRequestWithToken(url, datapara, false, function(data) {
		plus.nativeUI.closeWaiting();
		$("#weibo-content").html("");
		allItems = new Array();
		if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
			var list = data.Data;
			allItems = allItems.concat(list); 
			for(var i = 0; i < list.length; i++) {
				var item = list[i];
				initWeiBoItem(item,true);
			}
			bindCheckBoxTab();
		} else {
			mui.toast("没有搜索到相关数据");
		}
	});
}