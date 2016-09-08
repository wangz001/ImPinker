package com.lang.autohome;

import us.codecraft.webmagic.ResultItems;
import us.codecraft.webmagic.Task;
import us.codecraft.webmagic.pipeline.Pipeline;

import com.lang.common.Article;
import com.lang.common.ArticleBll;
import com.lang.common.CompanyEnum;
import com.lang.util.TUtil;

public class AutohomePipeline implements Pipeline {

	ArticleBll articleBll = new ArticleBll();

	@Override
	public void process(ResultItems resultItems, Task task) {
		// TODO Auto-generated method stub
		Article article = new Article();
		article.setTitle(resultItems.get("title").toString());
		article.setKeyWord(resultItems.get("keyword").toString());
		if (resultItems.get("description").toString().length() > 200) {
			article.setDescription(resultItems.get("description").toString()
					.substring(0, 198));
		} else {
			article.setDescription(resultItems.get("description").toString());
		}
		article.setUrlString(resultItems.get("url").toString());
		article.setCompany(CompanyEnum.Autohome.getName());
		article.setCoverImage(resultItems.get("CoverImage").toString());
		article.setContent(resultItems.get("Content").toString());
		String timeString = "";
		if (null != resultItems.get("publishtime")
				&& "" != resultItems.get("publishtime")) {
			timeString = resultItems.get("publishtime").toString();
		} else {
			timeString = TUtil.getCurrentTime();
		}
		article.setCreateTime(timeString);
		articleBll.AddArticle(article);
	}

}
