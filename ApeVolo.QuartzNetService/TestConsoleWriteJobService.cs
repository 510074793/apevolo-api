﻿using ApeVolo.IBusiness.Interface.Tasks;
using Quartz;
using System;
using System.Threading.Tasks;
using ApeVolo.QuartzNetService.service;

namespace ApeVolo.QuartzNetService
{
    /// <summary>
    /// 测试控制台打印作业
    /// </summary>
    public class TestConsoleWriteJobService : JobBase, IJob
    {
        public TestConsoleWriteJobService(ISchedulerCenterService schedulerCenterService,
            IQuartzNetService quartzNetService, IQuartzNetLogService quartzNetLogService)
        {
            _quartzNetService = quartzNetService;
            _quartzNetLogService = quartzNetLogService;
            _schedulerCenterService = schedulerCenterService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await ExecuteJob(context, async () => await Run(context));
        }

        private async Task Run(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("当前时间：" + DateTime.Now + "\n");
            //获取传递参数
            JobDataMap data = context.JobDetail.JobDataMap;
        }
    }
}