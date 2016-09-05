package com.lang.common;

import java.util.Date;
import java.util.List;

import us.codecraft.webmagic.Spider;
import us.codecraft.webmagic.monitor.SpiderMonitor.MonitorSpiderListener;
import us.codecraft.webmagic.monitor.SpiderStatus;
import us.codecraft.webmagic.monitor.SpiderStatusMXBean;

public class CustomSpiderStatus extends SpiderStatus implements
		SpiderStatusMXBean {

	public CustomSpiderStatus(Spider spider,
			MonitorSpiderListener monitorSpiderListener) {
		super(spider, monitorSpiderListener);
		// TODO Auto-generated constructor stub
	}

	@Override
	public String getName() {
		// TODO Auto-generated method stub
		return spider.getScheduler().getClass().getName();
	}

	@Override
	public int getErrorPageCount() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public List<String> getErrorPages() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public int getLeftPageCount() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int getPagePerSecond() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public Date getStartTime() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public String getStatus() {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public int getSuccessPageCount() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int getThread() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public int getTotalPageCount() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public void start() {
		// TODO Auto-generated method stub

	}

	@Override
	public void stop() {
		// TODO Auto-generated method stub

	}

}
