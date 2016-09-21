package com.lang.autohome;

import java.util.List;

import us.codecraft.webmagic.Page;

import com.lang.interfac.MotorXPathInterface;
import com.lang.util.TUtil;

public class AutoHomeXPathCommon implements MotorXPathInterface {
	/**
	 * 获取title
	 * 
	 * @param page
	 * @return
	 */
	public String getTitleString(Page page) {
		String titleString = "//div[@id='articlewrap']/h1/text()";
		return page.getHtml().xpath(titleString).toString();
	}

	/**
	 * 获取首张图片
	 * 
	 * @param page
	 * @return
	 */
	public String getFirstImg(Page page) {
		String firstImgString = "//div[@id='articleContent']/p//img/@src";
		String firstImg = "";
		List<String> arrStrings = page.getHtml().xpath(firstImgString).all();
		if (arrStrings == null || arrStrings.size() == 0) { // 格式2
			String firstImgString2 = "//div[@id='articleContent']/p/img/@src";
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
	public String getKeyWordString(Page page) {
		String keyWordString = "//meta[@name='keywords']@content";
		return page.getHtml().xpath(keyWordString).toString();
	}

	/**
	 * description 简介
	 * 
	 * @param page
	 * @return
	 */
	public String getDescription(Page page) {
		String descriptionString = "//meta[@name='description']/@content";
		return page.getHtml().xpath(descriptionString).toString();
	}

	/**
	 * 获取正文内容
	 * 
	 * @param page
	 * @return
	 */
	public String getContentString(Page page) {
		String contentString = "//div[@id='articleContent']/html()";
		return page.getHtml()// con_weibo
				.xpath(contentString).toString();
	}

	/**
	 * 获取文章发布时间
	 * 
	 * @param page
	 * @return
	 */
	public String getPublishTime(Page page) {
		String timeString = "//div[@id='articlewrap']/div[@class='article-info']/span/text()";
		String timeStr = page.getHtml().xpath(timeString).toString();
		if (timeStr != "" && timeStr.length() > 0) {
			// 2016年08月30日 00:05 格式转换 yyyy-MM-dd hh:MM:ss
			return TUtil.strToFormatStr(timeStr.trim());
		}
		return "";
	}

	/**
	 * 判断文章是否有分页
	 * 
	 * @param page
	 * @return
	 */
	public boolean isPagination(Page page) {
		String pagingContent = "//div[@id='articlewrap']/div[@class='page']/html()";
		String pagingStr = page.getHtml().xpath(pagingContent).toString();
		if (pagingStr != null && pagingStr != "" && pagingStr.length() > 0) {
			return true;
		}
		return false;
	}

	/**
	 * 获取url。对url作一些处理。去除锚点等
	 * 
	 * @param page
	 * @return
	 */
	public String getUrl(Page page) {
		String url = page.getUrl().toString();
		if (url.contains("#")) {
			// http://news.bitauto.com/etaqzypzcs/20150528/2206534159.html#comment
			url = url.substring(0, url.indexOf('#'));
		}
		if (url.contains("?pvareaid")) {
			// http://www.autohome.com.cn/tuning/201508/877895-all.html?pvareaid=102337
			url = url.substring(0, url.indexOf("?pvareaid"));
		}
		return url;
	}

	@Override
	public String getPageKey(Page page) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public String getPageIndex(Page page) {
		String reg = "http://www.autohome.com.cn/\\w+/\\d+/\\w+-(\\d+)\\.html\\S*";
		String key = page.getUrl().toString().replaceAll(reg, "$1");
		if (key == null || key == "" || key == page.getUrl().toString()) {
			return "";
		}
		return key;
	}

	@Override
	public List<String> getAllPageUrls(Page page) {
		// TODO Auto-generated method stub
		return null;
	}
}
