package com.lang.autohome.pageprocessor;

import java.util.Arrays;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.AutohomePipeline;
import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.factory.XPathFactory;
import com.lang.interfac.MotorXPathInterface;

public class AutohomeCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static AutohomePipeline autohomePipeline = new AutohomePipeline();
	private MotorXPathInterface autohomeXPath = new XPathFactory()
			.createXPath(CompanyEnum.Autohome);

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
		boolean isPagination = autohomeXPath.isPagination(page);
		if (isPagination) {
			// 有分页
			String allUrl = page.getUrl().toString()
					.replace(".html", "-all.html");
			page.addTargetRequests(Arrays.asList(allUrl));
			page.setSkip(true);
			return;
		}
		String titleString = autohomeXPath.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String keyWord = autohomeXPath.getKeyWordString(page);
			String firstImg = autohomeXPath.getFirstImg(page);
			String description = autohomeXPath.getDescription(page);
			String content = autohomeXPath.getContentString(page);
			String publishTime = autohomeXPath.getPublishTime(page);

			page.putField("url", autohomeXPath.getUrl(page));
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
