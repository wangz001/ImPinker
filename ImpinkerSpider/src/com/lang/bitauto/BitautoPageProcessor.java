package com.lang.bitauto;

import javax.management.JMException;

import org.apache.log4j.Logger;
import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.monitor.SpiderMonitor;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.bitauto.pageprocessor.BitautoNewsPageProcessor;
import com.lang.common.SolrJUtil;
import com.lang.main.MyWebMagic;
import com.lang.properties.AppProperties;

public class BitautoPageProcessor implements PageProcessor, Job {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static BitautoPipeline bitautoPipeline = new BitautoPipeline();
	private int bitRequestCount = 0;
	Logger logger = Logger.getLogger(MyWebMagic.class);
	static int maxNum = Integer.parseInt(AppProperties
			.getPropertyByName("spider.maxnum"));

	@Override
	public void execute(JobExecutionContext arg0) throws JobExecutionException {
		Spider spider = Spider.create(new BitautoPageProcessor())
				.addUrl("http://www.bitauto.com/pingce/")
				.addPipeline(bitautoPipeline).thread(5);
		try {
			SpiderMonitor.instance().register(spider);
		} catch (JMException e) {
			e.printStackTrace();
		}
		spider.start();
		// 超过10000次时，停止爬取。防止ip被封
		while (true) {
			if (bitRequestCount > maxNum) {
				spider.stop();
				spider.close();
				bitRequestCount = 0; // 解决quartz第二次启动的问题
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
		logger.info("bitauto spider 结束");
	}

	/**
	 * 调试用
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		Spider spider = Spider.create(new BitautoPageProcessor())
				.addUrl("http://www.bitauto.com/pingce/")
				.addPipeline(bitautoPipeline).thread(1);
		try {
			SpiderMonitor.instance().register(spider);
		} catch (JMException e) {
			e.printStackTrace();
		}
		spider.run();
		SolrJUtil.getInstance().LastCommit();
	}

	@Override
	public Site getSite() {
		return site;
	}

	@Override
	public void process(Page page) {
		bitRequestCount++;
		new BitautoNewsPageProcessor().process(page);
	}

}
