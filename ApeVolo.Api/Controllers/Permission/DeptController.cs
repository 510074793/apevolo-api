using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using ApeVolo.Api.Controllers.Base;
using ApeVolo.Common.AttributeExt;
using ApeVolo.Common.Extention;
using ApeVolo.Common.Helper;
using ApeVolo.Common.Model;
using ApeVolo.IBusiness.Dto.Permission;
using ApeVolo.IBusiness.Interface.Permission;
using ApeVolo.IBusiness.QueryModel;
using ApeVolo.IBusiness.RequestModel;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace ApeVolo.Api.Controllers.Permission;

/// <summary>
/// 部门管理
/// </summary>
[Area("权限管理")]
[Route("/api/dept")]
public class DeptController : BaseApiController
{
    #region 构造函数

    public DeptController(IDepartmentService departmentService, IRoleDeptService roleDeptService,
        IUserService userService)
    {
        _departmentService = departmentService;
        _roleDeptService = roleDeptService;
        _userService = userService;
    }

    #endregion

    #region 字段

    private readonly IDepartmentService _departmentService;
    private readonly IRoleDeptService _roleDeptService;
    private readonly IUserService _userService;

    #endregion

    #region 对内接口

    /// <summary>
    /// 新增部门
    /// </summary>
    /// <param name="createUpdateDepartmentDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create")]
    [Description("创建")]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _departmentService.CreateAsync(createUpdateDepartmentDto);
        return Create();
    }


    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="createUpdateDepartmentDto"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("edit")]
    [Description("编辑")]
    public async Task<ActionResult<object>> Update(
        [FromBody] CreateUpdateDepartmentDto createUpdateDepartmentDto)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        await _departmentService.UpdateAsync(createUpdateDepartmentDto);
        return NoContent();
    }


    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("delete")]
    [Description("删除")]
    public async Task<ActionResult<object>> Delete([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        List<long> ids = new List<long>(idCollection.IdArray);
        var allIds = await _departmentService.GetChildIds(ids, null);


        var users = await _userService.QueryByDeptIdsAsync(allIds);
        if (users.Any())
        {
            var dept = await _departmentService.QueryByIdAsync(users.FirstOrDefault()!.DeptId);
            return Error($"所选部门({dept.Name})存在用户关联，请解除后再试！");
        }


        var rolesDepartments = await _roleDeptService.QueryByDeptIdsAsync(allIds);
        if (rolesDepartments.Any())
        {
            var dept = await _departmentService.QueryByIdAsync(rolesDepartments.FirstOrDefault()!.DeptId);
            return Error($"所选部门({dept.Name})存在角色关联，请解除后再试！");
        }


        await _departmentService.DeleteAsync(allIds);
        return Success();
    }

    /// <summary>
    /// 查看部门列表
    /// </summary>
    /// <param name="deptQueryCriteria"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("query")]
    [Description("查询")]
    public async Task<ActionResult<object>> Query(DeptQueryCriteria deptQueryCriteria,
        Pagination pagination)
    {
        var deptList = await _departmentService.QueryAsync(deptQueryCriteria, pagination);


        return new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = pagination.TotalElements
        }.ToJson();
    }

    /// <summary>
    /// 导出部门
    /// </summary>
    /// <param name="deptQueryCriteria"></param>
    /// <returns></returns>
    [HttpGet]
    [Description("导出")]
    [Route("download")]
    public async Task<ActionResult<object>> Download(DeptQueryCriteria deptQueryCriteria)
    {
        var deptExports = await _departmentService.DownloadAsync(deptQueryCriteria);
        var data = new ExcelHelper().GenerateExcel(deptExports, out var mimeType);
        return File(data, mimeType);
    }


    /// <summary>
    /// 获取同级与父级部门
    /// </summary>
    /// <param name="idCollection"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("superior")]
    [Description("获取同级、父级部门")]
    [ApeVoloAuthorize(new[] { "admin", "dept_list" })]
    public async Task<ActionResult<object>> GetSuperior([FromBody] IdCollection idCollection)
    {
        if (!ModelState.IsValid)
        {
            var actionError = ModelState.GetErrors();
            return Error(actionError);
        }

        var deptList = await _departmentService.QuerySuperiorDeptAsync(idCollection.IdArray);

        return new ActionResultVm<DepartmentDto>
        {
            Content = deptList,
            TotalElements = deptList.Count
        }.ToJson();
    }

    #endregion
}
