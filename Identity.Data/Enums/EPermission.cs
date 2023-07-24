﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity.Data.Enums;

public enum EPermission
{
    All = 1,
    [Display(Name = "Role control")]
    [Description("Role control")]
    RoleControl = 2,
}