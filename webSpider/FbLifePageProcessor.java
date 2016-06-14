package us.codecraft.webmagic.samples;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.pipeline.FilePipeline;
import us.codecraft.webmagic.processor.PageProcessor;
import us.codecraft.webmagic.selector.Html;

import java.io.UnsupportedEncodingException;
import java.util.List;

/**
 * @author 410775541@qq.com <br>
 * @since 0.5.1
 */
public class FbLifePageProcessor implements PageProcessor {

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
        Spider.create(new FbLifePageProcessor()).
                addUrl("http://tour.fblife.com/").
                addPipeline(new FilePipeline("D:\\webmagic\\fblife\\")).
                thread(1).
                run();
    }
    
    /**
     * 字符串编码转换的实现方法
     * @param str    待转换的字符串
     * @param newCharset    目标编码
     */
    public String changeCharset(String str, String newCharset) throws UnsupportedEncodingException {
        if(str != null) {
            //用默认字符编码解码字符串。与系统相关，中文windows默认为GB2312
            byte[] bs = str.getBytes();
            return new String(bs, newCharset);    //用新的字符编码生成字符串
        }
        return null;
    }
}
