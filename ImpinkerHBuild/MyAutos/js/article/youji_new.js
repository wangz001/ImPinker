/**
 * 发帖子，图文排版
 **/
(function(apputil, owner) {
	var article = {

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
				resultStr += '<p>' + htmlnode.innerHTML + '</p>'
			}
		}
		return resultStr;
	}
	owner.createYouji = function(titleStr, callback) {
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/NewArticle';
		var data = {
			ArticleName: titleStr
		};
		commonUtil.sendRequestWithToken(url, data, true, function(data) {
			console.log(JSON.stringify(data));
			console.log(data.IsSuccess);
			if(data.IsSuccess == '1' && data.Data != null) {
				console.log('2');
				var articleinfo = data.Data;
				return callback(articleinfo.Id);
			} else {
				return;
			}
		});
	}

	owner.updateTitle = function(titleStr, callback) {
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/UpdateArticle';
		var data = {
			ArticleName: titleStr
		}
		commonUtil.sendRequestWithToken(url, data, true, function(data) {
			console.log(JSON.stringify(data));
			console.log('1');
			if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
				var articleinfo = data.Data;
				return callback(articleinfo.Id);
			} else {
				return;
			}
		});
	}

	//保存草稿
	owner.saveDraft = function(articleid, contentstr, callback) {
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/SaveDraft';
		var data = {
			Id: articleid,
			Content: contentstr
		};
		commonUtil.sendRequestWithToken(url, data, true, function(data) {
			console.log(JSON.stringify(data));
			if(data.IsSuccess == 1 && data.Data != null && data.Data.length > 0) {
				var articleinfo = data.Data;
				return callback(articleinfo.Id);
			} else {
				return;
			}
		});
	}
	
	//插入图片
	owner.addImage = function(articleid,callback) {
		callback = callback || $.noop;
		plus.gallery.pick(function(path) {
			plus.io.resolveLocalFileSystemURL(path, function(entry) {
				console.log("真实路径：" + entry.fullPath);
				plus.zip.compressImage({
					src: entry.fullPath,
					dst: '_doc/articleimage.jpg',
					overwrite: true,
					quality: 90
				}, function(zip) {
					var size = zip.size
					console.log("filesize:" + zip.size + ",totalsize:" + size);
					if(size > (3 * 1024 * 1024)) {
						return mui.toast('文件超大,请重新选择~');
					}
					if(articleid > 0 && zip.target.length > 0) {
						var url = 'http://api.myautos.cn/api/article/UploadArticleImage';
						var files = [];
						files.push({
							name: "articleimages",
							path: zip.target
						});
						var params = {
							"articleid": articleid
						};
						commonUtil.uploadImageWithFomedata(url, files, params, function(data) {
							console.log(JSON.stringify(data));
							return callback(data);
						});
					}
				}, function(zipe) {
					mui.toast('压缩失败！')
				});
			}, function(e) {
				alert(e.message);
			});
		}, function(e) {
			console.log("取消选择图片");
			return callback();
		}, {
			filter: "image",
			multiple: false
		});
	}
	
	
	//设置封面图
	owner.setCoverimage = function() {
		plus.gallery.pick(function(path) {
			plus.io.resolveLocalFileSystemURL(path, function(entry) {
				console.log("真实路径：" + entry.fullPath);
				plus.zip.compressImage({
					src: entry.fullPath,
					dst: '_doc/coverimage.jpg',
					overwrite: true,
					quality: 90
				}, function(zip) {
					var size = zip.size
					console.log("filesize:" + zip.size + ",totalsize:" + size);
					if(size > (3 * 1024 * 1024)) {
						return mui.toast('文件超大,请重新选择~');
					}
					if(articleid > 0 && zip.target.length > 0) {
						var url = 'http://api.myautos.cn/api/article/SetCoverImage';
						var files = [];
						files.push({
							name: "coverimageimages",
							path: zip.target
						});
						var params = {
							"articleid": articleid
						};
						commonUtil.uploadImageWithFomedata(url, files, params, function(data) {
							console.log(JSON.stringify(data));
							var url = data.Data;
							$("#coverimage").attr('src', url);
						});
					}
				}, function(zipe) {
					mui.toast('压缩失败！')
				});
			}, function(e) {
				alert(e.message);
			});
		}, function(e) {
			console.log("取消选择图片");
		}, {
			filter: "image",
			multiple: false
		});

	}
	//发布
	owner.publishYouji = function(articleid, contentstr, callback) {
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/PublishArticle';
		var data = {
			Id: articleid,
			Content: contentstr
		};
		commonUtil.sendRequestWithToken(url, data, true, function(data) {
			console.log(JSON.stringify(data));
			if(data.IsSuccess == 1 && data.Data != null) {
				var articleinfo = data.Data;
				return callback(articleinfo.Id);
			} else {
				return;
			}
		});

		
	}
}(mui, window.edityoujiUtil = {}));