package com.lang.autohome.pageprocessor;

import java.util.Arrays;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.AutoHomeXPathCommon;
import com.lang.autohome.AutohomePipeline;
import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;

public class AutohomeCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static AutohomePipeline autohomePipeline = new AutohomePipeline();

	public static void main(String[] args) {
		Spider.create(new AutohomeCulturePageProcessor())
				.addUrl("http://www.autohome.com.cn/culture/")
				.addPipeline(autohomePipeline).thread(1).run();
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://www.autohome.com.cn/culture/\\S+)").all());
		boolean isPagination = AutoHomeXPathCommon.isPagination(page);
		if (isPagination) {
			// 有分页
			String allUrl = page.getUrl().toString()
					.replace(".html", "-all.html");
			page.addTargetRequests(Arrays.asList(allUrl));
			page.setSkip(true);
			return;
		}
		String titleString = AutoHomeXPathCommon.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String keyWord = AutoHomeXPathCommon.getKeyWordString(page);
			String firstImg = AutoHomeXPathCommon.getFirstImg(page);
			String description = AutoHomeXPathCommon.getDescription(page);
			String content = AutoHomeXPathCommon.getContentString(page);
			String publishTime = AutoHomeXPathCommon.getPublishTime(page);

			page.putField("url", AutoHomeXPathCommon.getUrl(page));
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword", keyWord + ArticleTypeEnum.WenHua.getName()
					+ "," + CompanyEnum.Autohome.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
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
