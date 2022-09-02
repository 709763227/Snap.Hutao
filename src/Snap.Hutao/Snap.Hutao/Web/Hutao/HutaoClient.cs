﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
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
internal class HutaoClient : ISupportAsyncInitialization
{
    private const string AuthHost = "https://auth.snapgenshin.com";
    private const string HutaoAPI = "https://hutao-api.snapgenshin.com";

    private readonly HttpClient httpClient;
    private readonly GameRecordClient gameRecordClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    private bool isInitialized = false;

    /// <summary>
    /// 构造一个新的胡桃API客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="gameRecordClient">游戏记录客户端</param>
    /// <param name="jsonSerializerOptions">json序列化选项</param>
    public HutaoClient(
        HttpClient httpClient,
        GameRecordClient gameRecordClient,
        JsonSerializerOptions jsonSerializerOptions)
    {
        this.httpClient = httpClient;
        this.gameRecordClient = gameRecordClient;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <inheritdoc/>
    public bool IsInitialized { get => isInitialized; private set => isInitialized = value; }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
    {
        if (!IsInitialized)
        {
            Auth auth = new(
                "08da6c59-da3b-48dd-8cf3-e3935a7f1d4f",
                "ox5dwglSXYgenK2YBc8KrAVPoQbIJ4eHfUciE+05WfI=");

            HttpResponseMessage response = await httpClient
                .PostAsJsonAsync($"{AuthHost}/Auth/Login", auth, jsonSerializerOptions, token)
                .ConfigureAwait(false);
            Response<Token>? resp = await response.Content
                .ReadFromJsonAsync<Response<Token>>(jsonSerializerOptions, token)
                .ConfigureAwait(false);

            httpClient.DefaultRequestHeaders.Authorization = new("Bearer", Must.NotNull(resp?.Data?.AccessToken!));
            IsInitialized = true;
        }

        return true;
    }

    /// <summary>
    /// 异步检查对应的uid当前是否上传了数据
    /// GET /Record/CheckRecord/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>当前是否上传了数据</returns>
    public async Task<bool> CheckPeriodRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<UploadStatus>? resp = await httpClient
            .GetFromJsonAsync<Response<UploadStatus>>($"{HutaoAPI}/Record/CheckRecord/{uid}", token)
            .ConfigureAwait(false);

        return resp is { Data: not null, Data.PeriodUploaded: true };
    }

    /// <summary>
    /// 异步获取排行信息
    /// GET /Record/Rank/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>排行信息</returns>
    public async Task<RankInfoWrapper?> GetRankInfoAsync(PlayerUid uid, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<RankInfoWrapper>? resp = await httpClient
               .GetFromJsonAsync<Response<RankInfoWrapper>>($"{HutaoAPI}/Record/Rank/{uid}", token)
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
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<Overview>? resp = await httpClient
            .GetFromJsonAsync<Response<Overview>>($"{HutaoAPI}/Statistics/Overview", token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取角色出场率
    /// GET /Statistics/AvatarParticipation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async Task<IEnumerable<AvatarParticipation>> GetAvatarParticipationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarParticipation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarParticipation>>>($"{HutaoAPI}/Statistics/AvatarParticipation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色使用率
    /// GET /Statistics2/AvatarParticipation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async Task<IEnumerable<AvatarParticipation>> GetAvatarParticipations2Async(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarParticipation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarParticipation>>>($"{HutaoAPI}/Statistics2/AvatarParticipation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色圣遗物搭配
    /// GET /Statistics/AvatarReliquaryUsage
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色圣遗物搭配</returns>
    public async Task<IEnumerable<AvatarReliquaryUsage>> GetAvatarReliquaryUsagesAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarReliquaryUsage>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarReliquaryUsage>>>($"{HutaoAPI}/Statistics/AvatarReliquaryUsage", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色搭配数据
    /// GET /Statistics/TeamCollocation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色搭配数据</returns>
    public async Task<IEnumerable<TeamCollocation>> GetTeamCollocationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCollocation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<TeamCollocation>>>($"{HutaoAPI}/Statistics/TeamCollocation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色武器搭配数据
    /// GET /Statistics/AvatarWEaponUsage
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色武器搭配数据</returns>
    public async Task<IEnumerable<AvatarWeaponUsage>> GetAvatarWeaponUsagesAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarWeaponUsage>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarWeaponUsage>>>($"{HutaoAPI}/Statistics/AvatarWeaponUsage", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色命座信息
    /// GET /Statistics/Constellation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async Task<IEnumerable<AvatarConstellation>> GetAvatarConstellationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarConstellation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarConstellation>>>($"{HutaoAPI}/Statistics/Constellation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取队伍出场次数 层间
    /// GET /Statistics/TeamCombination
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination>> GetTeamCombinationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCombination>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<TeamCombination>>>($"{HutaoAPI}/Statistics/TeamCombination", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取队伍出场次数 层
    /// GET /Statistics2/TeamCombination
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination2>> GetTeamCombinations2Async(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCombination2>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<TeamCombination2>>>($"{HutaoAPI}/Statistics2/TeamCombination", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步按角色列表异步获取推荐队伍
    /// POST /Statistics2/TeamRecommanded
    /// </summary>
    /// <param name="floor">楼层</param>
    /// <param name="avatarIds">期望的角色，按期望出现顺序排序</param>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination2>> GetRecommandedTeamCombination2sAsync(int floor, IEnumerable<string> avatarIds, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        DesiredInfo desiredInfo = new(floor, avatarIds);

        HttpResponseMessage response = await httpClient
            .PostAsJsonAsync($"{HutaoAPI}/Statistics2/TeamRecommanded", desiredInfo, jsonSerializerOptions, token)
            .ConfigureAwait(false);

        Response<IEnumerable<TeamCombination2>>? resp = await response.Content
            .ReadFromJsonAsync<Response<IEnumerable<TeamCombination2>>>(jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色的深渊记录
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家记录</returns>
    public async Task<PlayerRecord> GetPlayerRecordAsync(Snap.Hutao.Model.Binding.User user, CancellationToken token = default)
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

        return PlayerRecord.Create(Must.NotNull(user.SelectedUserGameRole!).GameUid, characters, spiralAbyssInfo);
    }

    /// <summary>
    /// 异步上传记录
    /// POST /Record/Upload
    /// </summary>
    /// <param name="playerRecord">玩家记录</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal async Task<Response<string>?> UploadRecordAsync(PlayerRecord playerRecord, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        HttpResponseMessage response = await httpClient
            .PostAsJsonAsync($"{HutaoAPI}/Record/Upload", playerRecord, jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return await response.Content
            .ReadFromJsonAsync<Response<string>>(jsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    private class Auth
    {
        public Auth(string appid, string secret)
        {
            Appid = appid;
            Secret = secret;
        }

        public string Appid { get; }

        public string Secret { get; }
    }

    private class Token
    {
        public string AccessToken { get; } = default!;
    }
}
