package com.lang.autohome;

import us.codecraft.webmagic.ResultItems;
import us.codecraft.webmagic.Task;
import us.codecraft.webmagic.pipeline.Pipeline;

import com.lang.common.Article;
import com.lang.common.ArticleDao;
import com.lang.common.CompanyEnum;
import com.lang.common.SolrJUtil;
import com.lang.util.TUtil;

public class AutohomePipeline implements Pipeline {

	ArticleDao articleDao = new ArticleDao();

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
		if ("" != resultItems.get("publishtime")) {
			timeString = resultItems.get("publishtime").toString();
		} else {
			timeString = TUtil.getCurrentTime();
		}
		article.setCreateTime(timeString);
		boolean flag = articleDao.IsExist(article);
		if (!flag) {
			long id = articleDao.Add(article);
			article.setId(id);
			String timeStr = TUtil.strToUTCTime(article.getCreateTime());
			article.setCreateTime(timeStr); // 转成utc时间格式
			// 添加索引
			SolrJUtil solrJUtil = SolrJUtil.getInstance();
			solrJUtil.AddDocs(article);
		}
	}

}
