﻿using ApeVolo.Common.DI;
using ApeVolo.IBusiness.Dto.Core;
using ApeVolo.IBusiness.Vo;
using System.Threading.Tasks;

namespace ApeVolo.IBusiness.Interface.Core
{
    /// <summary>
    /// 在线用户接口
    /// </summary>
    public interface IOnlineUserService : IDependencyService
    {
        #region 基础接口
        /// <summary>
        /// 保存在线用户
        /// </summary>
        /// <param name="jwtUserVo"></param>
        /// <param name="token"></param>
        Task<bool> SaveAsync(JwtUserVo jwtUserVo, string token);

        /// <summary>
        /// jwt用户信息
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        Task<JwtUserVo> FindJwtUserAsync(UserDto userDto);
        #endregion

    }
}
