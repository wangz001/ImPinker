package com.lang.fblife.pageprocessor;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.fblife.FbLifeXPathCommon;

/**
 * 旅行
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
			page.putField("url", FbLifeXPathCommon.getUrl(page));
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
		return null;
	}
}
