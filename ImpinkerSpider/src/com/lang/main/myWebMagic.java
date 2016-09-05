package com.lang.main;

import org.apache.log4j.Logger;
import org.apache.log4j.PropertyConfigurator;
import org.quartz.SchedulerException;

import com.lang.autohome.AutoHomePageProcessor;
import com.lang.bitauto.BitautoPageProcessor;
import com.lang.fblife.FblifePageProcessor;
import com.lang.quartz.QuartzUtil;
import com.lang.quartz.TestJob;

public class MyWebMagic {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		PropertyConfigurator.configure("log4j.properties");
		Logger logger = Logger.getLogger(MyWebMagic.class);
		logger.info("spider启动");
		// 添加第一个任务 每隔10秒执行一次
		try {

			QuartzUtil.addJobCronTrigger("job1", "trigger1", TestJob.class,
					"30 50-55 15 * * ?");

			// fblife任务 每隔一天执行一次
			QuartzUtil.addJobCronTrigger("fblifeSpider", "triggerFblife",
					FblifePageProcessor.class, "30 07 18 * * ?");
			// autohome任务 每隔一天执行一次
			QuartzUtil.addJobCronTrigger("autohomeSpider", "triggerautohome",
					AutoHomePageProcessor.class, "30 10 3 * * ?");
			// bitauto任务 每隔一天执行一次
			QuartzUtil.addJobCronTrigger("bitautoSpider", "triggerbitauto",
					BitautoPageProcessor.class, "30 10 5 * * ?");
		} catch (SchedulerException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

}
