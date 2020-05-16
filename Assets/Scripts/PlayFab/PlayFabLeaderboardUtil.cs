using System;
using System.Collections.Generic;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UniRx.Async;
using UnityEngine;

public class PlayFabLeaderboardUtil
{
    public static UniTask<UpdatePlayerStatisticsResult> UpdatePlayerStatisticAsync(string statisticName, int value)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>() {
                new StatisticUpdate {
                    StatisticName = statisticName,
                    Value = value
                }
            }
        };

        var source = new UniTaskCompletionSource<UpdatePlayerStatisticsResult>();
        Action<UpdatePlayerStatisticsResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.UpdatePlayerStatistics(request, resultCallback, errorCallback);
        return source.Task;
    }

    public static UniTask<List<PlayerLeaderboardEntry>> GetLeaderboardAsync(string statisticName, int maxResultsCount)
    {
        var request = new GetLeaderboardRequest
        {
            MaxResultsCount = maxResultsCount,
            StatisticName = statisticName,
        };

        var source = new UniTaskCompletionSource<List<PlayerLeaderboardEntry>>();
        Action<GetLeaderboardResult> resultCallback = (_result) =>
        {
            DebugLogLeaderboard(_result);
            source.TrySetResult(_result.Leaderboard);
        };
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.GetLeaderboard(request, resultCallback, errorCallback);
        return source.Task;
    }

    private static void DebugLogLeaderboard(GetLeaderboardResult result)
    {
        var stringBuilder = new StringBuilder();
        foreach (var entry in result.Leaderboard)
        {
            stringBuilder.AppendFormat("{0}:{1}:{2}:{3}\n", entry.Position, entry.StatValue, entry.PlayFabId, entry.DisplayName);
        }
        Debug.Log(stringBuilder);
    }

    public static UniTask<UpdateUserTitleDisplayNameResult> UpdateUserTitleDisplayNameAsync(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        var source = new UniTaskCompletionSource<UpdateUserTitleDisplayNameResult>();
        Action<UpdateUserTitleDisplayNameResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, resultCallback, errorCallback);
        return source.Task;
    }
}
