﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using MiniExcelLibs.Attributes;
using Snap.Hutao.Core.Json.Converter;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// UIGF物品
/// </summary>
public class UIGFItem : GachaLogItem
{
    /// <summary>
    /// 额外祈愿映射
    /// </summary>
    [ExcelColumn(Name = "uigf_gacha_type")]
    [JsonPropertyName("uigf_gacha_type")]
    [JsonConverter(typeof(EnumStringValueConverter<GachaConfigType>))]
    public GachaConfigType UIGFGachaType { get; set; } = default!;
}