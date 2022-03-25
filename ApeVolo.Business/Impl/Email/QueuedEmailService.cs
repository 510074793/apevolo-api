﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Caches.Redis.Service;
using ApeVolo.Common.Caches.Redis.Service.MessageQueue;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Global;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.Entity.Do.Email;
using ApeVolo.IBusiness.Dto.Email;
using ApeVolo.IBusiness.EditDto.Email;
using ApeVolo.IBusiness.Interface.Email;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IRepository.Email;
using AutoMapper;

namespace ApeVolo.Business.Impl.Email;

/// <summary>
/// 邮件队列接口实现
/// </summary>
public class QueuedEmailService : BaseServices<QueuedEmail>, IQueuedEmailService
{
    #region 字段

    private readonly IEmailMessageTemplateService _emailMessageTemplateService;
    private readonly IEmailAccountService _emailAccountService;
    private readonly IRedisCacheService _redisCacheService;

    #endregion

    #region 构造函数

    public QueuedEmailService(IQueuedEmailRepository queuedEmailRepository, IMapper mapper,
        IEmailMessageTemplateService emailMessageTemplateService, IEmailAccountService emailAccountService,
        IRedisCacheService redisCacheService)
    {
        _baseDal = queuedEmailRepository;
        _mapper = mapper;
        _emailMessageTemplateService = emailMessageTemplateService;
        _emailAccountService = emailAccountService;
        _redisCacheService = redisCacheService;
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
    {
        var queuedEmail = _mapper.Map<QueuedEmail>(createUpdateQueuedEmailDto);
        return await AddEntityAsync(queuedEmail);
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="queuedEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateTriesAsync(QueuedEmailDto queuedEmailDto)
    {
        var queuedEmail = _mapper.Map<QueuedEmail>(queuedEmailDto);
        return await _baseDal.UpdateAsync(queuedEmail) > 0;
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="createUpdateQueuedEmailDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateQueuedEmailDto createUpdateQueuedEmailDto)
    {
        if (!await IsExistAsync(x => x.IsDeleted == false
                                     && x.Id == createUpdateQueuedEmailDto.Id))
        {
            throw new BadRequestException($"({nameof(QueuedEmail)})不存在!");
        }

        var queuedEmail = _mapper.Map<QueuedEmail>(createUpdateQueuedEmailDto);
        return await UpdateEntityAsync(queuedEmail);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var emailAccounts = await QueryByIdsAsync(ids);
        if (emailAccounts.Count < 1)
            throw new BadRequestException("无可删除数据!");

        return await DeleteEntityListAsync(emailAccounts);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="queuedEmailQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<QueuedEmailDto>> QueryAsync(QueuedEmailQueryCriteria queuedEmailQueryCriteria,
        Pagination pagination)
    {
        Expression<Func<QueuedEmail, bool>> whereExpression = x => x.IsDeleted == false;
        if (!queuedEmailQueryCriteria.Id.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(x => x.Id == queuedEmailQueryCriteria.Id);
        }

        if (queuedEmailQueryCriteria.MaxTries > 0)
        {
            whereExpression = whereExpression.And(x => x.SentTries < queuedEmailQueryCriteria.MaxTries);
        }

        if (!queuedEmailQueryCriteria.Form.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(x =>
                x.From.Contains(queuedEmailQueryCriteria.Form) ||
                x.FromName.Contains(queuedEmailQueryCriteria.Form));
        }

        if (!queuedEmailQueryCriteria.To.IsNullOrEmpty())
        {
            whereExpression = whereExpression.And(x =>
                x.To.Contains(queuedEmailQueryCriteria.To) || x.ToName.Contains(queuedEmailQueryCriteria.To));
        }

        if (queuedEmailQueryCriteria.IsSend.IsNotNull())
        {
            whereExpression = queuedEmailQueryCriteria.IsSend.ToBool()
                ? whereExpression.And(x => x.SendTime != null)
                : whereExpression.And(x => x.SendTime == null);
        }

        if (!queuedEmailQueryCriteria.CreateTime.IsNullOrEmpty() && queuedEmailQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.And(x =>
                x.CreateTime >= queuedEmailQueryCriteria.CreateTime[0] &&
                x.CreateTime <= queuedEmailQueryCriteria.CreateTime[1]);
        }

        return _mapper.Map<List<QueuedEmailDto>>(await _baseDal.QueryPageListAsync(whereExpression, pagination));
    }

    #endregion

    #region 扩展方法

    /// <summary>
    /// 变更邮箱验证码
    /// </summary>
    /// <param name="emailAddres"></param>
    /// <param name="messageTemplateName"></param>
    /// <returns></returns>
    public async Task<bool> ResetEmail(string emailAddres, string messageTemplateName)
    {
        var emailMessageTemplate =
            await _emailMessageTemplateService.QueryFirstAsync(x =>
                x.IsDeleted == false && x.Name == messageTemplateName);
        if (emailMessageTemplate.IsNull())
            throw new BadRequestException($"{messageTemplateName} 获取邮件模板失败！");
        var emailAccount = await _emailAccountService.QuerySingleAsync(emailMessageTemplate.EmailAccountId);

        //生成6位随机码
        var captcha = ImgVerifyCodeHelper.BuilEmailCaptcha(6);

        QueuedEmail queuedEmail = new QueuedEmail();
        queuedEmail.InitEntity();
        queuedEmail.From = emailAccount.Email;
        queuedEmail.FromName = emailAccount.DisplayName;
        queuedEmail.To = emailAddres;
        queuedEmail.Priority = (int)QueuedEmailPriority.High;
        queuedEmail.Bcc = emailMessageTemplate.BccEmailAddresses ?? null;
        queuedEmail.Subject = emailMessageTemplate.Subject ?? null;
        queuedEmail.Body = emailMessageTemplate.Body.Replace("%captcha%", captcha);
        queuedEmail.SentTries = 0;
        queuedEmail.EmailAccountId = emailAccount.Id;

        bool isTrue = await _baseDal.AddReturnBoolAsync(queuedEmail);
        if (isTrue)
        {
            await _redisCacheService.RemoveAsync(RedisKey.EmailCaptchaKey + queuedEmail.To.ToMd5String());
            await _redisCacheService.SetCacheAsync(RedisKey.EmailCaptchaKey + queuedEmail.To.ToMd5String(), captcha,
                TimeSpan.FromMinutes(5));
            //进redis队列执行发送
            await _redisCacheService.ListLeftPushAsync(MqTopicNameKey.MailboxQueue, queuedEmail.Id.ToString());
        }

        return isTrue;
    }

    #endregion
}