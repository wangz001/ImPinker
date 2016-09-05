package com.lang.fblife.pageprocessor;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.common.SolrJUtil;
import com.lang.fblife.FbLifeXPathCommon;
import com.lang.fblife.FblifePipeline;

public class FblifeCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline = new FblifePipeline();

	public static void main(String[] args) {
		Spider.create(new FblifeCulturePageProcessor())
				.addUrl("http://www.fblife.com/").addPipeline(fbPipeline)
				.thread(3).run();
		SolrJUtil.getInstance().LastCommit();
		System.out.println("spider stop success!!");
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://culture.fblife\\.com/html/\\w+/\\w+.html)")
				.all());

		String titleString = FbLifeXPathCommon.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String firstImg = FbLifeXPathCommon.getFirstImg(page);
			String keyWord = FbLifeXPathCommon.getKeyWordString(page);
			String description = FbLifeXPathCommon.getDescription(page);
			String content = FbLifeXPathCommon.getContentString(page);
			String publishTime = FbLifeXPathCommon.getPublishTime(page);

			page.putField("url", page.getUrl());
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword", keyWord + ArticleTypeEnum.WenHua.getName()
					+ "," + CompanyEnum.Fblife.getName());
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
