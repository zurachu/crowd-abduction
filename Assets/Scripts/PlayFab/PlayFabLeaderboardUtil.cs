using System;
using System.Collections.Generic;
using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLeaderboardUtil
{
    public static void UpdatePlayerStatistic(string statisticName, int value, Action onSuccess, Action<string> onFailure)
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

        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            _result =>
            {
                DebugLogUpdateStatistics(_result);
                onSuccess?.Invoke();
            },
            _error =>
            {
                var report = _error.GenerateErrorReport();
                Debug.LogError(report);
                onFailure?.Invoke(report);
            });
    }

    private static void DebugLogUpdateStatistics(UpdatePlayerStatisticsResult result)
    {
        var request = result.Request as UpdatePlayerStatisticsRequest;
        var stringBuilder = new StringBuilder();
        foreach (var statistic in request.Statistics)
        {
            stringBuilder.AppendFormat("{0}:{1}:{2}", statistic.StatisticName, statistic.Version, statistic.Value);
        }
        Debug.Log(stringBuilder);
    }

    public static void GetLeaderboard(string statisticName, int maxResultsCount, Action<List<PlayerLeaderboardEntry>> onSuccess, Action<string> onFailure)
    {
        var request = new GetLeaderboardRequest
        {
            MaxResultsCount = maxResultsCount,
            StatisticName = statisticName,
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            _result =>
            {
                DebugLogLeaderboard(_result);
                onSuccess?.Invoke(_result.Leaderboard);
            },
            _error =>
            {
                var report = _error.GenerateErrorReport();
                Debug.LogError(report);
				onFailure?.Invoke(report);
            });
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

    public static void UpdateUserTitleDisplayName(string displayName, Action onSuccess, Action<string> onFailure)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            _result =>
            {
                Debug.Log(_result.DisplayName);
                onSuccess?.Invoke();
            },
            _error => {
                var report = _error.GenerateErrorReport();
                Debug.LogError(report);
                onFailure?.Invoke(report);
            });
    }
}
