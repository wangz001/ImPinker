package com.lang.properties;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.Properties;

import org.apache.log4j.Logger;

public class AppProperties {

	static Logger logger = Logger.getLogger(AppProperties.class);
	static String fileName = "app.properties";
	static Properties prop;
	static FileInputStream fis = null;
	/**
	 * 静态块
	 */
	static {
		prop = new Properties();
		try {
			fis = new FileInputStream("app.properties");
		} catch (FileNotFoundException e) {
			e.printStackTrace();
			logger.error(e);
		}
		try {
			prop.load(fis);
		} catch (IOException e) {
			logger.error(e);
			e.printStackTrace();
		}
	}

	public static String getPropertyByName(String name) {
		String value = prop.getProperty(name);
		return value;
	}
}
