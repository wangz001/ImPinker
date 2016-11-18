package com.lang.util;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;

import org.apache.log4j.Logger;

import com.lang.fblife.pageprocessor.FblifeCulturePageProcessor;

public class HttpUtil {
	private static Logger logger = Logger
			.getLogger(FblifeCulturePageProcessor.class);

	/**
	 * 发送get请求
	 * 
	 * @param url
	 *            路径
	 * @return
	 */
	public static String httpGet(String urlStr) {
		// get请求返回结果
		StringBuffer document = new StringBuffer();
		try {
			// 链接URL
			URL url = new URL(urlStr);
			// 创建链接
			URLConnection conn = url.openConnection();
			// 读取返回结果集
			BufferedReader reader = new BufferedReader(new InputStreamReader(
					conn.getInputStream(), "utf-8"));
			String line = null;
			while ((line = reader.readLine()) != null) {
				document.append(line);
			}
			reader.close();
		} catch (IOException e) {
			logger.error("get请求提交失败:" + urlStr, e);
		}
		return document.toString();
	}

}
