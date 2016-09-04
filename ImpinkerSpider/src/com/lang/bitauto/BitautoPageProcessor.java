package com.lang.bitauto;

import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.bitauto.pageprocessor.BitautoNewsPageProcessor;

public class BitautoPageProcessor implements PageProcessor, Job {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static BitautoPipeline bitautoPipeline = new BitautoPipeline();

	@Override
	public void execute(JobExecutionContext arg0) throws JobExecutionException {
		Spider.create(new BitautoPageProcessor())
				.addUrl("http://www.bitauto.com/pingce/")
				.addPipeline(bitautoPipeline).thread(5).run();
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
