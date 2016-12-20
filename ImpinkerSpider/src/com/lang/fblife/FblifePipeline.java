package com.lang.fblife;

import us.codecraft.webmagic.ResultItems;
import us.codecraft.webmagic.Task;
import us.codecraft.webmagic.pipeline.Pipeline;

import com.lang.impinker.bll.ArticleBll;
import com.lang.impinker.model.Article;
import com.lang.impinker.model.CompanyEnum;
import com.lang.util.TUtil;

public class FblifePipeline implements Pipeline {

	ArticleBll articleBll = new ArticleBll();

	@Override
	public void process(ResultItems resultItems, Task task) {
		// TODO Auto-generated method stub
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
		article.setCompany(CompanyEnum.Fblife.getName());
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

		// 根据url获取id。如果id>0，表示存在，则重新做索引。如果==0，则表示不存在
		articleBll.AddArticle(article);
	}
}
