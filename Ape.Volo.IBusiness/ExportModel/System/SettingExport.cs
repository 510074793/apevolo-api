﻿using System.ComponentModel.DataAnnotations;
using Ape.Volo.Common.Global;
using Ape.Volo.Common.Model;

namespace Ape.Volo.IBusiness.ExportModel.System;

public class SettingExport : ExportBase
{
    [Display(Name = "键")]
    public string Name { get; set; }

    [Display(Name = "值")]
    public string Value { get; set; }

    [Display(Name = "是否启用")]
    public EnabledState EnabledState { get; set; }

    [Display(Name = "描述")]
    public string Description { get; set; }
}
