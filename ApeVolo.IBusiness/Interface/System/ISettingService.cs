using System.Collections.Generic;
using System.Threading.Tasks;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Base;
using ApeVolo.IBusiness.Dto.System;
using ApeVolo.IBusiness.QueryModel;

namespace ApeVolo.IBusiness.Interface.System;

public interface ISettingService : IBaseServices<Entity.System.Setting>
{
    #region 基础接口

    Task<bool> CreateAsync(CreateUpdateSettingDto createUpdateSettingDto);
    Task<bool> UpdateAsync(CreateUpdateSettingDto createUpdateSettingDto);
    Task<bool> DeleteAsync(HashSet<long> ids);
    Task<List<SettingDto>> QueryAsync(SettingQueryCriteria settingQueryCriteria, Pagination pagination);
    Task<List<ExportBase>> DownloadAsync(SettingQueryCriteria settingQueryCriteria);

    Task<SettingDto> FindSettingByName(string settingName);

    #endregion
}
