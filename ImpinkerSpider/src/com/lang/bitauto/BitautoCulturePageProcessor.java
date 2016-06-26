package com.lang.bitauto;

import java.util.List;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

public class BitautoCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static BitautoPipeline bitautoPipeline = new BitautoPipeline();

	public static void main(String[] args) {
		Spider.create(new BitautoCulturePageProcessor())
				.addUrl("http://www.bitauto.com/pingce/")
				.addPipeline(bitautoPipeline).thread(1).run();
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://www.autohome.com.cn/culture/\\S+)").all());
		String titleString = page.getHtml()
				.xpath("//div[@id='articlewrap']/h1/text()")
				.toString();
		if (titleString != null && titleString.length() > 0) {
			String keyword = page.getHtml()
					.xpath("//meta[@name='keywords']@content").toString();
			String firstImg = "";
			List<String> arrStrings = page
					.getHtml()
					.xpath("//div[@id='articleContent']/p/a/img/@src")
					.all();
			if (arrStrings != null && arrStrings.size() > 0) {
				firstImg = arrStrings.get(0);
			}
			page.putField("url", page.getUrl());
			page.putField("title", titleString);
			page.putField("description", titleString);
			page.putField("keyword", keyword);
			page.putField("CoverImage", firstImg);
		} else {
			page.setSkip(true);
		}
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

}
