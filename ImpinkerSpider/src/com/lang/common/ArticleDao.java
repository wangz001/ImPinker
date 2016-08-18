package com.lang.common;

import com.lang.util.DBHelper;
import com.lang.util.TUtil;

public class ArticleDao {

	public boolean NewArticle(Article article) {
		if (IsExist(article)) {
			return true;
		} else {
			return Add(article)>0;
		}
	}

	public long Add(Article article) {
		String timeString = TUtil.getCurrentTime();
		String sqlString = "INSERT INTO Article"
				+ "(ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime,Company,Content) "
				+ "values (?,?,?,?,?,?,?,?,?,?,?)";
		Object[] objects = new Object[] { article.getTitle(),
				article.getUrlString(), article.getCoverImage(),
				AppStart.AdminUserId, article.getKeyWord(),
				article.getDescription(), 1, timeString, timeString,
				article.getCompany(),article.getContent() };
		int id = DBHelper.InsertAndRetId("Article",sqlString, objects);
		return id;
	}

	public boolean IsExist(Article article) {

		String sqlString = "select id from article where url=?  ";
		Object[] objects = new Object[] { article.getUrlString() };
		boolean flag = DBHelper.isExist(sqlString, objects);
		return flag;
	}
}
