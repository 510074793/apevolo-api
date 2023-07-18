﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApeVolo.Business.Base;
using ApeVolo.Common.Exception;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Model;
using ApeVolo.Common.Resources;
using ApeVolo.Common.WebApp;
using ApeVolo.Entity.Message.Email;
using ApeVolo.IBusiness.Dto.Message.Email;
using ApeVolo.IBusiness.Interface.Message.Email;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.Business.Message.Email;

/// <summary>
/// 邮件消息模板实现
/// </summary>
public class EmailMessageTemplateService : BaseServices<EmailMessageTemplate>, IEmailMessageTemplateService
{
    #region 构造函数

    public EmailMessageTemplateService(ApeContext apeContext) : base(apeContext)
    {
    }

    #endregion

    #region 基础方法

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    public async Task<bool> CreateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        var messageTemplate = await TableWhere(x => x.Name == createUpdateEmailMessageTemplateDto.Name).FirstAsync();
        if (messageTemplate.IsNotNull())
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("EmailMessageTemplate"),
                createUpdateEmailMessageTemplateDto.Name));

        return await AddEntityAsync(ApeContext.Mapper.Map<EmailMessageTemplate>(createUpdateEmailMessageTemplateDto));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="createUpdateEmailMessageTemplateDto"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(CreateUpdateEmailMessageTemplateDto createUpdateEmailMessageTemplateDto)
    {
        var emailMessageTemplate = await TableWhere(x => x.Id == createUpdateEmailMessageTemplateDto.Id).FirstAsync();
        if (emailMessageTemplate.IsNull())
        {
            throw new BadRequestException(Localized.Get("DataNotExist"));
        }

        if (emailMessageTemplate.Name != createUpdateEmailMessageTemplateDto.Name &&
            await TableWhere(j => j.Id == emailMessageTemplate.Id).AnyAsync())
        {
            throw new BadRequestException(Localized.Get("{0}{1}IsExist", Localized.Get("EmailMessageTemplate"),
                emailMessageTemplate.Name));
        }

        return await UpdateEntityAsync(
            ApeContext.Mapper.Map<EmailMessageTemplate>(createUpdateEmailMessageTemplateDto));
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<bool> DeleteAsync(HashSet<long> ids)
    {
        var messageTemplateList = await TableWhere(x => ids.Contains(x.Id)).ToListAsync();
        if (messageTemplateList.Count <= 0)
            throw new BadRequestException(Localized.Get("DataNotExist"));
        return await LogicDelete<EmailMessageTemplate>(x => ids.Contains(x.Id)) > 0;
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="messageTemplateQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public async Task<List<EmailMessageTemplateDto>> QueryAsync(
        EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria, Pagination pagination)
    {
        var whereExpression = GetWhereExpression(messageTemplateQueryCriteria);

        return ApeContext.Mapper.Map<List<EmailMessageTemplateDto>>(
            await SugarRepository.QueryPageListAsync(whereExpression, pagination));
    }

    #endregion

    #region 条件表达式

    private static Expression<Func<EmailMessageTemplate, bool>> GetWhereExpression(
        EmailMessageTemplateQueryCriteria messageTemplateQueryCriteria)
    {
        Expression<Func<EmailMessageTemplate, bool>> whereExpression = x => true;
        if (!messageTemplateQueryCriteria.Name.IsNullOrEmpty())
        {
            whereExpression = whereExpression.AndAlso(x => x.Name.Contains(messageTemplateQueryCriteria.Name));
        }

        if (messageTemplateQueryCriteria.IsActive.IsNotNull())
        {
            whereExpression = whereExpression.AndAlso(x => x.IsActive == messageTemplateQueryCriteria.IsActive);
        }

        if (!messageTemplateQueryCriteria.CreateTime.IsNullOrEmpty() &&
            messageTemplateQueryCriteria.CreateTime.Count > 1)
        {
            whereExpression = whereExpression.AndAlso(x =>
                x.CreateTime >= messageTemplateQueryCriteria.CreateTime[0] &&
                x.CreateTime <= messageTemplateQueryCriteria.CreateTime[1]);
        }

        return whereExpression;
    }

    #endregion
}
