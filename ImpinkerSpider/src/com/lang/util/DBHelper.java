package com.lang.util;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

public final class DBHelper {

	DBConnection connection1 = new DBConnection();

	// 此方法为获取数据库连接

	public static Connection getConnection() {

		Connection conn = null;
		DBConnection connection = new DBConnection();
		conn = connection.getConnection();
		System.out.println("数据库连接成功" + conn);
		return conn;
	}

	/**
	 * 
	 * 增删改【Add、Del、Update】
	 * 
	 * 
	 * 
	 * @param sql
	 * 
	 * @return int
	 */

	public static int executeNonQuery(String sql) {

		int result = 0;

		Connection conn = null;

		Statement stmt = null;

		try {

			conn = getConnection();

			stmt = conn.createStatement();

			result = stmt.executeUpdate(sql);

		} catch (SQLException err) {

			err.printStackTrace();

			free(null, stmt, conn);

		} finally {

			free(null, stmt, conn);

		}

		return result;

	}

	/**
	 * 
	 * 增删改【Add、Delete、Update】,带参数
	 * @param sql
	 * @param obj
	 * @return int
	 */

	public static int executeNonQuery(String sql, Object... obj) {

		int result = 0;

		Connection conn = null;

		PreparedStatement pstmt = null;

		try {

			conn = getConnection();

			pstmt = conn.prepareStatement(sql);

			for (int i = 0; i < obj.length; i++) {

				pstmt.setObject(i + 1, obj[i]);

			}

			result = pstmt.executeUpdate();
		} catch (SQLException err) {

			err.printStackTrace();

			free(null, pstmt, conn);

		} finally {

			free(null, pstmt, conn);

		}

		return result;

	}

	/**
	 * 执行插入操作，并返回记录的ID
	 * 
	 * @param sqlStr
	 * @param obj
	 * @return
	 */
	public static int InsertAndRetId(String TableName, String sqlStr,
			Object... obj) {

		int result = 0;

		Connection conn = null;

		PreparedStatement pstmt = null;

		try {

			conn = getConnection();

			pstmt = conn.prepareStatement(sqlStr);

			for (int i = 0; i < obj.length; i++) {

				pstmt.setObject(i + 1, obj[i]);

			}

			int flag = pstmt.executeUpdate();
			if (flag > 0) {
				pstmt = conn.prepareStatement("SELECT IDENT_CURRENT('"
						+ TableName + "')");
				ResultSet rs = pstmt.executeQuery();
				while (rs.next()) {
					result = rs.getInt(1);
				}
			}

		} catch (SQLException err) {

			err.printStackTrace();

			free(null, pstmt, conn);

		} finally {

			free(null, pstmt, conn);

		}
		return result;

	}

	/**
	 * 
	 * 查【Query】
	 * 
	 * 
	 * 
	 * @param sql
	 * 
	 * @return ResultSet
	 */

	public static ResultSet executeQuery(String sql) {

		Connection conn = null;

		Statement stmt = null;

		ResultSet rs = null;

		try {

			conn = getConnection();

			stmt = conn.createStatement();

			rs = stmt.executeQuery(sql);

		} catch (SQLException err) {

			err.printStackTrace();

			free(rs, stmt, conn);

		}

		return rs;

	}

	/**
	 * 
	 * 查【Query】
	 * 
	 * 
	 * 
	 * @param sql
	 * 
	 * @param obj
	 * 
	 * @return ResultSet
	 */

	public static ResultSet executeQuery(String sql, Object... obj) {

		Connection conn = null;

		PreparedStatement pstmt = null;

		ResultSet rs = null;

		try {

			conn = getConnection();
			pstmt = conn.prepareStatement(sql);

			for (int i = 0; i < obj.length; i++) {

				pstmt.setObject(i + 1, obj[i]);

			}

			rs = pstmt.executeQuery();

		} catch (SQLException err) {

			err.printStackTrace();

			free(rs, pstmt, conn);

		}

		return rs;

	}

	/**
	 * 
	 * 判断记录是否存在
	 * 
	 * 
	 * 
	 * @param sql
	 * 
	 * @return Boolean
	 */

