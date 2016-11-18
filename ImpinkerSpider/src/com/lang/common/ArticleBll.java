package com.lang.common;

import com.lang.util.TUtil;

public class ArticleBll {

	ArticleDao articleDao = new ArticleDao();

	/**
	 * 添加数据库文档。添加或更新索引文档
	 * 
	 * @param article
	 */
	public void AddArticle(Article article) {
		// 根据url获取id。如果id>0，表示存在，则重新做索引。如果==0，则表示不存在
		synchronized (ArticleBll.class) { // 解决多线程重复插入的问题
			long id = GetIdByUrl(article.getUrl());
			if (id > 0) {
				article.setId(id);
			} else {
				id = articleDao.AddArticleWithSnap(article);// 同时向articlesnap表插入数据,事务操作
				if (id > 0) {
					article.setId(id);
					ArticleUrlCache.getInstance().AddUrl(article.getUrl(), id);// 添加到缓存
				} else {
					return; // 向数据库添加失败
				}
			}
		}
		String timeStr = TUtil.strToUTCTime(article.getPublishTime());
		article.setPublishTime(timeStr); // 转成utc时间格式
		// 添加索引
		SolrJUtil solrJUtil = SolrJUtil.getInstance();
		solrJUtil.AddDocs(article);
	}

	private long GetIdByUrl(String Url) {
		if ("" == Url) {
			return 0;
		}
		return ArticleUrlCache.getInstance().GetIdByUrl(Url);
	}
}
