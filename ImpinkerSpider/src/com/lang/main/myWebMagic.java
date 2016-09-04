package com.lang.main;

import org.apache.log4j.Logger;
import org.quartz.SchedulerException;
import org.quartz.TimeOfDay;

import com.lang.autohome.AutoHomePageProcessor;
import com.lang.bitauto.BitautoPageProcessor;
import com.lang.fblife.FblifePageProcessor;
import com.lang.quartz.QuartzUtil;
import com.lang.quartz.TestJob;

public class MyWebMagic {

	private static Logger logger = Logger.getLogger(MyWebMagic.class);

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		logger.info("spider启动");
		try {
			// 添加第一个任务 每隔10秒执行一次
			QuartzUtil.addJob("job1", "trigger1", TestJob.class, new TimeOfDay(
					17, 27));

			// fblife任务 每隔一天执行一次
			QuartzUtil.addJob("fblifeSpider", "triggerFblife",
					FblifePageProcessor.class, new TimeOfDay(17, 26));
			// autohome任务 每隔一天执行一次
			QuartzUtil.addJob("autohomeSpider", "triggerautohome",
					AutoHomePageProcessor.class, new TimeOfDay(02, 26));
			// bitauto任务 每隔一天执行一次
			QuartzUtil.addJob("bitautoSpider", "triggerbitauto",
					BitautoPageProcessor.class, new TimeOfDay(06, 26));
		} catch (SchedulerException e) {
			e.printStackTrace();
		}
	}

}
