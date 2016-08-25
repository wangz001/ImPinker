package com.lang.fblife;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;

/**
 * @author 410775541@qq.com <br>
 * @since 0.5.1
 */
public class FbLifeTourPageProcessor implements PageProcessor {

	@Override
	public void process(Page page) {
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://tour.fblife\\.com/html/\\w+/\\w+.html)").all());
		String titleString = FbLifeXPathCommon.getTitleString(page);
		boolean exist = false;
		if (titleString != null && titleString.length() > 0) {
			String keyWord = FbLifeXPathCommon.getKeyWordString(page);
			String content = FbLifeXPathCommon.getContentString(page);
			String firstImg = FbLifeXPathCommon.getFirstImg(page);
			String description = FbLifeXPathCommon.getDescription(page);
			String publishTime = FbLifeXPathCommon.getPublishTime(page);
			page.putField("url", page.getUrl());
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword", keyWord + ArticleTypeEnum.LvXing.getName()
					+ "," + CompanyEnum.Fblife.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
			exist = true;
		}
		if (!exist) {
			page.setSkip(true);
		}
	}

	@Override
	public Site getSite() {
		return site;
	}

	private Site site = Site
			.me()
			.setCycleRetryTimes(5)
			.setRetryTimes(5)
			.setSleepTime(500)
			.setTimeOut(3 * 60 * 1000)
			.setUserAgent(
					"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0")
			.addHeader("Accept",
					"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
			.addHeader("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3")
			.setCharset("gbk");

	public static void main(String[] args) {

		Spider.create(new FbLifeTourPageProcessor())
				.addUrl("http://tour.fblife.com/")
				.addPipeline(new FblifePipeline()).thread(1).run();
	}

}
