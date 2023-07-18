﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.Role), typeof(CreateUpdateRoleDto))]
public class CreateUpdateRoleDto : BaseEntityDto<long>
{
    [Display(Name = "Role.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Name { get; set; }

    public int Level { get; set; }

    public string Description { get; set; }

    public string DataScope { get; set; }

    [Display(Name = "Role.Permission")]
    [Required(ErrorMessage = "{0}required")]
    public string Permission { get; set; }

    public List<RoleDeptDto> Depts { get; set; }

    public List<RoleMenuDto> Menus { get; set; }
}
