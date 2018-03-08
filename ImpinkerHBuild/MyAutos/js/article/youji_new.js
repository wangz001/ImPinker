mui.init({
	beforeback: function() {
		if(edityoujiUtil.article.articleid > 0) {
			plus.nativeUI.showWaiting('正在发布保存到草稿');
			edityoujiUtil.saveDraft(function(data) {
				plus.nativeUI.closeWaiting();
				mui.toast("修改已保存到草稿！");
				setTimeout(function() {
					var self = plus.webview.currentWebview();
					self.close();
				}, 1000);
			});
			return false;
		}
		return true;
	}
});
mui.plusReady(function() {
	var self = plus.webview.currentWebview();
	//从草稿页跳转
	var articleid = self.articleid;
	var articlename = self.articlename;
	var coverimage = self.coverimage;
	var description = self.description;
	var content = self.content;
	if(articleid != null && articleid > 0) {
		edityoujiUtil.article.articleid = articleid;
		edityoujiUtil.article.articlename = articlename;
		edityoujiUtil.article.coverimage = coverimage;
		edityoujiUtil.article.description = description;
		edityoujiUtil.article.content = content;
		$("#youi_title").val(articlename);
		$("#coverimage").attr('src', coverimage);
		$("#description").val(description);
		edityoujiUtil.getandShowContent(articleid, function() {});
	}
	autosize($(".mui-content-padded textarea"));
});
//自定义事件。接受选择微博页传回的数据
window.addEventListener('weiboitems', function(event) {
	//获得事件参数  
	var selectedWeibos = event.detail.items;
	if(selectedWeibos != null) {
		//在页面上展示
		edityoujiUtil.showWeiBoItems(selectedWeibos);
	}
});

$("#youi_title").blur(function() {
	var content = $(this).val();
	console.log(content);
	if(content == "") return;
	//创建文章或更新标题
	setTimeout(function() {
		if(edityoujiUtil.article.articleid == 0) {
			edityoujiUtil.createYouji(content, function(id) {
				if(id != null && id > 0) {
					console.log(id);
					articleid = id;
				}
			});
		} else {
			edityoujiUtil.article.articlename = content;
		}
	}, 500);
});
//简介
$("#description").change(function() {
	var textDescription = $(this).val();
	setTimeout(function() {
		console.log(textDescription);
		edityoujiUtil.article.description = textDescription;
	}, 200);
});
//图片删除图片事件绑定  mui-icon-trash
mui('.mui-content-padded').on('tap', '.post-img .del', function() {
	blurTextarea();
	var thisa = this;
	mui.confirm("是否要删除图片？", "", ["是", "否"], function(event, index) {
		if(event.index == 0) {
			console.log("aaa");
			var imgitem = $(thisa).parents('.post-img');
			edityoujiUtil.delImage(imgitem)
//			var nextTextarea = $(imgitem).next('textarea');
//			var preTextarea = $(imgitem).prev('textarea');
//			var preTextval=($(preTextarea).val()!=null ? $(preTextarea).val():"");
//			$(preTextarea).val(preTextval + "\r\n" + $(nextTextarea).val()); //两个文本框的内容合并
//			$(imgitem).remove(); //删除图片和图片的下一个textarea
//			$(nextTextarea).remove();
		}
	});
});

//让textarea失去焦点
function blurTextarea(){
	console.log("aa");
	$("textarea").blur();
}

//时间及地理位置删除图片事件绑定  mui-icon-trash
mui('.mui-content-padded').on('tap', '.datetimeContent .mui-icon-close', function() {
	blurTextarea();
	var thisa = this;
	mui.confirm("是否要删除当前块？", "", ["是", "否"], function(event, index) {
		if(event.index == 0) {
			console.log("aaa");
			var imgitem = $(thisa).parent('div');
			$(imgitem).remove(); 
		}
	});
});
mui('.mui-content-padded').on('tap', '.gisContent .mui-icon-close', function() {
	blurTextarea();
	var thisa = this;
	console.log("bbb");
	mui.confirm("是否要删除当前块？", "", ["是", "否"], function(event, index) {
		if(event.index == 0) {
			var imgitem = $(thisa).parent('div');
			$(imgitem).remove(); 
		}
	});
});

//标记当前文本框
mui('.mui-content-padded').on('tap', 'textarea', function() {
	edityoujiUtil.article.currentNode = $(this).parent(".textareacontent");
});
//保存草稿
$("#youji_content textarea").change(function() {
	setTimeout(function() {
		if(edityoujiUtil.article.articleid > 0) {
			edityoujiUtil.saveDraft(function(data) {
				console.log("保存草稿成功！");
			});
		} else {
			edityoujiUtil.createYouji("草稿" + (new Date().toLocaleString()), function(id) {
				if(id != null && id > 0) {
					console.log(id);
				}
			});
		}
	}, 2000);
});
//设置封面图
document.getElementById('setcoverimage').addEventListener('tap', function() {
	//edityoujiUtil.setCoverimage();
	mui("#middlePopover").popover("show");
});
$("#middlePopover a").bind("click", function() {
	mui("#middlePopover").popover("hide");
	var type = $(this).attr("datetype");
	if(type == "1") {
		edityoujiUtil.setCoverimage();
	} else {
		mui.toast("请稍后");
	}
});
//发布
document.getElementById('publishyouji').addEventListener('tap', function() {
	setTimeout(function() {
		edityoujiUtil.publishYouji(function(err) {
			if(err) {
				alert(err);
			} else {
				plus.webview.getWebviewById("tab-webview-subpage-article.html").reload();
				//关闭页面
				var curr = plus.webview.currentWebview();
				var youjiPage = plus.webview.getWebviewById('youji_new');
				var selectPage = plus.webview.getWebviewById('youji_new_selectweibo');
				plus.webview.close(youjiPage);
				plus.webview.close(selectPage);
				plus.webview.close(curr);
				//mui.back();
			}
		});
	}, 500);
});

//添加图片
document.getElementById('addimage').addEventListener('tap', function() {
	edityoujiUtil.addImage(function(data) {
		edityoujiUtil.showImage(data.Data);
	});
});
//添加微博
document.getElementById('selectWeibo').addEventListener('tap', function() {
	mui.openWindow({
		url: "youji_new_selectweibo.html",
		id: "youji_new_selectweibo",
		createNew: true,
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		}
	});
});
//预览
document.getElementById('preview_btn').addEventListener('tap', function() {
	var resultStr = edityoujiUtil.getcontenthtml('yjcontent');
	mui.openWindow({
		url: "youji_preview.html",
		id: "youji_preview",
		createNew: true,
		show: {
			aniShow: 'slide-in-right',
			duration: 200
		},
		extras: {
			contentStr: resultStr
		},
	});
});