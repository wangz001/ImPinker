package com.lang.util;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.TimeZone;

import org.apache.solr.common.util.DateUtil;

/**
 * @author wangzheng1
 * 
 */
public class TUtil {

	public static String getCurrentTime() {
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
	 * 
	 * @param time
	 * @return
	 */
	public static String getUTCTime(long time) {
		System.out.print("utc时间："
				+ DateUtil.getThreadLocalDateFormat().format((time)));
		return DateUtil.getThreadLocalDateFormat().format((time));
	}

	/**
	 * 获取北京标准时间格式
	 * 
	 * @return
	 */
	public static String getBJUtcTime() {
		// 当前系统默认时区的时间：
		Calendar calendar = new GregorianCalendar();
		System.out.print("时区：" + calendar.getTimeZone().getID() + "  ");
		System.out.println("时间：" + calendar.get(Calendar.HOUR_OF_DAY) + ":"
				+ calendar.get(Calendar.MINUTE));
		// 1、取得本地时间：
		java.util.Calendar cal = java.util.Calendar.getInstance();

		// 2、取得时间偏移量：
		int zoneOffset = cal.get(java.util.Calendar.ZONE_OFFSET);

		// 3、取得夏令时差：
		int dstOffset = cal.get(java.util.Calendar.DST_OFFSET);

		// 4、从本地时间里扣除这些差量，即可以取得UTC时间：
		cal.add(java.util.Calendar.MILLISECOND, -(zoneOffset + dstOffset));

		// 之后调用cal.get(int x)或cal.getTimeInMillis()方法所取得的时间即是UTC标准时间。
		System.out.println("UTC:" + new Date(cal.getTimeInMillis()));

		Calendar calendar1 = Calendar.getInstance();
		TimeZone tztz = TimeZone.getTimeZone("GMT");
		calendar1.setTimeZone(tztz);
		System.out.println(calendar.getTime());
		System.out.println(calendar.getTimeInMillis());
		return "";
	}
}
