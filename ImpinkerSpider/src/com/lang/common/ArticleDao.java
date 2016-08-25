package com.lang.common;

import com.lang.util.DBHelper;

public class ArticleDao {

	public boolean NewArticle(Article article) {
		if (IsExist(article)) {
			return true;
		} else {
			return Add(article) > 0;
		}
	}

	public long Add(Article article) {

		String sqlString = "INSERT INTO Article"
				+ "(ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime,Company) "
				+ "values (?,?,?,?,?,?,?,?,?,?)";
		Object[] objects = new Object[] { article.getTitle(),
				article.getUrlString(), article.getCoverImage(),
				AppStart.AdminUserId, article.getKeyWord(),
				article.getDescription(), 1, article.getCreateTime(),
				article.getCreateTime(), article.getCompany() };
		int id = DBHelper.InsertAndRetId("Article", sqlString, objects);
		return id;
	}

	/**
	 * 是否存在记录
	 * 
	 * @param article
	 * @return true 存在；false 不存在
	 */
	public boolean IsExist(Article article) {

		String sqlString = "select id from article where url=?  ";
		Object[] objects = new Object[] { article.getUrlString() };
		boolean flag = DBHelper.isExist(sqlString, objects);
		return flag;
	}
}
