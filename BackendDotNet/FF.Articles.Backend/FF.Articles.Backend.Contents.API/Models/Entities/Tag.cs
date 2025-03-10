﻿using FF.Articles.Backend.Common.Bases;

namespace FF.Articles.Backend.Contents.API.Models.Entities;
/// <summary>
/// Ignore BaseEntity optional columns
/// </summary>
public class Tag : BaseEntity
{
    public string TagName { get; set; }
}
