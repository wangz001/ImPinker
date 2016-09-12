package com.lang.fblife;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.TimeZone;

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

import com.google.common.base.Joiner;
import com.lang.common.SolrJUtil;
import com.lang.fblife.pageprocessor.FbLifeTourPageProcessor;
import com.lang.fblife.pageprocessor.FblifeCulturePageProcessor;
import com.lang.fblife.pageprocessor.FblifeEvaluatePageProcessor;
import com.lang.fblife.pageprocessor.FblifeNewsPageProcessor;
import com.lang.fblife.pageprocessor.FblifeReStylePageProcessor;
import com.lang.main.MyWebMagic;
import com.lang.util.RegexUtil;

public class FblifePageProcessor implements PageProcessor, Job {

	/*
	 * private Site site = Site .me() .setRetryTimes(3) .setSleepTime(100) //
	 * 使用代理 .setHttpProxyPool( Lists.newArrayList( new String[] {
	 * "123.57.155.168", "8123" }, new String[] { "123.57.155.168", "8123" }))
	 * .addHeader("Proxy-Authorization", getMd5());
	 */
	private Site site = Site.me().setRetryTimes(3).setSleepTime(100);
	private static FblifePipeline fbPipeline = new FblifePipeline();
	Logger logger = Logger.getLogger(MyWebMagic.class);

	@Override
	public void execute(JobExecutionContext context)
			throws JobExecutionException {
		logger.info("Fblife spider启动");
		Spider spider = Spider.create(new FblifePageProcessor())
				.addUrl("http://www.fblife.com/").addPipeline(fbPipeline)
				.thread(8);
		try {
			SpiderMonitor.instance().register(spider);
		} catch (JMException e) {
			e.printStackTrace();
		}
		spider.run();
		SolrJUtil.getInstance().LastCommit();
		logger.info("Fblife spider 结束");
	}

	/**
	 * 调试用
	 * 
	 * @param args
	 */
	public static void main(String[] args) {

		String tmpDir = System.getProperty("java.io.tmpdir");
		System.out.println(tmpDir);
		Spider spider = Spider.create(new FblifePageProcessor())
				.addUrl("http://www.fblife.com/").addPipeline(fbPipeline)
				.thread(1);
		try {
			SpiderMonitor.instance().register(spider);
		} catch (JMException e) {
			e.printStackTrace();
		}
		spider.run();
		SolrJUtil.getInstance().LastCommit();
	}

	@Override
	public void process(Page page) {
		// TODO Auto-generated method stub
		List<String> fbLinks = page.getHtml().links()
				.regex("(http://\\w+.fblife\\.com/html/\\w+/\\w+.html)").all();
		page.addTargetRequests(fbLinks);
		String thisUrlString = page.getUrl().toString();

		if (RegexUtil.match("http://culture.fblife\\.com/html/\\w+/\\w+.html",
				thisUrlString)) {
			new FblifeCulturePageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://tour.fblife\\.com/html/\\w+/\\w+.html", thisUrlString)) {
			new FbLifeTourPageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://restyle.fblife\\.com/html/\\w+/\\w+.html",
				thisUrlString)) {
			new FblifeReStylePageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://news.fblife\\.com/html/\\w+/\\w+.html", thisUrlString)) {
			new FblifeNewsPageProcessor().process(page);
			return;
		} else if (RegexUtil.match(
				"http://drive.fblife\\.com/html/\\w+/\\w+.html", thisUrlString)) {
			new FblifeEvaluatePageProcessor().process(page);
			return;
		}
		page.setSkip(true);
	}

	@Override
	public Site getSite() {
		// TODO Auto-generated method stub
		site.addHeader("Proxy-Authorization", getMd5());
		return site;
	}

	private String getMd5() {
		// 定义申请获得的appKey和appSecret
		String appkey = "194885822";
		String secret = "7a1333d3c40e8ea691a1372e084f25e9";

		// 创建参数表
		Map<String, String> paramMap = new HashMap<String, String>();
		paramMap.put("app_key", appkey);
		DateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		format.setTimeZone(TimeZone.getTimeZone("GMT+8"));// 使用中国时间，以免时区不同导致认证错误
		paramMap.put("timestamp", format.format(new Date()));

		// 对参数名进行排序
		String[] keyArray = paramMap.keySet().toArray(new String[0]);
		Arrays.sort(keyArray);

		// 拼接有序的参数名-值串
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.append(secret);
		for (String key : keyArray) {
			stringBuilder.append(key).append(paramMap.get(key));
		}

		stringBuilder.append(secret);
		String codes = stringBuilder.toString();

		// MD5编码并转为大写， 这里使用的是Apache codec
		String sign = org.apache.commons.codec.digest.DigestUtils.md5Hex(codes)
				.toUpperCase();

		paramMap.put("sign", sign);

		// 拼装请求头Proxy-Authorization的值，这里使用 guava 进行map的拼接
		String authHeader = "MYH-AUTH-MD5 "
				+ Joiner.on('&').withKeyValueSeparator("=").join(paramMap);

		System.out.println(authHeader);
		return authHeader;
	}

}
