package com.lang.util;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

import com.lang.properties.AppProperties;

public class DBConnection {
	// Sqlserver
	public static final String DBDRIVER = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
	public static final String DBURL = AppProperties.getPropertyByName("dburl");
	public static final String DBUSER = AppProperties.getPropertyByName("dbuser");
	public static final String DBPASS =AppProperties.getPropertyByName("dbpath");
	// 本地mysql---------------------
	/*public static final String DBDRIVER = "com.mysql.jdbc.Driver";
	public static final String DBURL = "jdbc:mysql://127.0.0.1:3306/newpinker";
	public static final String DBUSER = "root";
	public static final String DBPASS = "admin";*/
	
	
	// MySql-----------
//	 public static final String DBDRIVER = "com.mysql.jdbc.Driver";
//	 String databaseName = ApplicationHelper.BAIDU_DB_NAME;
//	 String host = "sqld.duapp.com";
//	 String port = "4050";
//	 String dbUrl = "jdbc:mysql://";
//	 String serverName = host + ":" + port + "/";
//	 String connName = dbUrl + serverName + databaseName;
//	 public final String DBURL = dbUrl + serverName + databaseName;
//	 public static final String DBUSER = ApplicationHelper.API_KEY;
//	 public static final String DBPASS =ApplicationHelper.SECRET_KEY ;
	
	

	private Connection conn = null;

	public DBConnection() { // 在构造方法中进行数据库连接
		try {
			Class.forName(DBDRIVER); // 加载驱动程序
			conn = DriverManager.getConnection(DBURL, DBUSER, DBPASS);
		} catch (ClassNotFoundException e) {

			System.out.println("Sorry,can't find the Driver!");

			e.printStackTrace();

		} catch (SQLException e) {

			e.printStackTrace();

		} catch (Exception e) {

			e.printStackTrace();

		}
	}

	public Connection getConnection() { // 取得数据库连接
		System.out.println("数据库连接成功！");
		return this.conn;
	}

	public void close() {
		if (this.conn != null) { // 数据库关闭操作，避免空指针异常。
			try {
				this.conn.close();
			} catch (Exception e) {
			}
		}
	}
}
