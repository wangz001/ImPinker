package com.lang.autohome.pageprocessor;

import java.util.Arrays;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.factory.XPathFactory;
import com.lang.interfac.MotorXPathInterface;

/**
 * 新闻
 * 
 * @author wangzheng1
 * 
 */
public class AutoHomeNewsPageProcessor implements PageProcessor {

	private MotorXPathInterface autohomeXPath = new XPathFactory()
			.createXPath(CompanyEnum.Autohome);

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void process(Page page) {
		page.addTargetRequests(page.getHtml().links()
				.regex("http://www.autohome.com.cn/news/\\S+").all());
		boolean isPagination = autohomeXPath.isPagination(page);
		if (isPagination) {
			// 有分页
			String allUrl = "";
			String pageIndex = autohomeXPath.getPageIndex(page);
			if ("" == pageIndex) {
				allUrl = page.getUrl().toString().replace(".html", "-all.html");
			} else {
				allUrl = page.getUrl().toString()
						.replace("-" + pageIndex + ".html", "-all.html");
			}

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
