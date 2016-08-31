package com.lang.bitauto.pageprocessor;

import java.util.List;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.bitauto.BitautoXPathCommon;

public class BitautoNewsPageProcessor implements PageProcessor {

	@Override
	public void process(Page page) {
		List<String> bitautoLinks = page.getHtml().links()
				.regex("http://news.bitauto.com/\\S+").all();
		List<String> pingceLinks = page.getHtml().links()
				.regex("http://www.bitauto.com/pingce/\\S+").all();
		page.addTargetRequests(pingceLinks);
		page.addTargetRequests(bitautoLinks);
		String titleString = BitautoXPathCommon.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String keyword = BitautoXPathCommon.getKeyWordString(page);
			String firstImg = BitautoXPathCommon.getFirstImg(page);
			String description = BitautoXPathCommon.getDescription(page);
			String content = BitautoXPathCommon.getContentString(page);
			String publishTime = BitautoXPathCommon.getPublishTime(page);

			page.putField("url", page.getUrl());
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword", keyword);
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
		} else {
			page.setSkip(true);
		}
	}

	@Override
	public Site getSite() {
		return null;
	}
}
