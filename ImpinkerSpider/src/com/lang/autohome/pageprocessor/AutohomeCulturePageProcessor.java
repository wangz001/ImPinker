package com.lang.autohome.pageprocessor;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

import org.apache.commons.lang.StringUtils;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.AutohomePipeline;
import com.lang.factory.XPathFactory;
import com.lang.impinker.model.CompanyEnum;
import com.lang.interfac.MotorXPathInterface;
import com.lang.util.HtmlTagUtil;
import com.lang.util.JcSegUtil;

/**
 * 文化、改装、新闻、评测。使用该解析
 * 
 * @author Administrator
 * 
 */
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
			String urlStr = autohomeXPath.getUrl(page);
			String keyWord = autohomeXPath.getKeyWordString(page);
			String firstImg = autohomeXPath.getFirstImg(page);
			String description = autohomeXPath.getDescription(page);
			String content = autohomeXPath.getContentString(page);
			String publishTime = autohomeXPath.getPublishTime(page);
			String articleTypeStr = autohomeXPath.getTypeByUrl(urlStr);

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
		// TODO Auto-generated method stub
		return site;
	}

}
