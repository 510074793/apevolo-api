﻿using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.User;

[AutoMapping(typeof(Entity.Do.Core.Department), typeof(UserDept))]
public class UserDept : RootId<long>
{
}