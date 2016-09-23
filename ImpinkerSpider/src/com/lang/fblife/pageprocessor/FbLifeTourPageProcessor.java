package com.lang.fblife.pageprocessor;

import java.util.List;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.ArticleTypeEnum;
import com.lang.common.CompanyEnum;
import com.lang.common.MutilePageModel;
import com.lang.common.MutilePageUtil;
import com.lang.factory.XPathFactory;
import com.lang.interfac.MotorXPathInterface;

/**
 * 旅行
 */
public class FbLifeTourPageProcessor implements PageProcessor {

	private MotorXPathInterface fbXPath = new XPathFactory()
			.createXPath(CompanyEnum.Fblife);

	@Override
	public void process(Page page) {
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://tour.fblife\\.com/html/\\w+/\\w+.html)").all());
		boolean isPagination = fbXPath.isPagination(page);
		if (isPagination) {
			// 有分页的，单独处理
			String pageKey = fbXPath.getPageKey(page);
			String pageIndex = fbXPath.getPageIndex(page);
			List<String> allPageUrls = fbXPath.getAllPageUrls(page);

			MutilePageModel mutilePageModel = new MutilePageModel();
			mutilePageModel.setPageKey(pageKey);
			mutilePageModel.setPageindex(pageIndex);
			mutilePageModel.setOtherPages(allPageUrls);
			mutilePageModel.setPage(page);

			MutilePageUtil.getInstance().AddMutilPage(mutilePageModel,
					CompanyEnum.Fblife);
			page.setSkip(true);
			return;
		}

		String titleString = fbXPath.getTitleString(page);
		boolean exist = false;
		if (titleString != null && titleString.length() > 0) {
			String keyWord = fbXPath.getKeyWordString(page);
			String content = fbXPath.getContentString(page);
			String firstImg = fbXPath.getFirstImg(page);
			String description = fbXPath.getDescription(page);
			String publishTime = fbXPath.getPublishTime(page);
			page.putField("url", fbXPath.getUrl(page));
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
