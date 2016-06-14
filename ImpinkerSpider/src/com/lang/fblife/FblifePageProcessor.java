package com.lang.fblife;
import com.lang.main.myWebMagic;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

public class FblifePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline=new FblifePipeline();
	
	public static void main(String[] args){
		Spider.create(new FblifePageProcessor())
		.addUrl("http://tour.fblife.com/")
		.addPipeline(fbPipeline)
		.thread(5).run();
	}
	
	
	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links().regex("(http://tour.fblife\\.com/html/\\w+/\\w+.html)").all());
		page.putField("title", page.getHtml().xpath("//div[@class='content']/div/div/div[@class='tit']/h1/text()").toString());
        page.putField("content", page.getHtml().xpath("//div[@id='con_weibo']/tidyText()").toString());
        //page.putField("tags",page.getHtml().xpath("//div[@class='BlogTags']/a/text()").all());
    
		
		if (page.getResultItems().get("title")==null){
            //skip this page
            page.setSkip(true);
        }
		
		//fbPipeline.process(page, this);
        //page.putField("readme", page.getHtml().xpath("//div[@id='readme']/tidyText()"));
        System.out.println(page.getResultItems().get("title"));
        System.out.println(page.getUrl());
        
       
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

	
	
}
