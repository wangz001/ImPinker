package com.lang.quartz;

import java.util.Date;

import org.quartz.DateBuilder;
import org.quartz.JobBuilder;
import org.quartz.JobDetail;
import org.quartz.Scheduler;
import org.quartz.SchedulerException;
import org.quartz.SchedulerFactory;
import org.quartz.SimpleScheduleBuilder;
import org.quartz.Trigger;
import org.quartz.TriggerBuilder;
import org.quartz.impl.StdSchedulerFactory;

public class QuartzDemo {
	public void run() throws SchedulerException{  
        
        System.out.println("------- 初始化 ----------------------");    
          
        //通过SchedulerFactory工厂类获取Scheduler  
        SchedulerFactory sf = new StdSchedulerFactory();  
          
        System.out.println("------- 初始化完成 -----------");    
          
        //任务执行时间  
//      Date runTime = DateBuilder.evenMinuteDate(new Date());  
        Date runTime = DateBuilder.evenSecondDateAfterNow();  
          
        System.out.println("------- 将Job加入Scheduler中  ------");    
          
        //用于描叙Job实现类及其他的一些静态信息，构建一个作业实例  
        JobDetail jobDetail = JobBuilder.newJob(TestJob.class)  
                        .withIdentity("testJob_1", "group_1")  
                        .build();  
          
          
        //描叙触发Job执行的时间触发规则,Trigger实例化一个触发器  
        Trigger trigger = TriggerBuilder.newTrigger()//创建一个新的TriggerBuilder来规范一个触发器  
                        .withIdentity("trigger_1", "group_1")//给触发器一个名字和组名  
//                      .startNow()//立即执行  
                        .startAt(runTime)//设置触发开始的时间  
                        .withSchedule  
                        (  
                            SimpleScheduleBuilder.simpleSchedule()  
                            .withIntervalInSeconds(2)//时间间隔  
                            .withRepeatCount(5)//重复次数（将执行6次）  
                        )  
                        .build();//产生触发器  
          
        //运行容器，使用SchedulerFactory创建Scheduler实例  
        Scheduler sched = sf.getScheduler();  
          
        //向Scheduler添加一个job和trigger  
        sched.scheduleJob(jobDetail, trigger);  
        System.out.println(jobDetail.getKey() + " 运行在: " + runTime);   
          
        sched.start();  
    }  
      
    /** 
     * 调度测试 
     * @param args 
     * @throws SchedulerException  
     */  
    public static void main(String[] args) throws SchedulerException {  
        QuartzDemo demo = new QuartzDemo();  
        demo.run();  
    }  
}
