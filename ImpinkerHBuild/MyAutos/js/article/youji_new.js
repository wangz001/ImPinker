/**
 * 发帖子，图文排版
 **/
(function(apputil, owner) {
	owner.article = {
		articleid: 0,
		articlename: "",
		coverimage: "",
		content: ""
	}
	var weiboimg_1200style = '?x-oss-process=style/weibo_1200';
	var weiboimg_24style='?x-oss-process=style/weibo_24_16';
	
	var article_900='?x-oss-process=style/article_900';

	owner.showWeiBoItems = function(items) {
		for(var i = 0; i < items.length; i++) {
			var item = items[i];
			if(item.ContentValue.length > 0) {
				var imgs = item.ContentValue.split(',');
				var imgHtmlStr = "";
				for(var j = 0; j < imgs.length; j++) {
					owner.showImage(imgs[j], item.Description);
				}
			}
		}
	}
	owner.showImage = function(imgurl, contenttext) {
		var imgPath = 'http://img.myautos.cn/';
		if(imgurl.indexOf('http://') == -1) {
			imgurl = imgPath + imgurl;
		}
		console.log(imgurl);
		//显示尺寸为900的图片
		var imgurl_900=imgurl.substring(0,imgurl.indexOf('.jpg')+4);
		console.log(imgurl_900);
		imgurl_900=imgurl_900+article_900;
		//获取模板上的HTML
		var imgTemplate = $('script[id="postimg"]').html();
		var imgHtmlStr = imgTemplate.temp({
			"src": imgurl_900
		});
		var textareaStr = '<textarea name="yjcontent" value="' + contenttext + '"></textarea>';
		$(".mui-content-padded").append(imgHtmlStr);
		$(".mui-content-padded").append(textareaStr);

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

	//插入图片
	owner.addImage = function(callback) {
		callback = callback || $.noop;
		plus.gallery.pick(function(path) {
			plus.io.resolveLocalFileSystemURL(path, function(entry) {
				console.log("真实路径：" + entry.fullPath);
				plus.zip.compressImage({
					src: entry.fullPath,
					dst: '_doc/articleimage.jpg',
					overwrite: true,
					width: "1200px",
					quality: 90
				}, function(zip) {
					var size = zip.size
					console.log("filesize:" + zip.size + ",totalsize:" + size);
					if(size > (3 * 1024 * 1024)) {
						return mui.toast('文件超大,请重新选择~');
					}
					if(owner.article.articleid > 0 && zip.target.length > 0) {
						var url = 'http://api.myautos.cn/api/article/UploadArticleImage';
						var files = [];
						files.push({
							name: "articleimages",
							path: zip.target
						});
						var params = {
							"articleid": owner.article.articleid
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

	owner.createYouji = function(titleStr, callback) {
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/NewArticle';
		var data = {
			ArticleName: titleStr
		};
		owner.article.articlename = titleStr;
		commonUtil.sendRequestWithToken(url, data, true, function(data) {
			console.log(JSON.stringify(data));
			if(data.IsSuccess == '1' && data.Data != null) {
				var articleinfo = data.Data;
				owner.article.articleid = articleinfo.Id;
				return callback(articleinfo.Id);
			} else {
				return;
			}
		});
	}

	owner.updateTitle = function(titleStr, callback) {
		console.log(owner.article.articleid);
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/UpdateArticle';
		var data = {
			ArticleName: titleStr
		}
		owner.article.articlename = titleStr;
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
	owner.saveDraft = function(callback) {
		if(owner.article.articleid == 0) {
			alert("请输入标题");
			return;
		}
		callback = callback || $.noop;
		var resultStr = edityoujiUtil.getcontenthtml('yjcontent');
		var url = 'http://api.myautos.cn/api/article/SaveDraft';
		var data = {
			Id: owner.article.articleid,
			Content: resultStr
		};
		owner.article.content = resultStr;
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

	//设置封面图
	owner.setCoverimage = function() {
		plus.gallery.pick(function(path) {
			plus.io.resolveLocalFileSystemURL(path, function(entry) {
				console.log("真实路径：" + entry.fullPath);
				plus.zip.compressImage({
					src: entry.fullPath,
					dst: '_doc/coverimage.jpg',
					width: "1200px",
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
							"articleid": owner.article.articleid
						};
						commonUtil.uploadImageWithFomedata(url, files, params, function(data) {
							console.log(JSON.stringify(data));
							var url = data.Data;
							owner.article.coverimage = url;
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
	owner.publishYouji = function(callback) {
		if(owner.article.coverimage == "") {
			alert('请设置封面图');
			return;
		}
		if(owner.article.articleid == 0) {
			alert('articleid不能为0啊');
			return;
		}
		var resultStr = edityoujiUtil.getcontenthtml('yjcontent');
		if(resultStr == "") {
			alert('resultStr不能为kong啊');
			return;
		}
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/PublishArticle';
		var data = {
			Id: articleid,
			Content: resultStr
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