using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System.Setting;

[AutoMapping(typeof(Entity.Do.Core.Setting), typeof(SettingDto))]
public class SettingDto : EntityDtoRoot<long>
{
    [Display(Name = "Setting.Name")]
    public string Name { get; set; }

    [Display(Name = "Setting.Value")]
    public string Value { get; set; }

    [Display(Name = "Setting.Enabled")]
    public bool Enabled { get; set; }

    [Display(Name = "Setting.Description")]
    public string Description { get; set; }
}