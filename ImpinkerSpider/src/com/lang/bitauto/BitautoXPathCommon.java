package com.lang.bitauto;

import java.util.List;

import us.codecraft.webmagic.Page;

import com.lang.impinker.model.ArticleTypeEnum;
import com.lang.impinker.model.CompanyEnum;
import com.lang.interfac.MotorXPathInterface;
import com.lang.util.RegexUtil;

public class BitautoXPathCommon implements MotorXPathInterface {
	/**
	 * 获取title
	 * 
	 * @param page
	 * @return
	 */
	public String getTitleString(Page page) {
		String titleString = "//div[@id='content_bit']/div/article/h1/span[@class='yuanchuang']/text()";
		return page.getHtml().xpath(titleString).toString();
	}

	/**
	 * 获取首张图片
	 * 
	 * @param page
	 * @return
	 */
	public String getFirstImg(Page page) {
		String firstImgString = "//div[@class='article-contents']/p//img/@src";
		String firstImg = "";
		List<String> arrStrings = page.getHtml().xpath(firstImgString).all();
		if (arrStrings == null || arrStrings.size() == 0) { // 格式2
			String firstImgString2 = "//div[@class='article-contents']/p/span/img/@src";
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
	public String getPublishTime(Page page) {
		String timeString = "//span[@id='time']/text()";
		String timeStr = page.getHtml().xpath(timeString).toString();
		if (timeStr != "" && timeStr.length() > 0) {
			// 2016-08-30 格式转换 yyyy-MM-dd hh:MM:ss
			return timeStr.trim() + " 00:00:00";
		}
		return "";
	}

	public boolean isPagination(Page page) {
		String pagingContent = "//div[@id='content_bit']/div[@class='con_main']/div[@class='the_pages']/html()";
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
		return url;
	}

	@Override
	public String getPageKey(Page page) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public String getPageIndex(Page page) {
		String reg = "http://news.bitauto.com/\\w+/\\d+/\\w+-(\\d+)\\.html";
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

	@Override
	public String getTypeByUrl(String urlStr) {
		// TODO Auto-generated method stub
		return null;
	}
}
