package com.lang.common;

import java.sql.ResultSet;
import java.sql.SQLException;

import com.lang.util.DBHelper;
import com.lang.util.TUtil;

public class ArticleDao {

	/**
	 * 向article表添加记录
	 * 
	 * @param article
	 * @return
	 */
	public long Add(Article article) {
		String sqlString = "INSERT INTO Article"
				+ "(ArticleName,Url,UserId,KeyWords,Description,State,CreateTime,UpdateTime,PublishTime,Company) "
				+ "values (?,?,?,?,?,?,?,?,?,?)";
		Object[] objects = new Object[] { article.getTitle(), article.getUrl(),
				AppStart.AdminUserId, article.getKeyWord(),
				article.getDescription(), 1, TUtil.getCurrentTime(),
				TUtil.getCurrentTime(), article.getPublishTime(),
				article.getCompany() };
		int id = DBHelper.InsertAndRetId("Article", sqlString, objects);
		if (id > 0) {
			article.setId(id);
			AddToArticleSnap(article);
		}
		return id;
	}

	/**
	 * 向articlesanp表插入数据
	 * 
	 * @param article
	 * @return
	 */
	private boolean AddToArticleSnap(Article article) {
		String sqlString = "INSERT INTO ArticleSnaps (ArticleId ,FirstImageUrl ,KeyWords ,Description ,ConTent,CreateTime) "
				+ " values (?,?,?,?,?,?) ";
		Object[] objects = new Object[] { article.getId(),
				article.getSnapFirstImageUrl(), article.getSnapKeyWords(),
				article.getSnapDescription(), article.getSnapContent(),
				TUtil.getCurrentTime() };
		int id = DBHelper.executeNonQuery(sqlString, objects);
		return id > 0;
	}

	/**
	 * 是否存在记录
	 * 
	 * @param article
	 * @return true 存在；false 不存在
	 */
	public boolean IsExist(Article article) {

		String sqlString = "select id from article where url=?  ";
		Object[] objects = new Object[] { article.getUrl() };
		boolean flag = DBHelper.isExist(sqlString, objects);
		return flag;
	}

	public int GetIdByUrl(String Url) {
		String sqlString = "select id from article where url=?  ";
		Object[] objects = new Object[] { Url };
		ResultSet rs = DBHelper.executeQuery(sqlString, objects);
		if (rs != null) {
			try {
				while (rs.next()) {
					int id = rs.getInt("id");
					return id;
				}
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		return 0;
	}

	public ResultSet GetAllUrlAndId() {
		String sqlString = "SELECT id,url FROM dbo.Article";
		ResultSet rs = DBHelper.executeQuery(sqlString);
		return rs;
	}
}
