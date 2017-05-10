/**
 * 发帖子，图文排版
 **/
(function(app, owner) {
	owner.showWeiBoItems = function(contentLists, items) {
		var imgPath = 'http://img.myautos.cn/';
		for(var i = 0; i < items.length; i++) {
			var item = items[i];
			if(item.ContentValue.length > 0) {
				var imgs = item.ContentValue.split(',');
				var imgHtmlStr = "";
				for(var j = 0; j < imgs.length; j++) {
					imgHtmlStr += '<p><img src="' + imgPath + imgs[j] + '"></p>';
					imgHtmlStr += '<textarea>' + item.Description + '</textarea>';
					owner.InitDateObject(contentLists,"img",imgPath + imgs[j]);
					owner.InitDateObject(contentLists,"text",item.Description);
				}
				$(".mui-content-padded").append(imgHtmlStr);
			}
			if(item.Description.length > 0) {
				var textareaStr = '<textarea value="' + item.Description + '"></textarea>'
				$(".mui-content-padded").append(textareaStr);
				owner.InitDateObject(contentLists,"text",item.Description);
			}
		}
	}
	
	owner.InitDateObject=function(contentLists,dateType,dateValue){
		var obj=new Object;
		obj.dateType=dateType;
		obj.dateValue=dateValue;
		contentLists.push(obj);
	}
	

}(mui, window.edityoujiUtil = {}));