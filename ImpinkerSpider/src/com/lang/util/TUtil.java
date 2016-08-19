package com.lang.util;

import java.text.SimpleDateFormat;
import java.util.Date;

import org.apache.solr.common.util.DateUtil;

/**
 * @author wangzheng1
 *
 */
public class TUtil {

	public static String getCurrentTime(){
		SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		return format.format(new Date());
	}
	
	public static String getTime(long time) {
		SimpleDateFormat format = new SimpleDateFormat("yy-MM-dd HH:mm");
		return format.format(new Date(time));
	}

	public static String getHourAndMin(long time) {
		SimpleDateFormat format = new SimpleDateFormat("HH:mm");
		return format.format(new Date(time));
	}

	/**
	 * 获取格林尼治标准时间格式
	 * @param time
	 * @return
	 */
    public static String getUTCTime(long time){
    	return DateUtil.getThreadLocalDateFormat().format((time));
    }
}
