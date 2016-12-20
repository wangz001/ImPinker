package com.lang.common;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Hashtable;

import com.lang.impinker.dal.ArticleDao;

/**
 * 数据库中url的缓存
 * 
 * @author wangzheng1
 * 
 */
public class ArticleUrlCache {

	static Hashtable<String, Long> articleUrls = new Hashtable<String, Long>();// Hashtable
																				// 线程安全

	private volatile static ArticleUrlCache articleUrlCache = null;

	/**
	 * 单利模式，获取实例
	 * 
	 * @return
	 */
	public static ArticleUrlCache getInstance() {
		if (articleUrlCache == null) {
			synchronized (ArticleUrlCache.class) { // 加锁，解决多线程使用单例的问题
				articleUrlCache = new ArticleUrlCache();
				initData();
			}
		}
		return articleUrlCache;
	}

	private ArticleUrlCache() {

	}

	private static void initData() {
		ArticleDao articleDao = new ArticleDao();
		ResultSet rs = articleDao.GetAllUrlAndId();
		if (rs != null) {
			try {
				while (rs.next()) {
					long id = rs.getInt("id");
					String url = rs.getString("url");
					if (!articleUrls.containsKey(url)) {
						articleUrls.put(url, id);
					}
				}
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
	}

	/**
	 * 判断数据库中是否存在该url
	 * 
	 * @param url
	 * @return id 对应的id。 0 表示不存在
	 */
	public long GetIdByUrl(String url) {
		if (articleUrls.containsKey(url)) {
			return articleUrls.get(url);
		}
		return 0;
	}

	/**
	 * 添加缓存
	 * 
	 * @param url
	 */
	public void AddUrl(String url, long Id) {
		if (!articleUrls.containsKey(url)) {
			articleUrls.put(url, Id);
		}
	}

}
