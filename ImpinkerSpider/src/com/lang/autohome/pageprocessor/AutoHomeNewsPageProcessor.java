package com.lang.autohome.pageprocessor;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.AutoHomeXPathCommon;
import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.fblife.FbLifeXPathCommon;

/**
 * 新闻
 * 
 * @author wangzheng1
 * 
 */
public class AutoHomeNewsPageProcessor implements PageProcessor {

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void process(Page page) {
		page.addTargetRequests(page.getHtml().links()
				.regex("http://www.autohome.com.cn/news/\\S+").all());

		String titleString = AutoHomeXPathCommon.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String keyWord = AutoHomeXPathCommon.getKeyWordString(page);
			String firstImg = AutoHomeXPathCommon.getFirstImg(page);
			String description = FbLifeXPathCommon.getDescription(page);
			String content = FbLifeXPathCommon.getContentString(page);
			String publishTime = FbLifeXPathCommon.getPublishTime(page);

			page.putField("url", page.getUrl());
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword", keyWord + ArticleTypeEnum.XinWen.getName()
					+ "," + CompanyEnum.Autohome.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
		} else {
			page.setSkip(true);
		}

	}

}
