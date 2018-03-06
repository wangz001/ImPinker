(function(appConfig, owner) {
	
	//原生到航头样式
	owner.titleNView={ //详情页原生导航配置
				backgroundColor: '#f7f7f7', //导航栏背景色
				titleText: '', //导航栏标题
				titleColor: '#000000', //文字颜色
				type: 'transparent', //透明渐变样式
				autoBackButton: true, //自动绘制返回箭头
				splitLine: { //底部分割线
					color: '#cccccc'
				}
	};
	//请求接口的根地址
	owner.apiRoot="http://api.myautos.cn";
	
	//移动站地址
	owner.mWebRoot="http://m.myautos.cn";
	
	//图片规格配置
	owner.imgStyle={
		weibo_1200:'style/weibo_1200',
		weibo_200_200:'style/weibo_200_200',
		weibo_60_34:'style/weibo_60_34',    //600*340
		weibo_24_16:'style/weibo_24_16',    //240*160
		article_1200_605:'style/article_1200_605',
		article_900:'style/article_900',
		article_24_20:'style/article_24_20',
		articlecover_36_24:'style/articlecover_36_24',
		articlecover_100:'style/articlecover_100'
	}

}(mui, window.commonConfig = {}));