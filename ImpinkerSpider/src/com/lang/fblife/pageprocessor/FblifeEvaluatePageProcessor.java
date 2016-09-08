package com.lang.fblife.pageprocessor;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.factory.XPathFactory;
import com.lang.interfac.MotorXPathInterface;

/**
 * 评测
 * 
 * @author wangzheng1
 * 
 */
public class FblifeEvaluatePageProcessor implements PageProcessor {

	private MotorXPathInterface fbXPath = new XPathFactory()
			.createXPath(CompanyEnum.Fblife);

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void process(Page page) {
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://drive.fblife\\.com/html/\\w+/\\w+.html)").all());

		String titleString = fbXPath.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String firstImg = fbXPath.getFirstImg(page);
			String keyWord = fbXPath.getKeyWordString(page);
			String description = fbXPath.getDescription(page);
			String content = fbXPath.getContentString(page);
			String publishTime = fbXPath.getPublishTime(page);

			page.putField("url", fbXPath.getUrl(page));
			page.putField("title", titleString);
			page.putField("description", description);
			page.putField("keyword", keyWord + ArticleTypeEnum.PingCe.getName()
					+ "," + CompanyEnum.Fblife.getName());
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
			page.putField("publishtime", publishTime);
		} else {
			page.setSkip(true);
		}
	}

}
