﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.SnowflakeIdHelper;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Monitor.Logs;
using ApeVolo.IBusiness.Interface.Monitor.Auditing;
using ApeVolo.IBusiness.Interface.System.Setting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Shyjus.BrowserDetection;

namespace ApeVolo.Api.Filter;

/// <summary>
/// 审计过滤器
/// </summary>
public class AuditingFilter : IAsyncActionFilter
{
    private readonly IAuditLogService _auditInfoService;
    private readonly ICurrentUser _currentUser;
    private readonly ISettingService _settingService;
    private readonly IBrowserDetector _browserDetector;
    private readonly ILogger<AuditingFilter> _logger;

    public AuditingFilter(IAuditLogService auditInfoService, ICurrentUser currentUser,
        ISettingService settingService, IBrowserDetector browserDetector, ILogger<AuditingFilter> logger)
    {
        _auditInfoService = auditInfoService;
        _currentUser = currentUser;
        _settingService = settingService;
        _browserDetector = browserDetector;
        _logger = logger;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return ExecuteAuditing(context, next);
    }

    /// <summary>
    /// 执行审计功能
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    private async Task ExecuteAuditing(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            var resultContext = await next();
            sw.Stop();
            //执行结果

            if ((await _settingService.FindSettingByName("IsAuditLogSaveDB")).Value.ToBool())
            {
                var result = resultContext.Result;
                if (context.HttpContext.IsNotNull() && result.IsNotNull())
                {
                    var reqUrlPath = context.HttpContext.Request.Path.Value?.ToLower();
                    var settingDto = await _settingService.FindSettingByName("LgnoreAuditLogUrlPath");
                    var lgnoreAuditLogUrlPathList = settingDto.Value.Split("|");
                    if (!lgnoreAuditLogUrlPathList.Contains(reqUrlPath))
                    {
                        var auditInfo = CreateAuditLog(context);
                        switch (result?.GetType().FullName)
                        {
                            case "Microsoft.AspNetCore.Mvc.ObjectResult":
                            {
                                var value = ((ObjectResult)result).Value;
                                if (value != null)
                                    auditInfo.ResponseData = value.ToString();
                                break;
                            }
                            case "Microsoft.AspNetCore.Mvc.FileContentResult":
                                auditInfo.ResponseData = ((FileContentResult)result).FileDownloadName;
                                break;
                            default:
                                auditInfo.ResponseData = ((ContentResult)result)?.Content;
                                break;
                        }

                        //用时
                        auditInfo.ExecutionDuration = Convert.ToInt32(sw.ElapsedMilliseconds);
                        await _auditInfoService.CreateAsync(auditInfo);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ExceptionLogFormat.WriteLog(context.HttpContext, ex, _currentUser?.Name,
                _browserDetector.Browser?.OS, _browserDetector.Browser?.DeviceType, _browserDetector.Browser?.Name,
                _browserDetector.Browser?.Version));
            ConsoleHelper.WriteLine(ex.Message, ConsoleColor.Red);
        }
    }

    /// <summary>
    /// 创建审计对象
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private AuditLog CreateAuditLog(ActionExecutingContext context)
    {
        var routeValues = context.ActionDescriptor.RouteValues;
        var desc =
            ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttribute(
                typeof(DescriptionAttribute), true);

        var httpContext = context.HttpContext;
        var remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        var arguments = HttpHelper.GetAllRequestParams(httpContext); //context.ActionArguments;
        var description = desc == null ? "" : ((DescriptionAttribute)desc).Description;

        var auditLog = new AuditLog
        {
            Id = IdHelper.GetLongId(),
            CreateBy = _currentUser.Name ?? "",
            CreateTime = DateTime.Now,
            Area = routeValues["area"],
            Controller = routeValues["controller"],
            Action = routeValues["action"],
            Method = httpContext.Request.Method,
            Description = ExceptionLogFormat.GetResourcesDescription(description, routeValues["area"]),
            RequestUrl = httpContext.Request.GetDisplayUrl(),
            RequestParameters = arguments.ToJson(),
            RequestIp = remoteIp,
            IpAddress = IpHelper.GetIpAddress(remoteIp),
            OperatingSystem = _browserDetector.Browser?.OS,
            DeviceType = _browserDetector.Browser?.DeviceType,
            BrowserName = _browserDetector.Browser?.Name,
            Version = _browserDetector.Browser?.Version
        };


        var reqUrl = httpContext.Request.Path.Value?.ToLower();
        if (reqUrl is "/auth/login")
        {
            var (_, value) = arguments.SingleOrDefault(k => k.Key == "username");
            if (!value.IsNullOrEmpty())
            {
                auditLog.CreateBy = value.ToString();
            }
        }

        return auditLog;
    }
}