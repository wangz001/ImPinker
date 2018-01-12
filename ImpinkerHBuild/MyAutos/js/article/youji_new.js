/**
 * 发帖子，图文排版
 **/
(function(apputil, owner) {
	owner.article = {
		articleid: 0,
		articlename: "",
		coverimage: "",
		description: "",
		content: "",
		currentNode: null //文章中间插入内容的标记
	}
	var weiboimg_1200style = '?x-oss-process=style/weibo_1200';
	var weiboimg_24style = '?x-oss-process=style/weibo_24_16';

	var article_900 = '?x-oss-process=style/article_900';

	owner.showWeiBoItems = function(items) {
		for(var i = 0; i < items.length; i++) {
			var item = items[i];
			if(item.ContentValue.length > 0) {
				if(item.Description != "") {
					owner.showTextAreaStr(item.Description);
				}
				var imgs = item.ContentValue.split(',');
				var imgHtmlStr = "";
				for(var j = 0; j < imgs.length; j++) {
					owner.showImage(imgs[j]);

				}
			}
		}
	}
	//显示文字
	owner.showTextAreaStr = function(textStr) {
		if(owner.article.currentNode == null) {
			//初始化时，第一个文本框是默认
			owner.article.currentNode = $(".mui-content-padded").children().last();
		}
		//图片之间只能有一个textarea。多了的内容合并
		if($(owner.article.currentNode).is("textarea")) {
			var inner = $(owner.article.currentNode).val();
			inner = inner.trim() == "" ? textStr : inner + "\r\n" + textStr;
			$(owner.article.currentNode).val(inner);
			autosize(owner.article.currentNode);
			autosize.update(owner.article.currentNode);
		} else {
			var textareaDom = document.createElement("textarea");
			$(textareaDom).attr("name", "yjcontent");
			$(textareaDom).val(textStr);
			textareaDom.innerHTML = textStr;
			//var textareaStr = '<textarea name="yjcontent" value="' + textStr + '">' + textStr + '</textarea>';
			$(owner.article.currentNode).after(textareaDom);
			owner.article.currentNode = $(owner.article.currentNode).next();
			autosize(textareaDom);
			autosize.update(textareaDom);
		}

	}
	//显示图片(可在文章中间插入图片)
	owner.showImage = function(imgurl) {
		if(owner.article.currentNode == null) {
			console.log("111");
			//初始化时，第一个文本框是默认
			owner.article.currentNode = $(".mui-content-padded").children().last();
		}
		var imgPath = 'http://img.myautos.cn/';
		if(imgurl.indexOf('http://') == -1) {
			imgurl = imgPath + imgurl;
		}
		//显示尺寸为900的图片
		var imgurl_900 = imgurl.substring(0, imgurl.indexOf('.jpg') + 4);
		imgurl_900 = imgurl_900 + article_900;
		//获取模板上的HTML
		var imgTemplate = $('script[id="postimg"]').html();
		var imgHtmlStr = imgTemplate.temp({
			"src": imgurl_900
		});
		//$(".mui-content-padded").append(imgHtmlStr);
		$(owner.article.currentNode).after(imgHtmlStr);
		owner.article.currentNode = $(owner.article.currentNode).next();
		owner.showTextAreaStr("");

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
					} else {
						mui.toast("请填写游记名称~");
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
		var url = 'http://api.myautos.cn/api/article/UpdateArticleTitle';
		var data = {
			Id: owner.article.articleid,
			ArticleName: titleStr
		}
		owner.article.articlename = titleStr;
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
			if(data.IsSuccess == 1 && data.Data) {
				var articleinfo = data.Data;
				console.log("aaa");
				return callback(articleinfo);
			} else {
				return callback();
			}
		});
	}

	//设置封面图
	owner.setCoverimage = function() {
		plus.gallery.pick(function(path) {
			commonUtil.compressImage(path, function(zip) {
				var size = zip.size
				console.log("filesize:" + zip.size + ",totalsize:" + size);
				if(size > (3 * 1024 * 1024)) {
					return mui.toast('文件超大,请重新选择~');
				}
				if(owner.article.articleid > 0 && zip.target.length > 0) {
					var url = 'http://api.myautos.cn/api/article/SetCoverImage';
					var files = [];
					files.push({
						name: "coverimageimages",
						path: zip.target
					});
					console.log(owner.article.articleid);
					var params = {
						"articleid": owner.article.articleid
					};
					commonUtil.uploadImageWithFomedata(url, files, params, function(data) {
						console.log(JSON.stringify(data));
						var url = data.Data;
						owner.article.coverimage = url;
						$("#setcoverimage").attr('src', url);
					});
				} else {
					mui.toast("请填写游记名称~");
				}
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
			return callback('请设置封面图');
		}
		if(owner.article.articleid == 0) {
			return callback('articleid不能为0啊');
		}
		if(owner.article.articlename == "") {
			return callback('名称不能为空啊');
		}
		if(owner.article.description == "") {
			return callback('简介不能为空啊');
		}
		var resultStr = edityoujiUtil.getcontenthtml('yjcontent');
		if(resultStr == "") {
			return callback('resultStr不能为kong啊');
		}
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/PublishArticle';
		var data = {
			Id: owner.article.articleid,
			ArticleName: owner.article.articlename,
			Description: owner.article.description,
			Content: resultStr
		};
		commonUtil.sendRequestWithToken(url, data, true, function(data) {
			console.log(JSON.stringify(data));
			if(data.IsSuccess == 1 && data.Data != null) {
				var articleinfo = data.Data;
				return callback();
			} else {
				return callback("err");
			}
		});
	}
	//获取文章内容
	owner.getandShowContent = function(articleid, callback) {
		callback = callback || $.noop;
		var url = 'http://api.myautos.cn/api/article/GetArticleWithContent';
		var data = {
			articleid: articleid,
		};
		commonUtil.sendRequestGet(url, data, function(data) {
			if(data.IsSuccess == 1 && data.Data != null) {
				var articleinfo = data.Data;
				var contentStr = articleinfo.Content;
				owner.showContentEdit(contentStr);
				return callback();
			} else {
				return callback();
			}
		});
	}
	//把文章内容显示为编辑状态
	owner.showContentEdit = function(contentStr) {
		$("#hideContent").append(contentStr);
		var pArr = $("#hideContent p");
		for(var i = 0; i < pArr.length; i++) {
			var pitem = pArr[i];
			var img = $(pitem).find("img");
			if(img != null && img.length > 0) {
				//图片内容
				var src = $(img[0]).attr('src');
				owner.showImage(src);
			} else {
				var text = $(pitem).text();
				owner.showTextAreaStr(text);
			}
		}
	}
}(mui, window.edityoujiUtil = {}));