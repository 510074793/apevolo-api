﻿using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.Permission.Job;

[AutoMapping(typeof(Entity.Do.Core.Job), typeof(JobDto))]
public class JobDto : EntityDtoRoot<long>
{
    [Display(Name = "Job.Name")]
    public string Name { get; set; }

    [Display(Name = "Job.Sort")]
    public int Sort { get; set; }

    [Display(Name = "Job.Enabled")]
    public bool Enabled { get; set; }
}