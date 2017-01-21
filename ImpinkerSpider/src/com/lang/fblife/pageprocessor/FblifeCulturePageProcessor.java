package com.lang.fblife.pageprocessor;

import java.util.ArrayList;
import java.util.List;

import net.sf.json.JSONObject;

import org.apache.commons.lang.StringUtils;
import org.apache.log4j.Logger;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.factory.XPathFactory;
import com.lang.fblife.FblifePipeline;
import com.lang.impinker.model.CompanyEnum;
import com.lang.interfac.MotorXPathInterface;
import com.lang.util.HtmlTagUtil;
import com.lang.util.HttpUtil;
import com.lang.util.JcSegUtil;

/**
 * 文化、改装、新闻、旅行、评测。使用该解析
 * 
 * @author Administrator
 * 
 */
public class FblifeCulturePageProcessor implements PageProcessor {
	private static Logger logger = Logger
			.getLogger(FblifeCulturePageProcessor.class);
	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline = new FblifePipeline();
	private MotorXPathInterface fbXPath = new XPathFactory()
			.createXPath(CompanyEnum.Fblife);

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		boolean isPagination = fbXPath.isPagination(page);
		/*
		 * 2016-11-15 分页的通过接口请求数据 if (isPagination) { // 有分页的，单独处理 String
		 * pageKey = fbXPath.getPageKey(page); String pageIndex =
		 * fbXPath.getPageIndex(page); List<String> allPageUrls =
		 * fbXPath.getAllPageUrls(page);
		 * 
		 * MutilePageModel mutilePageModel = new MutilePageModel();
		 * mutilePageModel.setPageKey(pageKey);
		 * mutilePageModel.setPageindex(pageIndex);
		 * mutilePageModel.setOtherPages(allPageUrls);
		 * mutilePageModel.setPage(page);
		 * 
		 * MutilePageUtil.getInstance().AddMutilPage(mutilePageModel,
		 * CompanyEnum.Fblife); page.setSkip(true); return; }
		 */
		String titleString = fbXPath.getTitleString(page);
		if (titleString != null && titleString.length() > 0) {
			String urlStr = fbXPath.getUrl(page);
			//去掉分页的的_index.html   的页面，防止重复
			String pageIndex =fbXPath.getPageIndex(page);
			if(pageIndex!=null&&"1"!=pageIndex){
				urlStr=urlStr.replace("_"+pageIndex,"" );
			}
			String firstImg = fbXPath.getFirstImg(page);
			String keyWord = fbXPath.getKeyWordString(page);
			String description = fbXPath.getDescription(page);
			String content = fbXPath.getContentString(page);
			String publishTime = fbXPath.getPublishTime(page);
			String articleTypeStr = fbXPath.getTypeByUrl(urlStr);
			if (isPagination) {
				String str = GetContentFromApi(page);
				if (str != "") {
					content = str;
				}
			}

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

	/**
	 * fblife 通过请求接口获取文章内容
	 * 
	 * @param page
	 * @return
	 */
	private String GetContentFromApi(Page page) {
		String returnStr = "";
		String api = "{0}/ajax.php?c=news&a=getallpage&id={1}&curpage=0&t=1479212543400";
		String pageKey = fbXPath.getPageKey(page);
		String urlStr = fbXPath.getUrl(page);
		String root = urlStr.substring(0, urlStr.indexOf("fblife.com/") + 10);
		String url = api.replace("{0}", root).replace("{1}", pageKey);
		String result = HttpUtil.httpGet(url);
		if (result != null && result != "") {
			JSONObject dataOfJson = JSONObject.fromObject(result);
			if (dataOfJson.getInt("error") != 0) {
				return null;
			}
			String content = dataOfJson.getString("content");
			returnStr = content;
			return returnStr;
		}
		return "";
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

}
