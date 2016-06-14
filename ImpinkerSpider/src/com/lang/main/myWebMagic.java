package com.lang.main;

import com.lang.fblife.FblifePageProcessor;
import com.lang.fblife.FblifePipeline;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.ResultItems;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.Task;
import us.codecraft.webmagic.pipeline.FilePipeline;
import us.codecraft.webmagic.pipeline.JsonFilePipeline;
import us.codecraft.webmagic.pipeline.Pipeline;
import us.codecraft.webmagic.processor.PageProcessor;

public class myWebMagic implements PageProcessor{

	
	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FilePipeline pipeline=new FilePipeline("D:\\spiler\\pipeline\\");
	
	/**
	 * @param args
	 */
	public static void main(String[] args)  {
		
		System.out.print("start");
		Spider.create(new myWebMagic())
		.addUrl("https://github.com/code4craft")
		.addPipeline(new FilePipeline("D:\\webmagic\\"))
		.thread(5).run();
		
		System.out.print("webmagic");
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

	@Override
	public void process(Page page) {
		
		 page.addTargetRequests(page.getHtml().links().regex("(https://github\\.com/\\w+/\\w+)").all());
	        page.putField("author", page.getUrl().regex("https://github\\.com/(\\w+)/.*").toString());
	        page.putField("name", page.getHtml().xpath("//h1[@class='entry-title public']/strong/a/text()").toString());
	        if (page.getResultItems().get("name")==null){
	            //skip this page
	            page.setSkip(true);
	        }
	        page.putField("readme", page.getHtml().xpath("//div[@id='readme']/tidyText()"));
	        
	        System.out.println(page.getUrl());
	        
	        //new ConsolePipeline().process(page, this.site);
	}

	
}
