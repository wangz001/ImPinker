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

	/*
	 在页面上显示微博。先显示地理位置、日期，然后显示图片，最后显示图片介绍
	 * */
	owner.showWeiBoItems = function(items) {
		for(var i = 0; i < items.length; i++) {
			var item = items[i];
			if(item.ContentValue.length > 0) {
				owner.showDatetime(item.PublishTime);
				owner.showGIs(item.LocationText);
				var imgs = item.ContentValue.split(',');
				var imgHtmlStr = "";
				for(var j = 0; j < imgs.length; j++) {
					owner.showImage(imgs[j]);
				}
				if(item.Description != "") {
					owner.showTextAreaStr(item.Description);
				}
			}
		}
	}
	//显示文字  
	var textareaTemplate = $('script[id="textareacontent"]').html();
	owner.showTextAreaStr = function(textStr) {
		if(owner.article.currentNode == null) {
			//初始化时，第一个文本框是默认
			owner.article.currentNode = $(".mui-content-padded").children().last();
		}
		//图片之间只能有一个textarea。多了的内容合并
		var currentNodeClass = $(owner.article.currentNode).attr("class");
		if(currentNodeClass.indexOf("textareacontent") != -1) {
			var currentTextarea = $(owner.article.currentNode).find("textarea");
			var inner = $(currentTextarea).val();
			inner = inner.trim() == "" ? textStr : inner + "\r\n" + textStr;
			$(currentTextarea).val(inner);
			autosize(currentTextarea);
			autosize.update(currentTextarea);
		} else {
			var textareaHtml = textareaTemplate.temp({
				"text": textStr
			});
			$(owner.article.currentNode).after(textareaHtml);
			owner.article.currentNode = $(owner.article.currentNode).next();
			var textareaDom = $(owner.article.currentNode).find("textarea");
			autosize(textareaDom);
			autosize.update(textareaDom);
		}

	}
	//显示图片(可在文章中间插入图片)
	var imgTemplate = $('script[id="postimg"]').html();
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

		var imgHtmlStr = imgTemplate.temp({
			"src": imgurl_900
		});
		//$(".mui-content-padded").append(imgHtmlStr);
		$(owner.article.currentNode).after(imgHtmlStr);
		owner.article.currentNode = $(owner.article.currentNode).next();
		owner.showTextAreaStr("");

	}

	//删除图片
	owner.delImage = function(postImg) {
		var imgitem = postImg;
		var nextDiv = $(imgitem).next(".textareacontent");
		var preDiv = $(imgitem).prev(".textareacontent");
		var nextTextarea=$(nextDiv).find("textarea");
		var preTextarea=$(preDiv).find("textarea");
		var preTextval = ($(preTextarea).val() != null ? $(preTextarea).val() : "");
		$(preTextarea).val(preTextval + "\r\n" + $(nextTextarea).val()); //两个文本框的内容合并
		$(imgitem).remove(); //删除图片和图片的下一个textarea
		$(nextDiv).remove();
	}

	//显示时间
	var datetimeTemplate = $('script[id="datetimecontent"]').html();
	var lastDatetimeStr;
	owner.showDatetime = function(datetime) {
		if(owner.article.currentNode == null) {
			//初始化时，第一个文本框是默认
			owner.article.currentNode = $(".mui-content-padded").children().last();
		}
		//当日期不同时，显示新日期
		if(datetime != lastDatetimeStr) {
			lastDatetimeStr = datetime;
			var datetemeHtmlStr = datetimeTemplate.temp({
				"PublishTime": datetime
			});
			$(owner.article.currentNode).after(datetemeHtmlStr);
			owner.article.currentNode = $(owner.article.currentNode).next();
		}
	}

	//显示地理位置
	var gisTemplate = $('script[id="giscontent"]').html();
	owner.showGIs = function(locationStr) {
		if(owner.article.currentNode == null) {
			//初始化时，第一个文本框是默认
			owner.article.currentNode = $(".mui-content-padded").children().last();
		}
		if(locationStr != null && locationStr != "") {
			var gisHtmlStr = gisTemplate.temp({
				"LocationText": locationStr
			});
			$(owner.article.currentNode).after(gisHtmlStr);
			owner.article.currentNode = $(owner.article.currentNode).next();
		}
	}

	//根据标签 name属性，拼接出 游记content
	owner.getcontenthtml = function(nodename) {
		var resultStr = "";
		var yjcontents = document.getElementsByName(nodename);
		for(var i = 0; i < yjcontents.length; i++) {
			var htmlnode = yjcontents[i];
			if($(htmlnode).is("textarea")) {
				//console.log('是文本2:'+$(htmlnode).val());
				//替换空格和换行符
				var strContent=$(htmlnode).val();
				strContent = strContent.replace(/\r\n/g, '<br/>'); //IE9、FF、chrome  
        		strContent = strContent.replace(/\n/g, '<br/>'); //IE7-8
        		//strContent=strContent.replace(/\s/g,' ');
				resultStr += '<p>' + strContent + '</p>';
			} else {
				var divClass = $(htmlnode).attr("class");
				if(divClass.indexOf("add-img") != -1) {
					//图片
					resultStr += '<p>' + htmlnode.innerHTML + '</p>'
				}
				if(divClass.indexOf("datetimeContent") != -1) {
					//时间
					resultStr += '<p class="date-time"><span>' + htmlnode.innerText + '</span></p>';
				}
				if(divClass.indexOf("gisContent") != -1) {
					//gis 位置
					resultStr += '<p class="gis-location"><span class="mui-icon mui-icon-location"></span><span>' + htmlnode.innerText + '</span></p>'
				}
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
						var url = commonConfig.apiRoot+'/api/article/UploadArticleImage';
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
		var url = commonConfig.apiRoot+'/api/article/NewArticle';
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
		var url = commonConfig.apiRoot+'/api/article/UpdateArticleTitle';
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
		var url = commonConfig.apiRoot+'/api/article/SaveDraft';
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
					var url = commonConfig.apiRoot+'/api/article/SetCoverImage';
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
		var url = commonConfig.apiRoot+'/api/article/PublishArticle';
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
		var url = commonConfig.apiRoot+'/api/article/GetArticleWithContent';
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