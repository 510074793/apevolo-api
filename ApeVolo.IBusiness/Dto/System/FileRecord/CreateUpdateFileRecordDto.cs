﻿using System.ComponentModel.DataAnnotations;
using ApeVolo.Common.AttributeExt;
using ApeVolo.IBusiness.Base;

namespace ApeVolo.IBusiness.Dto.System.FileRecord;

[AutoMapping(typeof(Entity.Do.Core.FileRecord), typeof(CreateUpdateFileRecordDto))]
public class CreateUpdateFileRecordDto : EntityDtoRoot<long>
{
    [Display(Name = "FileRecord.Name")]
    [Required(ErrorMessage = "{0}required")]
    public string Description { get; set; }

    public string ContentType { get; set; }

    public string ContentTypeName { get; set; }

    public string ContentTypeNameEn { get; set; }

    public string OriginalName { get; set; }

    public string NewName { get; set; }

    public string FilePath { get; set; }

    public string Size { get; set; }
}