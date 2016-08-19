package com.lang.fblife;

import us.codecraft.webmagic.ResultItems;
import us.codecraft.webmagic.Task;
import us.codecraft.webmagic.pipeline.Pipeline;

import com.lang.common.Article;
import com.lang.common.ArticleDao;
import com.lang.common.CompanyEnum;
import com.lang.common.SolrJUtil;

public class FblifePipeline implements Pipeline {

	ArticleDao articleDao = new ArticleDao();

	@Override
	public void process(ResultItems resultItems, Task task) {
		// TODO Auto-generated method stub
		Article article = new Article();
		article.setTitle(resultItems.get("title").toString());
		article.setKeyWord(resultItems.get("keyword").toString());
		article.setDescription(resultItems.get("description").toString());
		article.setUrlString(resultItems.get("url").toString());
		article.setCompany(CompanyEnum.Fblife.getName());
		article.setCoverImage(resultItems.get("CoverImage").toString());
		article.setContent(resultItems.get("Content").toString());

		boolean flag = articleDao.IsExist(article);
		if (!flag) {
			long id = articleDao.Add(article);
			article.setId(id);
			// 添加索引
			SolrJUtil solrJUtil = SolrJUtil.getInstance();
			solrJUtil.AddDocs(article);
		}
		System.out.println(resultItems.get("title"));
	}

}
