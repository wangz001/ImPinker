package com.lang.bitauto;

import javax.management.JMException;

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

public class BitautoPageProcessor implements PageProcessor, Job {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static BitautoPipeline bitautoPipeline = new BitautoPipeline();

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
		spider.run();
		SolrJUtil.getInstance().LastCommit();
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
		new BitautoNewsPageProcessor().process(page);
	}

}