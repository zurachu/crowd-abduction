using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using UniRx.Async;

public class LeaderboardView : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private PlayFabLeaderboardEntryItem leaderboardEntryItemPrefab;
    [SerializeField] private Button tweetButton;

    private async void Start()
    {
        var playerLeaderboardEntries = await GetLeaderboardWithRetry();
        SetupScrollView(playerLeaderboardEntries);
    }

    public void Initialize(/* with tweet */)
    {
        
    }

    private UniTask<List<PlayerLeaderboardEntry>> GetLeaderboardWithRetry()
    {
        var source = new UniTaskCompletionSource<List<PlayerLeaderboardEntry>>();
        Action<List<PlayerLeaderboardEntry>> onSuccess = (_entries) => source.TrySetResult(_entries);

        var statisticName = TitleConstData.LeaderboardStatisticName;
        var maxResultsCount = TitleConstData.LeaderboardEntryCount;
        PlayFabLeaderboardUtil.GetLeaderboard(statisticName, maxResultsCount, onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            var playerLeaderboardEntries = await GetLeaderboardWithRetry();
            source.TrySetResult(playerLeaderboardEntries);
        });

        return source.Task;
    }

    private void SetupScrollView(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
    {
        foreach (var entry in playerLeaderboardEntries)
        {
            var isMyself = entry.PlayFabId == PlayFabLoginManagerSingleton.Instance.PlayFabId;
            var entryItem = Instantiate(leaderboardEntryItemPrefab, scrollRect.content.transform);
            entryItem.Initialize(entry, isMyself);
        }
    }

    public void OnClickTweet()
    {
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
