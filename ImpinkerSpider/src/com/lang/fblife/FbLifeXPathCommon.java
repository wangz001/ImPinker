package com.lang.fblife;

import java.util.ArrayList;
import java.util.List;

import us.codecraft.webmagic.Page;

import com.lang.impinker.model.ArticleTypeEnum;
import com.lang.interfac.MotorXPathInterface;
import com.lang.util.RegexUtil;

/**
 * xpath 公共类，fblife
 * 
 * @author wangzheng1
 * 
 */
public class FbLifeXPathCommon implements MotorXPathInterface {

	/**
	 * 获取title
	 * 
	 * @param page
	 * @return
	 */
	public String getTitleString(Page page) {
		String titleString = "//div[@class='content']/div/div/div[@class='tit']/h1/text()";
		return page.getHtml().xpath(titleString).toString();
	}

	/**
	 * 获取首张图片
	 * 
	 * @param page
	 * @return
	 */
	public String getFirstImg(Page page) {
		String firstImgString = "//div[@id='con_weibo']/div[@class='testdiv']/p//img/@src";
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
	public String getKeyWordString(Page page) {
		String keyWordString = "//meta[@name='keywords']/@content";
		String str = page.getHtml().xpath(keyWordString).toString();
		if (str.length() > 50) {
			str = str.substring(0, 50);
		}
		return str;
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
	public String getPublishTime(Page page) {
		String timeString = "//div[@class='tit']/div[@class='tit_xia']/p/span/text()";
		return page.getHtml().xpath(timeString).toString();
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

	/**
	 * 判断文章是否有分页
	 * 
	 * @param page
	 * @return
	 */
	public boolean isPagination(Page page) {
		String pagingContent = "//div[@id='page_div']/div[@class='channelpage']/html()";
		String pagingStr = page.getHtml().xpath(pagingContent).toString();
		if (pagingStr != null && pagingStr != "" && pagingStr.length() > 0) {
			return true;
		}
		return false;
	}

	public String getPageKey(Page page) {
		String reg = "http://\\w+\\.fblife\\.com/html/\\w+/(\\d+).*\\.html";
		String key = page.getUrl().toString().replaceAll(reg, "$1");
		return key;
	}

	public String getPageIndex(Page page) {
		String reg = "http://\\w+\\.fblife\\.com/html/\\w+/\\w+_(\\d+)\\.html";
		String key = page.getUrl().toString().replaceAll(reg, "$1");
		if (key == null || key == "" || key == page.getUrl().toString()) {
			return "1";
		}
		return key;
	}

	public List<String> getAllPageUrls(Page page) {
		List<String> list = new ArrayList<String>();
		list.add(page.getUrl().toString());
		String pagingContent = "//div[@id='page_div']/div[@class='channelpage']";
		List<String> test = (List<String>) page.getHtml().xpath(pagingContent)
				.links().all();
		for (String url : test) {
			if (!list.contains(url)
					&& RegexUtil.match(
							"http://\\w+\\.fblife\\.com/html/\\w+/\\w+\\.html",
							url)) {
				list.add(url);
			}
		}
		return list;
	}

	@Override
	public String getTypeByUrl(String urlStr) {
		String reg = "http://(\\w+)\\.fblife\\.com/html/\\w+/\\w+.*\\.html";
		String key = urlStr.replaceAll(reg, "$1");

		if ("culture".equals(key)) {
			return ArticleTypeEnum.WenHua.getName();
		}
		if ("tour".equals(key)) {
			return ArticleTypeEnum.LvXing.getName();
		}
		if ("restyle".equals(key)) {
			return ArticleTypeEnum.GaiZhuang.getName();
		}
		if ("news".equals(key)) {
			return ArticleTypeEnum.XinWen.getName();
		}

		if ("drive".equals(key)) {
			return ArticleTypeEnum.ZiJiaYou.getName();
		}
		return "越野e族";
	}
}
