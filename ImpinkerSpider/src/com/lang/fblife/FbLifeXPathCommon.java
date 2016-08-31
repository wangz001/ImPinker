package com.lang.fblife;

import java.util.List;

import us.codecraft.webmagic.Page;

/**
 * xpath 公共类，fblife
 * 
 * @author wangzheng1
 * 
 */
public class FbLifeXPathCommon {

	/**
	 * 获取title
	 * 
	 * @param page
	 * @return
	 */
	public static String getTitleString(Page page) {
		String titleString = "//div[@class='content']/div/div/div[@class='tit']/h1/text()";
		return page.getHtml().xpath(titleString).toString();
	}

	/**
	 * 获取首张图片
	 * 
	 * @param page
	 * @return
	 */
	public static String getFirstImg(Page page) {
		String firstImgString = "//div[@id='con_weibo']/div[@class='testdiv']/p/a/img/@src";
		String firstImg = "";
		List<String> arrStrings = page.getHtml().xpath(firstImgString).all();
		if (arrStrings == null || arrStrings.size() == 0) { // 格式2
			String firstImgString2 = "//div[@id='con_weibo']/div[@class='testdiv']/p/img/@src";
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
		return page.getHtml().xpath(keyWordString).toString();
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
		String contentString = "//div[@id='con_weibo']/html()";
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
		String timeString = "//div[@class='tit']/div[@class='tit_xia']/p/span/text()";
		return page.getHtml().xpath(timeString).toString();
	}
}
