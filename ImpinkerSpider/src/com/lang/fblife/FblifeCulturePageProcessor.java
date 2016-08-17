package com.lang.fblife;

import java.util.List;
import com.lang.common.Article;
import com.lang.common.SolrJUtil;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

public class FblifeCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline = new FblifePipeline();

	public static void main(String[] args) {
//		Article article=new Article();
//		article.setId(1);
//		article.setTitle("测试solrj插入");
//		article.setContent("负责蒙方地接的车辆是一台黑色的LC200，开车的人名字叫吉雅，他的车辆明显做了改装升高，前后都更换了FOX的减震器，装的轮胎型号是33吋+ 级别的255／85R16的百路驰MUT，这");
//		new SolrJUtil().AddDocs(article);
//		
		
		Spider.create(new FblifeCulturePageProcessor())
				.addUrl("http://www.fblife.com/").addPipeline(fbPipeline)
				.thread(1).run();
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links()
				.regex("(http://culture.fblife\\.com/html/\\w+/\\w+.html)")
				.all());

		String titleString = page
				.getHtml()
				.xpath("//div[@class='content']/div/div/div[@class='tit']/h1/text()")
				.toString();
		if (titleString != null && titleString.length() > 0) {
			String firstImg = "";
			List<String> arrStrings = page
					.getHtml()
					.xpath("//div[@id='con_weibo']/div[@class='testdiv']/p/a/img/@src")
					.all();
			if (arrStrings != null && arrStrings.size() > 0) {
				firstImg = arrStrings.get(0);
			}
			String keyWord=page
					.getHtml()
					.xpath("//meta[@name='keywords']/@content")
					.toString();
			String content=page
					.getHtml()//con_weibo
					.xpath("//div[@id='con_weibo']/html()")
				.toString();
			page.putField("url", page.getUrl());
			page.putField("title", titleString);
			page.putField("description", titleString);
			page.putField("keyword", keyWord);
			page.putField("CoverImage", firstImg);
			page.putField("Content", content);
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
