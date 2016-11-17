package com.lang.fblife.pageprocessor;

import java.io.IOException;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.List;

import org.apache.commons.lang.StringUtils;
import org.apache.http.HttpResponse;
import org.apache.http.HttpStatus;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.apache.log4j.Logger;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.common.CompanyEnum;
import com.lang.factory.XPathFactory;
import com.lang.fblife.FblifePipeline;
import com.lang.interfac.MotorXPathInterface;
import com.lang.util.HtmlTagUtil;
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
			String firstImg = fbXPath.getFirstImg(page);
			String keyWord = fbXPath.getKeyWordString(page);
			String description = fbXPath.getDescription(page);
			String content = fbXPath.getContentString(page);
			if (isPagination) {
				String str = GetContentFromApi(page);
				if (str != "") {
					// str = unicode2String(str);// fblife接口需要转换
					content = str;

				}
			}
			content = HtmlTagUtil.delHrefTag(content);// 去除a标签

			String publishTime = fbXPath.getPublishTime(page);
			String articleTypeStr = fbXPath.getTypeByUrl(urlStr);
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
			page.putField("snapDescription", JcSummary);
			page.putField("snapContent", content);
		} else {
			page.setSkip(true);
		}
	}

	/**
	 * unicode 转字符串
	 */
	public static String unicode2String(String unicode) {

		StringBuffer string = new StringBuffer();

		String[] hex = unicode.split("\\\\u");

		for (int i = 1; i < hex.length; i++) {

			// 转换出每一个代码点
			int data = Integer.parseInt(hex[i], 16);

			// 追加成string
			string.append((char) data);
		}

		return string.toString();
	}

	/**
	 * fblife 通过请求接口获取文章内容
	 * 
	 * @param page
	 * @return
	 */
	private String GetContentFromApi(Page page) {
		String api = "{0}/ajax.php?c=news&a=getallpage&id={1}&curpage=0&t=1479212543400";
		String pageKey = fbXPath.getPageKey(page);
		String urlStr = fbXPath.getUrl(page);
		String root = urlStr.substring(0, urlStr.indexOf("fblife.com/") + 10);
		String url = api.replace("{0}", root).replace("{1}", pageKey);
		String result = httpGet(url);
		if (result != null && result != "") {
			// 暂时处理，后改为json对象处理
			result = result.replace("{\"error\":0,\"content\":\"", "");
			result = result.substring(0, result.length() - 2);// 移除 ”}

			return result;
		}
		return "";
	}

	/**
	 * 发送get请求
	 * 
	 * @param url
	 *            路径
	 * @return
	 */
	public static String httpGet(String url) {
		// get请求返回结果
		String jsonResult = null;
		try {
			DefaultHttpClient client = new DefaultHttpClient();
			// 发送get请求
			HttpGet request = new HttpGet(url);
			HttpResponse response = client.execute(request);

			/** 请求发送成功，并得到响应 **/
			if (response.getStatusLine().getStatusCode() == HttpStatus.SC_OK) {
				/** 读取服务器返回过来的json字符串数据 **/
				jsonResult = EntityUtils.toString(response.getEntity());
				/** 把json字符串转换成json对象 **/
				// jsonResult = JSONObject..fromObject(strResult);
				url = URLDecoder.decode(url, "UTF-8");
			} else {
				logger.error("get请求提交失败:" + url);
			}
		} catch (IOException e) {
			logger.error("get请求提交失败:" + url, e);
		}
		return jsonResult;
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

}
