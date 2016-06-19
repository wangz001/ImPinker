package com.lang.fblife;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.pipeline.FilePipeline;
import us.codecraft.webmagic.processor.PageProcessor;
import java.util.List;

import com.lang.util.DBHelper;

/**
 * @author 410775541@qq.com <br>
 * @since 0.5.1
 */
public class FbLifeTourPageProcessor implements PageProcessor {

    private Site site = Site.me().setCycleRetryTimes(5).setRetryTimes(5).setSleepTime(500).setTimeOut(3 * 60 * 1000)
            .setUserAgent("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0")
            .addHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
            .addHeader("Accept-Language", "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3")
            .setCharset("gbk");

    private static final int voteNum = 1000;


    @Override
    public void process(Page page) {
        List<String> relativeUrl = page.getHtml().xpath("//div[@id='f_mtscroll_ul']/ul/li/a/@href").all();
        relativeUrl.addAll(page.getHtml().xpath("//div[@id='zixun_list']/div/div/div/a/@href").all());
        relativeUrl.addAll(page.getHtml().xpath("//div[@class='channelpage']/a/@href").all());
        page.addTargetRequests(relativeUrl);
        String content =  page.getHtml().xpath("//div[@id='f_content']/div/div/div/h1/text()").toString();
        
        boolean exist = false;
        if(content!=null&&content.length()>0){
        	page.putField("url",page.getUrl());
            page.putField("title",content);
            page.putField("description",content);
            page.putField("keyword",content);
            //page.putField("userid", new Html(answer).xpath("//a[@class='author-link']/@href"));
            exist = true;
        }
        if(!exist){
            page.setSkip(true);
        }
    }

    @Override
    public Site getSite() {
        return site;
    }

    public static void main(String[] args) {
        
    	
    	Spider.create(new FbLifeTourPageProcessor()).
                addUrl("http://tour.fblife.com/").
                addPipeline(new FblifePipeline()).
                thread(1).
                run();
    }
    
}
