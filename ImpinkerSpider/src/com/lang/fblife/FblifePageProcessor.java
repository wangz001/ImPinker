package com.lang.fblife;

import java.util.List;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.fblife.pageprocessor.FbLifeTourPageProcessor;
import com.lang.fblife.pageprocessor.FblifeEvaluatePageProcessor;
import com.lang.fblife.pageprocessor.FblifeNewsPageProcessor;
import com.lang.fblife.pageprocessor.FblifeReStylePageProcessor;
import com.lang.util.RegexUtil;

public class FblifePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline = new FblifePipeline();

	public static void main(String[] args) {
		Spider.create(new FblifePageProcessor())
				.addUrl("http://www.fblife.com/").addPipeline(fbPipeline)
				.thread(3).run();
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		List<String> fbLinks = page.getHtml().links()
				.regex("(http://\\w+.fblife\\.com/html/\\w+/\\w+.html)").all();
		page.addTargetRequests(fbLinks);
		String thisUrlString = page.getUrl().toString();

		if (RegexUtil.match("http://culture.fblife\\.com/html/\\w+/\\w+.html",
				thisUrlString)) {
			new FblifeCulturePageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://tour.fblife\\.com/html/\\w+/\\w+.html", thisUrlString)) {
			new FbLifeTourPageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://restyle.fblife\\.com/html/\\w+/\\w+.html",
				thisUrlString)) {
			new FblifeReStylePageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://news.fblife\\.com/html/\\w+/\\w+.html", thisUrlString)) {
			new FblifeNewsPageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://drive.fblife\\.com/html/\\w+/\\w+.html", thisUrlString)) {
			new FblifeEvaluatePageProcessor().process(page);
			return;
		}

		page.setSkip(true);

		System.out.println(page.getUrl());
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

}
