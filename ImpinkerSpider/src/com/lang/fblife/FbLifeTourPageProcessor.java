package com.lang.fblife;

import java.util.List;

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

	@Override
	public void process(Page page) {
		List<String> relativeUrl = page.getHtml()
				.xpath("//div[@id='f_mtscroll_ul']/ul/li/a/@href").all();
		relativeUrl.addAll(page.getHtml()
				.xpath("//div[@id='zixun_list']/div/div/div/a/@href").all());
		relativeUrl.addAll(page.getHtml()
				.xpath("//div[@class='channelpage']/a/@href").all());
		page.addTargetRequests(relativeUrl);
		String content = page.getHtml()
				.xpath("//div[@id='f_content']/div/div/div/h1/text()")
				.toString();
		String firstImg = "";
		List<String> arrStrings = page
				.getHtml()
				.xpath("//div[@id='con_weibo']/div[@class='testdiv']/p/a/img/@src")
				.all();
		if (arrStrings != null && arrStrings.size() > 0) {
			firstImg = arrStrings.get(0);
		}
		boolean exist = false;
		if (content != null && content.length() > 0) {
			page.putField("url", page.getUrl());
			page.putField("title", content);
			page.putField("description", content);
			page.putField("keyword", content + ArticleTypeEnum.LvXing.getName()
					+ "," + CompanyEnum.Fblife.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
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

	public static void main(String[] args) {

		Spider.create(new FbLifeTourPageProcessor())
				.addUrl("http://tour.fblife.com/")
				.addPipeline(new FblifePipeline()).thread(1).run();
	}

}
