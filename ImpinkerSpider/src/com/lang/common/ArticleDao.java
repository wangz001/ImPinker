package com.lang.common;

import com.lang.util.DBHelper;
import com.lang.util.TUtil;

public class ArticleDao {

	public boolean NewArticle(Article article) {

		String timeString = TUtil.getCurrentTime();
		String sqlString = "INSERT INTO Article" +
				"(ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime,Company) " +
				"values (?,?,?,?,?,?,?,?,?,?)";
		Object[] objects = new Object[] { article.getTitle(), article.getUrlString(),
				"",2,article.getKeyWord(),article.getDescription(),1 ,timeString,timeString,article.getCompany() };
		boolean flag = DBHelper.executeNonQuery(sqlString, objects) > 0;
		return flag;
	}
	
	
}
