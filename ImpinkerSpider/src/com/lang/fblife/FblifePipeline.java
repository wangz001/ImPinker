package com.lang.fblife;

import com.lang.common.Article;
import com.lang.common.ArticleDao;
import com.lang.util.DBConnection;
import com.lang.util.DBHelper;

import us.codecraft.webmagic.ResultItems;
import us.codecraft.webmagic.Task;
import us.codecraft.webmagic.pipeline.Pipeline;

public class FblifePipeline implements Pipeline {

	ArticleDao articleDao=new ArticleDao(); 
	@Override
	public void process(ResultItems resultItems, Task task) {
		// TODO Auto-generated method stub
		Article article=new Article();
		article.setTitle(resultItems.get("title").toString());
		article.setKeyWord(resultItems.get("keyword").toString());
		article.setDescription(resultItems.get("description").toString());
		article.setUrlString(resultItems.get("url").toString());
		boolean flag=articleDao.NewArticle(article);
		
		System.out.println(resultItems.get("title"));
	}

}
