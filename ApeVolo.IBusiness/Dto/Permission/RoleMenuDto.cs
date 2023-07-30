﻿using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Entity.Permission;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Menu), typeof(RoleMenuDto))]
public class RoleMenuDto
{
    [RegularExpression(@"^\+?[1-9]\d*$")]
    public long Id { get; set; }
}
