package com.lang.autohome;

import org.quartz.Job;
import org.quartz.JobExecutionContext;
import org.quartz.JobExecutionException;

import us.codecraft.webmagic.Page;
import us.codecraft.webmagic.Site;
import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.processor.PageProcessor;

import com.lang.autohome.pageprocessor.AutoHomeEvaluatePageProcessor;
import com.lang.autohome.pageprocessor.AutoHomeNewsPageProcessor;
import com.lang.autohome.pageprocessor.AutoHomeReStylePageProcessor;
import com.lang.autohome.pageprocessor.AutohomeCulturePageProcessor;
import com.lang.util.RegexUtil;

public class AutoHomePageProcessor implements PageProcessor, Job {

	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static AutohomePipeline autohomePipeline = new AutohomePipeline();

	@Override
	public void execute(JobExecutionContext arg0) throws JobExecutionException {
		Spider.create(new AutoHomePageProcessor())
				.addUrl("http://www.autohome.com.cn/")
				.addPipeline(autohomePipeline).thread(5).run();
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		return site;
	}

	@Override
	public void process(Page page) {

		page.addTargetRequests(page.getHtml().links()
				.regex("http://\\www.autohome.com.cn/culture/\\S+").all());
		page.addTargetRequests(page.getHtml().links()
				.regex("http://\\www.autohome.com.cn/drive/\\S+").all());
		page.addTargetRequests(page.getHtml().links()
				.regex("http://\\www.autohome.com.cn/news/\\S+").all());
		page.addTargetRequests(page.getHtml().links()
				.regex("http://\\www.autohome.com.cn/tuning/\\S+").all());

		String thisUrlString = page.getUrl().toString();

		if (RegexUtil.match("http://www.autohome.com.cn/culture/\\S+",
				thisUrlString)) {
			new AutohomeCulturePageProcessor().process(page);
			return;
		} else if (RegexUtil.match("http://www.autohome.com.cn/drive/\\S+",
				thisUrlString)) {
			new AutoHomeEvaluatePageProcessor().process(page);
			return;
		} else if (RegexUtil.match("http://www.autohome.com.cn/news/\\S+",
				thisUrlString)) {
			new AutoHomeNewsPageProcessor().process(page);
			return;
		} else if (RegexUtil.match("http://www.autohome.com.cn/tuning/\\S+",
				thisUrlString)) {
			new AutoHomeReStylePageProcessor().process(page);
			return;
		}
		page.setSkip(true);

		System.out.println(page.getUrl());
	}

}
