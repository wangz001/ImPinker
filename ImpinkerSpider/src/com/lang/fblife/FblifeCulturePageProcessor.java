package com.lang.fblife;
import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

public class FblifeCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline=new FblifePipeline();
	
	public static void main(String[] args){
		Spider.create(new FblifeCulturePageProcessor())
		.addUrl("http://www.fblife.com/")
		.addPipeline(fbPipeline)
		.thread(1).run();
	}
	
	
	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links().regex("(http://culture.fblife\\.com/html/\\w+/\\w+.html)").all());
		
		String titleString=page.getHtml().xpath("//div[@class='content']/div/div/div[@class='tit']/h1/text()").toString();
        if(titleString!=null&&titleString.length()>0){
        	page.putField("url",page.getUrl());
            page.putField("title",titleString);
            page.putField("description",titleString);
            page.putField("keyword",titleString);
        }else {
        	page.setSkip(true);
		}
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

	
	
}
