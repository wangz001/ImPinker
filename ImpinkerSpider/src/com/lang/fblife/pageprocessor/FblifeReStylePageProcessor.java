package com.lang.fblife.pageprocessor;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.fblife.FbLifeXPathCommon;

/**
 * 改装 restyle
 * 
 * @author wangzheng1
 * 
 */
public class FblifeReStylePageProcessor implements PageProcessor {

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://restyle.fblife\\.com/html/\\w+/\\w+.html)")
				.all());

		String titleString = FbLifeXPathCommon.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String firstImg = FbLifeXPathCommon.getFirstImg(page);
			String keyWord = FbLifeXPathCommon.getKeyWordString(page);
			String description = FbLifeXPathCommon.getDescription(page);
			String content = FbLifeXPathCommon.getContentString(page);
			String publishTime = FbLifeXPathCommon.getPublishTime(page);

			page.putField("url", FbLifeXPathCommon.getUrl(page));
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword",
					keyWord + ArticleTypeEnum.GaiZhuang.getName() + ","
							+ CompanyEnum.Fblife.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
		} else {
			page.setSkip(true);
		}
	}

}