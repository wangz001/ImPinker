package com.lang.impinker.dal;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import com.lang.common.AppStart;
import com.lang.impinker.model.Article;
import com.lang.util.DBHelper;
import com.lang.util.TUtil;

public class ArticleDao {

	/**
	 * 事务插入操作，向article和articlesnap两个表插入数据
	 * 
	 * @param article
	 * @return
	 */
	public long AddArticleWithSnap(Article article) {
		int result = 0;
		String sqlString = "INSERT INTO Article"
				+ "(ArticleName,Url,UserId,KeyWords,Description,State,CreateTime,UpdateTime,PublishTime,Company) "
				+ "values (?,?,?,?,?,?,?,?,?,?)";
		Object[] objects = new Object[] { article.getTitle(), article.getUrl(),
				AppStart.AdminUserId, article.getKeyWord(),
				article.getDescription(), 1, TUtil.getCurrentTime(),
				TUtil.getCurrentTime(), article.getPublishTime(),
				article.getCompany() };

		String sqlStringSnap = "INSERT INTO ArticleSnaps (ArticleId ,FirstImageUrl ,KeyWords ,Description ,ConTent,CreateTime) "
				+ " values (?,?,?,?,?,?) ";

		Connection conn = null;
		PreparedStatement pstmt = null;
		try {
			conn = DBHelper.getConnection();
			conn.setAutoCommit(false);
			pstmt = conn.prepareStatement(sqlString);
			for (int i = 0; i < objects.length; i++) {
				pstmt.setObject(i + 1, objects[i]);
			}
			int flag = pstmt.executeUpdate();
			if (flag > 0) {
				pstmt = conn
						.prepareStatement("SELECT IDENT_CURRENT('Article')");
				ResultSet rs = pstmt.executeQuery();
				while (rs.next()) {
					result = rs.getInt(1);
				}
			}
			pstmt = conn.prepareStatement(sqlStringSnap);
			Object[] objectsSnap = new Object[] { result,
					article.getSnapFirstImageUrl(), article.getSnapKeyWords(),
					article.getSnapDescription(), article.getSnapContent(),
					TUtil.getCurrentTime() };
			for (int i = 0; i < objectsSnap.length; i++) {
				pstmt.setObject(i + 1, objectsSnap[i]);
			}
			int flag2 = pstmt.executeUpdate();
			if (flag2 == 0) {
				result = 0;
			}
			conn.commit();// 提交事务
			conn.setAutoCommit(true);// 恢复JDBC事务的默认提交方式
		} catch (SQLException err) {
			result = 0;
			try {
				conn.rollback(); // 进行事务回滚
			} catch (SQLException ex) {
				ex.printStackTrace();
			}
			err.printStackTrace();
			DBHelper.free(null, pstmt, conn);

		} finally {
			DBHelper.free(null, pstmt, conn);
		}
		return result;
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
