package com.lang.common;

import com.lang.util.DBHelper;
import com.lang.util.TUtil;

public class ArticleDao {

	public boolean NewArticle(Article article) {
		if (IsExist(article)) {
			return true;
		} else {
			return Add(article);
		}
	}

	private boolean Add(Article article) {
		String timeString = TUtil.getCurrentTime();
		String sqlString = "INSERT INTO Article"
				+ "(ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime,Company) "
				+ "values (?,?,?,?,?,?,?,?,?,?)";
		Object[] objects = new Object[] { article.getTitle(),
				article.getUrlString(), article.getCoverImage(),
				2, article.getKeyWord(),
				article.getDescription(), 1, timeString, timeString,
				article.getCompany() };
		boolean flag = DBHelper.executeNonQuery(sqlString, objects) > 0;
		return flag;
	}

	private boolean IsExist(Article article) {

		String sqlString = "select id from article where url=?  ";
		Object[] objects = new Object[] { article.getUrlString() };
		boolean flag = DBHelper.isExist(sqlString, objects);
		return flag;
	}
}
