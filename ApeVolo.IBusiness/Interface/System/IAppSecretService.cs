using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.System;

/// <summary>
/// 应用秘钥
/// </summary>
public interface IAppSecretService : IBaseServices<Entity.System.AppSecret>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto);
    Task<bool> UpdateAsync(CreateUpdateAppSecretDto createUpdateAppSecretDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<AppSecretDto>> QueryAsync(AppsecretQueryCriteria appsecretQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(AppsecretQueryCriteria appsecretQueryCriteria);

    #endregion
}
