using System;
using System.Collections.Specialized;
using System.Threading;
using Bita.Common;
using Common.Logging;
using GetCarDataService.ImArticleFirstImage;
using Quartz;
using Quartz.Impl;

namespace GetCarDataService
{
    public class QuartzMain
    {
        private static ILog _log = LogManager.GetLogger(typeof(QuartzMain));
        public void Run()
        {
            NameValueCollection ncv = new NameValueCollection();
            ncv["quartz.scheduler.instanceName"] = "TestScheduler";
            ncv["quartz.scheduler.instanceId"] = "instance_one";
            ncv["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            ncv["quartz.threadPool.threadCount"] = "5";
            ncv["quartz.threadPool.threadPriority"] = "Normal";
            ncv["quartz.jobStore.misfireThreshold"] = "60000";
            ISchedulerFactory sf = new StdSchedulerFactory(ncv);
            IScheduler sched = sf.GetScheduler();

            DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);

            IJobDetail job = JobBuilder.Create<ArticleFirstImageUpload>()
                .WithIdentity("ArticleFirstImageUpload", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(runTime)
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5 * 60).RepeatForever())
                .Build();

            sched.ScheduleJob(job, trigger);
            _log.Info(string.Format("{0} will run at: {1}", job.Key, runTime.ToString("r")));
            sched.Start();
            Thread.Sleep(10000);

            //sched.Shutdown(true);
        }
    }

    public class HelloJob : IJob
    {
        private static ILog _log = LogManager.GetLogger(typeof(HelloJob));
        /// <summary> 
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="ITrigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute(IJobExecutionContext context)
        {

            // Say Hello to the World and display the date/time
            _log.Info(string.Format("Hello World! - {0}", System.DateTime.Now.ToString("r")));
            Console.WriteLine(string.Format("Hello World! - {0}", DateTime.Now.ToString("r")));
        }
    }
}
