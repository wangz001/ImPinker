package com.lang.bitauto.pageprocessor;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.apache.commons.lang.StringUtils;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.factory.XPathFactory;
import com.lang.impinker.model.CompanyEnum;
import com.lang.interfac.MotorXPathInterface;
import com.lang.util.HtmlTagUtil;
import com.lang.util.JcSegUtil;

public class BitautoNewsPageProcessor implements PageProcessor {
	private MotorXPathInterface yicheXPath = new XPathFactory()
			.createXPath(CompanyEnum.Yiche);

	@Override
	public void process(Page page) {
		List<String> bitautoLinks = page.getHtml().links()
				.regex("http://news.bitauto.com/\\S+").all();
		List<String> pingceLinks = page.getHtml().links()
				.regex("http://www.bitauto.com/pingce/\\S+").all();
		page.addTargetRequests(pingceLinks);
		page.addTargetRequests(bitautoLinks);
		boolean isPagination = yicheXPath.isPagination(page);
		if (isPagination) {
			// 有分页
			String allUrl = "";
			String pageIndex = yicheXPath.getPageIndex(page);
			if ("" == pageIndex) {
				allUrl = page.getUrl().toString().replace(".html", "_all.html");
			} else {
				allUrl = page.getUrl().toString()
						.replace("-" + pageIndex + ".html", "_all.html");
			}

			page.addTargetRequests(Arrays.asList(allUrl));
			page.setSkip(true);
			return;
		}
		String titleString = yicheXPath.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String urlStr = yicheXPath.getUrl(page);
			String keyWord = yicheXPath.getKeyWordString(page);
			String firstImg = yicheXPath.getFirstImg(page);
			String description = yicheXPath.getDescription(page);
			String content = yicheXPath.getContentString(page);
			String publishTime = yicheXPath.getPublishTime(page);
			String articleTypeStr = yicheXPath.getTypeByUrl(urlStr);

			content = HtmlTagUtil.delHrefTag(content);// 去除a标签
			String tempContent = HtmlTagUtil.delHTMLTag(content);// 去除html标签
			tempContent = keyWord + description + tempContent;
			List<String> JcKeyWords = JcSegUtil.GetKeyWords(tempContent);
			List<String> JcPhrases = JcSegUtil.GetKeyphrase(tempContent);
			String JcSummary = JcSegUtil.GetSummary(tempContent, 80);

			List<String> keywords = new ArrayList<String>();
			keywords.add(articleTypeStr);
			keywords.addAll(JcKeyWords);
			keywords.addAll(JcPhrases);
			String jckeywordsStr = StringUtils.join(keywords, ",");

			page.putField("url", urlStr);
			page.putField("title", titleString);
			page.putField("keyword", jckeywordsStr);
			page.putField("description", JcSummary);
			page.putField("publishtime", publishTime);
			page.putField("snapCoverImage", firstImg);
			page.putField("snapKeyWords", keyWord);
			page.putField("snapDescription", description);
			page.putField("snapContent", content);
		} else {
			page.setSkip(true);
		}
	}

	@Override
	public Site getSite() {
		return null;
	}
}
