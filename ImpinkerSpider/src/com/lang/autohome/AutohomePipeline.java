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
		Article article = new Article();
		article.setUrl(resultItems.get("url").toString());
		article.setTitle(resultItems.get("title").toString());
		if (resultItems.get("keyword").toString().length() > 100) {
			article.setKeyWord(resultItems.get("keyword").toString()
					.substring(0, 98));
		} else {
			article.setKeyWord(resultItems.get("keyword").toString());
		}
		if (resultItems.get("description").toString().length() > 200) {
			article.setDescription(resultItems.get("description").toString()
					.substring(0, 198));
		} else {
			article.setDescription(resultItems.get("description").toString());
		}
		article.setCompany(CompanyEnum.Autohome.getName());
		String timeString = "";
		if ("" != resultItems.get("publishtime")) {
			timeString = resultItems.get("publishtime").toString();
		} else {
			timeString = TUtil.getCurrentTime();
		}
		article.setPublishTime(timeString);
		// articleSnap表的属性
		article.setSnapFirstImageUrl(resultItems.get("snapCoverImage")
				.toString());
		article.setSnapKeyWords(resultItems.get("snapKeyWords").toString());
		article.setSnapDescription(resultItems.get("snapDescription")
				.toString());
		article.setSnapContent(resultItems.get("snapContent").toString());
		articleBll.AddArticle(article);
	}

}
