/**
 * 数据存储公用方法
 **/
(function(apputil, owner) {
	
	//保存轮播图数据
	owner.setSliderData = function(sliderData) {
		plus.storage.setItem('$article_slider', JSON.stringify(sliderData)); //app本地存储（速度慢，但可跨域）
	}
	//保存轮播图数据
	owner.getSliderData = function() {
		var sliderData = plus.storage.getItem('$article_slider');
		return JSON.parse(sliderData);
	}
	
	//保存第一页文章
	owner.setFirstPageArticle = function(articleData) {
		plus.storage.setItem('$article_firstpage', JSON.stringify(articleData)); //app本地存储（速度慢，但可跨域）
	}
	//获取第一页文章
	owner.getFirstPageArticle = function() {
		var articleData = plus.storage.getItem('$article_firstpage');
		return JSON.parse(articleData);
	}
	
	//保存第一页weibo
	owner.setFirstPageWeibo = function(weiboData) {
		plus.storage.setItem('$weibo_firstpage', JSON.stringify(weiboData)); //app本地存储（速度慢，但可跨域）
	}
	//获取第一页weibo
	owner.getFirstPageWeibo = function() {
		var weiboData = plus.storage.getItem('$weibo_firstpage');
		return JSON.parse(weiboData);
	}
	
	//var userstate = app.getState();
	var userstate = JSON.parse(localStorage.getItem('$state') || "{}");
	//保存我赞过的weibo
	owner.setWeiboVote = function(weiboid) {
		userstate = JSON.parse(localStorage.getItem('$state') || "{}");
		var username='0';
		if(userstate&&userstate.account){
			username=userstate.account;
		}
		localStorage.setItem('$weibo_vote_'+username+"_"+weiboid, "true"); //app本地存储（速度慢，但可跨域）
	}
	//获取我赞过的weibo
	owner.getWeiboVote = function(weiboid) {
		userstate = JSON.parse(localStorage.getItem('$state') || "{}");
		var username='0';
		if(userstate!=null){
			username=userstate.account;
		}
		var isVote = localStorage.getItem('$weibo_vote_'+username+"_"+weiboid);
		if(isVote!=null&&isVote=="true"){
			return true;
		}
		return false;
	}
	
	
	//保存我赞过的文章
	owner.setArticleVote = function(articleid) {
		userstate = JSON.parse(localStorage.getItem('$state') || "{}");
		var username='0';
		if(userstate&&userstate.account){
			username=userstate.account;
		}
		localStorage.setItem('$article_vote_'+username+"_"+articleid, "true"); //app本地存储（速度慢，但可跨域）
	}
	//获取我赞过的文章
	owner.getArticleVote = function(articleid) {
		userstate = JSON.parse(localStorage.getItem('$state') || "{}");
		var username='0';
		if(userstate!=null){
			username=userstate.account;
		}
		var isVote = localStorage.getItem('$article_vote_'+username+"_"+articleid);
		if(isVote!=null&&isVote=="true"){
			return true;
		}
		return false;
	}
	

}(mui, window.storageUtil = {}));