﻿using ApeVolo.Common.AttributeExt;
using SqlSugar;

namespace ApeVolo.Entity.Do.Core
{
    [InitTable(typeof(RoleMenu))]
    [SugarTable("sys_roles_menus", "角色与菜单关联表")]
    public class RoleMenu
    {
        [SugarColumn(ColumnName = "role_id", ColumnDataType = "char", Length = 19, IsNullable = false, IsPrimaryKey = true)]
        public string RoleId { get; set; }

        [SugarColumn(ColumnName = "menu_id", ColumnDataType = "char", Length = 19, IsNullable = false, IsPrimaryKey = true)]
        public string MenuId { get; set; }
    }
}
