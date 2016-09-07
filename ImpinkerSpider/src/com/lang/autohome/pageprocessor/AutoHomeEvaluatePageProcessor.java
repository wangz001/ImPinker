package com.lang.autohome.pageprocessor;

import java.util.Arrays;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.AutoHomeXPathCommon;
import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;

public class AutoHomeEvaluatePageProcessor implements PageProcessor {

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void process(Page page) {
		page.addTargetRequests(page.getHtml().links()
				.regex("http://www.autohome.com.cn/drive/\\S+").all());
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
			page.putField("keyword", keyWord + ArticleTypeEnum.PingCe.getName()
					+ "," + CompanyEnum.Autohome.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
		} else {
			page.setSkip(true);
		}

	}

}