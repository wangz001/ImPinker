package com.lang.autohome;
import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

public class AutohomeCulturePageProcessor implements PageProcessor {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static AutohomePipeline autohomePipeline=new AutohomePipeline();
	
	public static void main(String[] args){
		Spider.create(new AutohomeCulturePageProcessor())
		.addUrl("http://club.autohome.com.cn/jingxuan/")
		.addPipeline(autohomePipeline)
		.thread(1).run();
	}
	
	
	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		page.addTargetRequests(page.getHtml().links().regex("(http://club.autohome.com.cn/jingxuan/thread\\S+)").all());
		page.addTargetRequests(page.getHtml().links().regex("(http://club.autohome.com.cn/bbs/thread\\S+)").all());
		String titleString=page.getHtml().xpath("//title/text()").toString();
		String keyword=page.getHtml().xpath("//meta[@name='keywords']content/text()").toString();
		if(titleString!=null&&titleString.length()>0){
        	page.putField("url",page.getUrl());
            page.putField("title",titleString);
            page.putField("description",titleString);
            page.putField("keyword",keyword);
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
