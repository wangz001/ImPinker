﻿using System;
using System.Collections.Specialized;
using System.Threading;
using Bita.Common;
using Common.Logging;
using GetCarDataService.ImArticleFirstImage;
using GetCarDataService.SolrDicGenerate;
using Quartz;
using Quartz.Impl;

namespace GetCarDataService
{
    public class QuartzMain
    {
        public void Run()
        {
            var ncv = new NameValueCollection();
            ncv["quartz.scheduler.instanceName"] = "TestScheduler";
            ncv["quartz.scheduler.instanceId"] = "instance_one";
            ncv["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            ncv["quartz.threadPool.threadCount"] = "5";
            ncv["quartz.threadPool.threadPriority"] = "Normal";
            ncv["quartz.jobStore.misfireThreshold"] = "60000";
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
            //上传图片
            IJobDetail jobOssImage = JobBuilder.Create<ArticleFirstImageUpload>()
                .WithIdentity("ArticleFirstImageUpload", "group1")
                //.StoreDurably(true)
                .Build();
            ITrigger triggerOssImage = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(runTime)
                .WithPriority(1)
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(2).RepeatForever())
                .Build();
            sched.ScheduleJob(jobOssImage, triggerOssImage);
            Common.WriteInfoLog("quartz——生成封面图服务");

            //抓取汽车之家车型数据
            IJobDetail jobAHData = JobBuilder.Create<GetAHAutoCarsData.GetBasicData>()
                .WithIdentity("GetBasicData", "group2")
                .Build();
            ITrigger triggerAHData = TriggerBuilder.Create()
                .WithIdentity("triggerAHData", "group2")
                .StartAt(runTime)
                .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
                .Build();
            //sched.ScheduleJob(jobAHData, triggerAHData);
            Common.WriteInfoLog("quartz获取汽车之家车型数据");

            //生成词典
            IJobDetail jobGenerateSolrDic = JobBuilder.Create<GenerateCarDataDic>()
                .WithIdentity("GenerateCarDataDic", "groupGenerateSolrDic")
                .Build();
            ITrigger triggerGenerateSolrDic = TriggerBuilder.Create()
                .WithIdentity("triggerGenerateSolrDic", "groupGenerateSolrDic")
                .StartAt(runTime)
                .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
                .Build();
            //sched.ScheduleJob(jobGenerateSolrDic, triggerGenerateSolrDic);
            Common.WriteInfoLog("quartz生成车型名称词典");

            Common.WriteInfoLog(string.Format("{0} will run at: {1}", jobOssImage.Key, runTime.ToString("r")));
            sched.Start();
            Thread.Sleep(1000);
            
            Console.WriteLine("服务已启动");
            Console.ReadLine();
            //sched.Shutdown(true);
        }
    }

}
