package com.lang.bitauto;

import java.util.List;

import us.codecraft.webmagic.Page;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.util.RegexUtil;

public class BitautoXPathCommon {
	/**
	 * 获取title
	 * 
	 * @param page
	 * @return
	 */
	public static String getTitleString(Page page) {
		String titleString = "//div[@id='content_bit']/div/article/h1/span[@class='yuanchuang']/text()";
		return page.getHtml().xpath(titleString).toString();
	}

	/**
	 * 获取首张图片
	 * 
	 * @param page
	 * @return
	 */
	public static String getFirstImg(Page page) {
		String firstImgString = "//div[@class='article-contents']/p/a/img/@src";
		String firstImg = "";
		List<String> arrStrings = page.getHtml().xpath(firstImgString).all();
		if (arrStrings == null || arrStrings.size() == 0) { // 格式2
			String firstImgString2 = "//div[@class='article-contents']/p/img/@src";
			arrStrings = page.getHtml().xpath(firstImgString2).all();
		}
		if (arrStrings != null && arrStrings.size() > 0) {
			firstImg = arrStrings.get(0);
		} else {
			firstImg = "";
		}

		return firstImg;
	}

	/**
	 * 获取关键字
	 * 
	 * @param page
	 * @return
	 */
	public static String getKeyWordString(Page page) {
		String keyWordString = "//meta[@name='keywords']/@content";
		String resultStr = page.getHtml().xpath(keyWordString).toString();
		String articleTypeStr = ArticleTypeEnum.PingCe.getName();
		if (RegexUtil.match("http://news.bitauto.com/drive/\\S+", page.getUrl()
				.toString())) {
			articleTypeStr = ArticleTypeEnum.PingCe.getName();
		}
		resultStr = resultStr + articleTypeStr + ","
				+ CompanyEnum.Yiche.getName();
		return resultStr;
	}

	/**
	 * description 简介
	 * 
	 * @param page
	 * @return
	 */
	public static String getDescription(Page page) {
		String descriptionString = "//meta[@name='description']/@content";
		return page.getHtml().xpath(descriptionString).toString();
	}

	/**
	 * 获取正文内容
	 * 
	 * @param page
	 * @return
	 */
	public static String getContentString(Page page) {
		String contentString = "//div[@class='article-contents]/html()";
		return page.getHtml()// con_weibo
				.xpath(contentString).toString();
	}

	/**
	 * 获取文章发布时间
	 * 
	 * @param page
	 * @return
	 */
	public static String getPublishTime(Page page) {
		String timeString = "//span[@id='time']/text()";
		String timeStr = page.getHtml().xpath(timeString).toString();
		if (timeStr != "" && timeStr.length() > 0) {
			// 2016-08-30 格式转换 yyyy-MM-dd hh:MM:ss
			return timeStr.trim() + " 00:00:00";
		}
		return "";
	}
}
