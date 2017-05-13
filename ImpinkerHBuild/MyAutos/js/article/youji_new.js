/**
 * 发帖子，图文排版
 **/
(function(apputil, owner) {
	var article={
		
	}
	
	owner.showWeiBoItems = function(contentLists, items) {
		var imgPath = 'http://img.myautos.cn/';
		for(var i = 0; i < items.length; i++) {
			var item = items[i];
			if(item.ContentValue.length > 0) {
				var imgs = item.ContentValue.split(',');
				var imgHtmlStr = "";
				for(var j = 0; j < imgs.length; j++) {
					imgHtmlStr += '<p name="yjcontent"><img src="' + imgPath + imgs[j] + '"></p>';
					imgHtmlStr += '<textarea name="yjcontent">' + item.Description + '</textarea>';
					owner.InitDateObject(contentLists, "img", imgPath + imgs[j]);
					owner.InitDateObject(contentLists, "text", item.Description);
				}
				$(".mui-content-padded").append(imgHtmlStr);
			}
			if(item.Description.length > 0) {
				var textareaStr = '<textarea name="yjcontent" value="' + item.Description + '"></textarea>'
				$(".mui-content-padded").append(textareaStr);
				owner.InitDateObject(contentLists, "text", item.Description);
			}
		}
	}

	owner.InitDateObject = function(contentLists, dateType, dateValue) {
		var obj = new Object;
		obj.dateType = dateType;
		obj.dateValue = dateValue;
		contentLists.push(obj);
	}
	//根据标签 name属性，拼接出 content
	owner.getcontenthtml = function(nodename) {
		var resultStr = "";
		var yjcontents = document.getElementsByName(nodename);
		for(var i = 0; i < yjcontents.length; i++) {
			var htmlnode = yjcontents[i];
			if($(htmlnode).is("textarea")) {
				//console.log('是文本2:'+$(htmlnode).val());
				resultStr += '<p>' + $(htmlnode).val() + '</p>';
			} else {
				console.log('aa');
				resultStr += '<p>' + htmlnode.innerHTML + '</p>'
			}
		}
		return resultStr;
	}
	owner.createYouji = function(titleStr, callback) {
		callback = callback || $.noop;
		var userstate = app.getState();
		mui.ajax('http://api.myautos.cn/api/article/NewArticle', {
			data: {
				ArticleName: titleStr
			},
			headers: {
				"username": userstate.account,
				"usertoken": userstate.token
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post', //HTTP请求类型
			success: function(data) {
				console.log(JSON.stringify(data));
				console.log(data.IsSuccess);
				if(data.IsSuccess == '1' && data.Data != null) {
					console.log('2');
					var articleinfo = data.Data;
					return callback(articleinfo.Id);
				} else {
					return;
				}
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
			}
		});
	}

	owner.updateTitle = function(titleStr, callback) {
		callback = callback || $.noop;
		var userstate = app.getState();
		mui.ajax('http://api.myautos.cn/api/article/UpdateArticle', {
			data: {
				ArticleName: titleStr
			},
			headers: {
				"username": userstate.account,
				"usertoken": userstate.token
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post', //HTTP请求类型
			success: function(data) {
				console.log(JSON.stringify(data));
				console.log('1');
				if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
					var articleinfo = data.Data;
					return callback(articleinfo.Id);
				} else {
					return;
				}
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
			}
		});
	}

	//保存草稿
	owner.saveDraft = function(articleid, contentstr, callback) {
		callback = callback || $.noop;
		var userstate = app.getState();
		mui.ajax('http://api.myautos.cn/api/article/SaveDraft', {
			data: {
				Id: articleid,
				Content: contentstr
			},
			headers: {
				"username": userstate.account,
				"usertoken": userstate.token
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post', //HTTP请求类型
			success: function(data) {
				console.log(JSON.stringify(data));
				console.log('1');
				if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
					var articleinfo = data.Data;
					return callback(articleinfo.Id);
				} else {
					return;
				}
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
			}
		});
	}
	//设置封面图
	owner.setCoverimage = function(articleid, localimgpath, callback) {
		callback = callback || $.noop;
		var userstate = app.getState();
		var url = 'http://api.myautos.cn/api/article/SetCoverImage';
		var uploader = plus.uploader.createUpload(url, {
			method: 'POST'
		}, function(upload, status) {
			plus.nativeUI.closeWaiting()
			if(status == 200 || status == "200") {
				console.log(upload.responseText);
				var data = JSON.parse(upload.responseText);
				//上传成功，重置表单
				if(data.IsSuccess === 1) {
					return callback(data);
				}
			}
			return callback();
		});
		//设置头信息
		uploader.setRequestHeader("username", userstate.account);
		uploader.setRequestHeader("usertoken", userstate.token);
		//添加上传数据
		uploader.addData("articleid", articleid.toString());
		//添加上传文件
		uploader.addFile(localimgpath, {
			key: 'coverimage'
		});
		//开始上传任务
		uploader.start();
		plus.nativeUI.showWaiting('正在发布');
	}
	//发布
	owner.publishYouji = function(articleid, contentstr, callback) {
		callback = callback || $.noop;
		var userstate = app.getState();
		mui.ajax('http://api.myautos.cn/api/article/PublishArticle', {
			data: {
				Id: articleid,
				Content: contentstr
			},
			headers: {
				"username": userstate.account,
				"usertoken": userstate.token
			},
			dataType: 'json', //服务器返回json格式数据
			type: 'post', //HTTP请求类型
			success: function(data) {
				console.log(JSON.stringify(data));
				if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
					var articleinfo = data.Data;
					return callback(articleinfo.Id);
				} else {
					return;
				}
			},
			error: function(xhr, type, errorThrown) {
				console.log(JSON.stringify(errorThrown));
			}
		});
	}
}(mui, window.edityoujiUtil = {}));