	public static Boolean isExist(String sql) {

		ResultSet rs = null;

		try {

			rs = executeQuery(sql);

			rs.last();

			int count = rs.getRow();

			if (count > 0) {

				return true;

			} else {

				return false;

			}

		} catch (SQLException err) {

			err.printStackTrace();

			free(rs);

			return false;

		} finally {

			free(rs);

		}

	}

	/**
	 * 判断记录是否存在
	 * 
	 * @param sql
	 * @return Boolean
	 */

	public static Boolean isExist(String sql, Object... obj) {

		ResultSet rs = null;

		try {

			rs = executeQuery(sql, obj);

			// rs.last();

			// int count = rs.getRow();

			if (rs.next()) {

				return true;

			} else {

				return false;

			}

		} catch (SQLException err) {

			err.printStackTrace();

			free(rs);

			return false;

		} finally {

			free(rs);

		}

	}

	/**
	 * 
	 * 获取查询记录的总行数
	 * 
	 * 
	 * 
	 * @param sql
	 * 
	 * @return int
	 */

	public static int getCount(String sql) {

		int result = 0;

		ResultSet rs = null;

		try {

			rs = executeQuery(sql);

			rs.last();

			result = rs.getRow();

		} catch (SQLException err) {

			free(rs);

			err.printStackTrace();

		} finally {

			free(rs);

		}

		return result;

	}

	/**
	 * 
	 * 获取查询记录的总行数
	 * 
	 * @param sql
	 * @param obj
	 * @return int
	 */

	public static int getCount(String sql, Object... obj) {

		int result = 0;

		ResultSet rs = null;

		try {

			rs = executeQuery(sql, obj);

			rs.last();

			result = rs.getRow();

		} catch (SQLException err) {

			err.printStackTrace();

		} finally {

			free(rs);

		}

		return result;

	}
	
	//针对mysql数据库插入数据并返回ID的方法
	/**
	 * 执行插入操作，并返回记录的ID
	 * 
	 * @param sqlStr
	 * @param obj
	 * @return
	 */
	public static int MysqlInsertAndRetId(String sqlStr,
			Object... obj) {

		int result = 0;

		Connection conn = null;

		PreparedStatement pstmt = null;

		try {

			conn = getConnection();

			pstmt = conn.prepareStatement(sqlStr);

			for (int i = 0; i < obj.length; i++) {

				pstmt.setObject(i + 1, obj[i]);

			}

			int flag = pstmt.executeUpdate();
		    
			if (flag > 0) {
				ResultSet rs = pstmt.executeQuery("SELECT LAST_INSERT_ID()");
				//ResultSet rs = pstmt.getGeneratedKeys();
			    if (rs.next()) {
			    	result = rs.getInt(1);
			    } 
			}

		} catch (SQLException err) {

			err.printStackTrace();

			free(null, pstmt, conn);

		} finally {

			free(null, pstmt, conn);

		}
		return result;

	}

	/**
	 * 
	 * 释放【ResultSet】资源
	 * 
	 * @param rs
	 */

	public static void free(ResultSet rs) {

		try {

			if (rs != null) {

				rs.close();

			}

		} catch (SQLException err) {

			err.printStackTrace();

		}

	}

	/**
	 * 
	 * 释放【Statement】资源
	 * 
	 * 
	 * 
	 * @param st
	 */

	public static void free(Statement st) {

		try {

			if (st != null) {

				st.close();

			}

		} catch (SQLException err) {

			err.printStackTrace();

		}

	}

	/**
	 * 
	 * 释放【Connection】资源
	 * 
	 * 
	 * 
	 * @param conn
	 */

	public static void free(Connection conn) {

		try {

			if (conn != null) {

				conn.close();

			}

		} catch (SQLException err) {

			err.printStackTrace();

		}

	}

	/**
	 * 
	 * 释放所有数据资源
	 * 
	 * 
	 * 
	 * @param rs
	 * 
	 * @param st
	 * 
	 * @param conn
	 */

	public static void free(ResultSet rs, Statement st, Connection conn) {

		free(rs);

		free(st);

		free(conn);

	}

}
