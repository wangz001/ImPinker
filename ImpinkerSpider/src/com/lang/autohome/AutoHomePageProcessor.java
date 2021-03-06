package com.lang.autohome;

import java.util.Arrays;
import java.util.List;

import javax.management.JMException;

import org.apache.log4j.Logger;
import org.quartz.DisallowConcurrentExecution;
import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.monitor.SpiderMonitor;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.pageprocessor.AutohomeCulturePageProcessor;
import com.lang.common.SolrJUtil;
import com.lang.main.MyWebMagic;
import com.lang.properties.AppProperties;
import com.lang.util.RegexUtil;

@DisallowConcurrentExecution
public class AutoHomePageProcessor implements PageProcessor, Job {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static AutohomePipeline autohomePipeline = new AutohomePipeline();
	private static int autohomeRequestCount = 0;
	Logger logger = Logger.getLogger(MyWebMagic.class);
	static List<String> typearrStr = Arrays.asList("culture", "tuning", "news",
			"drive");
	static int maxNum = Integer.parseInt(AppProperties
			.getPropertyByName("spider.maxnum"));

	public static void main(String[] args) {
		Spider spider = Spider.create(new AutoHomePageProcessor())
				.addUrl("http://www.autohome.com.cn/all/")
				.addPipeline(autohomePipeline).thread(1);
		try {
			SpiderMonitor.instance().register(spider);
		} catch (JMException e) {
			e.printStackTrace();
		}
		spider.start();
		// 超过10000次时，停止爬取。防止ip被封
		while (true) {
			if (autohomeRequestCount > 100) {
				spider.stop();
				break;
			}
			try {
				Thread.sleep(1000 * 3);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		SolrJUtil.getInstance().LastCommit();
	}

	@Override
	public void execute(JobExecutionContext arg0) throws JobExecutionException {
		Spider spider = Spider.create(new AutoHomePageProcessor())
				.addUrl("http://www.autohome.com.cn/all/")
				.addPipeline(autohomePipeline).thread(5);
		try {
			SpiderMonitor.instance().register(spider);
		} catch (JMException e) {
			e.printStackTrace();
		}
		spider.start();
		// 超过10000次时，停止爬取。防止ip被封
		while (true) {
			if (autohomeRequestCount > maxNum) {
				spider.stop();
				spider.close();
				autohomeRequestCount = 0; // 解决quartz第二次启动的问题
				break;
			}
			try {
				Thread.sleep(1000 * 3);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		SolrJUtil.getInstance().LastCommit();
		logger.info("autohome spider 结束");
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

	@Override
	public void process(Page page) {
		autohomeRequestCount++;
		page.addTargetRequests(page.getHtml().links()
				.regex("http://www.autohome.com.cn/\\w+/\\d+/\\S*").all());
		//添加文章列表页，底部分页链接
		page.addTargetRequests(page.getHtml().links()
				.regex("http://www.autohome.com.cn/all/\\S*").all());
		String thisUrlString = page.getUrl().toString();

		if (RegexUtil.match("http://www.autohome.com.cn/\\w+/\\S+",
				thisUrlString)) {
			String reg = "http://www.autohome.com.cn/(\\w+)/\\S+";
			String key = thisUrlString.replaceAll(reg, "$1");
			if (typearrStr.contains(key)) {
				new AutohomeCulturePageProcessor().process(page);
				return;
			}
		}
		page.setSkip(true);
	}
}
