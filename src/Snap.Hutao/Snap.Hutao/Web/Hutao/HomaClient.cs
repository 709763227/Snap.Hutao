﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using Snap.Hutao.Web.Hutao.Model;
using Snap.Hutao.Web.Hutao.Model.Post;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃API客户端
/// </summary>
// [Injection(InjectAs.Transient)]
[HttpClient(HttpClientConfigration.Default)]
internal class HomaClient
{
    private const string HutaoAPI = "https://homa.snapgenshin.com";

    private readonly HttpClient httpClient;
    private readonly GameRecordClient gameRecordClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaClient> logger;

    /// <summary>
    /// 构造一个新的胡桃API客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="gameRecordClient">游戏记录客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public HomaClient(HttpClient httpClient, GameRecordClient gameRecordClient, JsonSerializerOptions options, ILogger<HomaClient> logger)
    {
        this.httpClient = httpClient;
        this.gameRecordClient = gameRecordClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步检查对应的uid当前是否上传了数据
    /// GET /Record/CheckRecord/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>当前是否上传了数据</returns>
    public async Task<bool> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        Response<bool>? resp = await httpClient
            .GetFromJsonAsync<Response<bool>>($"{HutaoAPI}/Record/Check?uid={uid}", token)
            .ConfigureAwait(false);

        return resp?.Data == true;
    }

    /// <summary>
    /// 异步获取排行信息
    /// GET /Record/Rank/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>排行信息</returns>
    public async Task<RankInfo?> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        Response<RankInfo>? resp = await httpClient
               .GetFromJsonAsync<Response<RankInfo>>($"{HutaoAPI}/Record/Rank?uid={uid}", token)
               .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取总览数据
    /// GET /Statistics/Overview
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>总览信息</returns>
    public async Task<Overview?> GetOverviewAsync(CancellationToken token = default)
    {
        Response<Overview>? resp = await httpClient
            .GetFromJsonAsync<Response<Overview>>($"{HutaoAPI}/Statistics/Overview", token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取角色出场率
    /// GET /Statistics/Avatar/AttendanceRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async Task<List<AvatarAppearanceRank>> GetAvatarAttendanceRatesAsync(CancellationToken token = default)
    {
        Response<List<AvatarAppearanceRank>>? resp = await httpClient
            .GetFromJsonAsync<Response<List<AvatarAppearanceRank>>>($"{HutaoAPI}/Statistics/Avatar/AttendanceRate", token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色使用率
    /// GET /Statistics/Avatar/UtilizationRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async Task<List<AvatarUsageRank>> GetAvatarUtilizationRatesAsync(CancellationToken token = default)
    {
        Response<List<AvatarUsageRank>>? resp = await httpClient
            .GetFromJsonAsync<Response<List<AvatarUsageRank>>>($"{HutaoAPI}/Statistics/Avatar/UtilizationRate", token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色/武器/圣遗物搭配
    /// GET /Statistics/Avatar/AvatarCollocation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色/武器/圣遗物搭配</returns>
    public async Task<List<AvatarCollocation>> GetAvatarCollocationsAsync(CancellationToken token = default)
    {
        Response<List<AvatarCollocation>>? resp = await httpClient
            .GetFromJsonAsync<Response<List<AvatarCollocation>>>($"{HutaoAPI}/Statistics/Avatar/AvatarCollocation", token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色命座信息
    /// GET /Statistics/Avatar/HoldingRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async Task<List<AvatarConstellationInfo>> GetAvatarHoldingRatesAsync(CancellationToken token = default)
    {
        Response<List<AvatarConstellationInfo>>? resp = await httpClient
            .GetFromJsonAsync<Response<List<AvatarConstellationInfo>>>($"{HutaoAPI}/Statistics/Avatar/HoldingRate", token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// GET /Team/Combination
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<List<TeamAppearance>> GetTeamCombinationsAsync(CancellationToken token = default)
    {
        Response<List<TeamAppearance>>? resp = await httpClient
            .GetFromJsonAsync<Response<List<TeamAppearance>>>($"{HutaoAPI}/Statistics/Team/Combination", token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色的深渊记录
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家记录</returns>
    public async Task<SimpleRecord> GetPlayerRecordAsync(Snap.Hutao.Model.Binding.User user, CancellationToken token = default)
    {
        PlayerInfo? playerInfo = await gameRecordClient
            .GetPlayerInfoAsync(user, token)
            .ConfigureAwait(false);
        Must.NotNull(playerInfo!);

        List<Character> characters = await gameRecordClient
            .GetCharactersAsync(user, playerInfo, token)
            .ConfigureAwait(false);

        SpiralAbyss? spiralAbyssInfo = await gameRecordClient
            .GetSpiralAbyssAsync(user, SpiralAbyssSchedule.Current, token)
            .ConfigureAwait(false);
        Must.NotNull(spiralAbyssInfo!);

        return new(Must.NotNull(user.SelectedUserGameRole!).GameUid, characters, spiralAbyssInfo);
    }

    /// <summary>
    /// 异步上传记录
    /// POST /Record/Upload
    /// </summary>
    /// <param name="playerRecord">玩家记录</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    public Task<Response<string>?> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        return httpClient.TryCatchPostAsJsonAsync<SimpleRecord, Response<string>>($"{HutaoAPI}/Record/Upload", playerRecord, options, logger, token);
    }
}