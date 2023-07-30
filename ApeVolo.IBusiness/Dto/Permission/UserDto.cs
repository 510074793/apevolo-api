﻿using System;
using System.Collections.Generic;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;
using Newtonsoft.Json;

namespace ApeVolo.IBusiness.Dto.Permission;

[AutoMapping(typeof(Entity.Permission.User), typeof(UserDto))]
public class UserDto : BaseEntityDto<long>
{
    public UserDto()
    {
        Roles = new List<RoleSmallDto>();
        PermissionUrl = new List<string>();
        Authorizes = new List<string>();
        Jobs = new List<JobSmallDto>();
    }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 内置管理员
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 签名随机盐
    /// </summary>
    public string SaltKey { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public long DeptId { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 头像文件名称
    /// </summary>
    public string AvatarName { get; set; }

    /// <summary>
    /// 头像文件路径
    /// </summary>
    public string AvatarPath { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public bool Sex { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 最后修改密码时间
    /// </summary>
    public DateTime? PasswordReSetTime { get; set; }

    /// <summary>
    /// 用户的权限点
    /// </summary>
    public List<RoleSmallDto> Roles { get; set; }

    /// <summary>
    /// 部门
    /// </summary>
    public DepartmentSmallDto Dept { get; set; }

    public List<JobSmallDto> Jobs { get; set; }

    /// <summary>
    /// 权限
    /// </summary>
    [JsonIgnore]
    public List<string> Authorizes { get; set; }

    /// <summary>
    /// 用户的权限url
    /// </summary>
    [JsonIgnore]
    public List<string> PermissionUrl { get; set; }
}
