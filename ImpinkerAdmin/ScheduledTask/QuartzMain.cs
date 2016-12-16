using System;
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
        private static ILog _log = LogManager.GetLogger(typeof(QuartzMain));
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
                .Build();
            ITrigger triggerOssImage = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(runTime)
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                .Build();
            sched.ScheduleJob(jobOssImage, triggerOssImage);

            //抓取汽车之家车型数据
            IJobDetail jobAHData = JobBuilder.Create<GetAHAutoCarsData.GetBasicData>()
                .WithIdentity("GetBasicData", "group2")
                .Build();
            ITrigger triggerAHData = TriggerBuilder.Create()
                .WithIdentity("triggerAHData", "group2")
                .StartAt(runTime)
                .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
                .Build();
            sched.ScheduleJob(jobAHData, triggerAHData);

            //生成词典
            IJobDetail jobGenerateSolrDic = JobBuilder.Create<GenerateCarDataDic>()
                .WithIdentity("GenerateCarDataDic", "groupGenerateSolrDic")
                .Build();
            ITrigger triggerGenerateSolrDic = TriggerBuilder.Create()
                .WithIdentity("triggerGenerateSolrDic", "groupGenerateSolrDic")
                .StartAt(runTime)
                .WithSimpleSchedule(x => x.WithIntervalInHours(12).RepeatForever())
                .Build();
            sched.ScheduleJob(jobGenerateSolrDic, triggerGenerateSolrDic);

            _log.Info(string.Format("{0} will run at: {1}", jobOssImage.Key, runTime.ToString("r")));
            sched.Start();
            Thread.Sleep(10000);
            _log.Info("quartz开始");
            Console.WriteLine("服务已启动");
            Console.ReadLine();
            //sched.Shutdown(true);
        }
    }

}
