using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using UniRx.Async;

public class LeaderboardView : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private PlayFabLeaderboardEntryItem leaderboardEntryItemPrefab;
    [SerializeField] private Button tweetButton;

    private string tweetMessage;
    private Texture2D screenShotTexture;

    private async void Start()
    {
        var playerLeaderboardEntries = await GetLeaderboardWithRetry();
        SetupScrollView(playerLeaderboardEntries);
    }

    public void InitializeTweetButton(int score, Texture2D screenShotTexture)
    {
        titleText.text = $"今回のホカク：{score}人";
        tweetMessage = string.Format(TitleConstData.TweetMessageFormat, score);
        this.screenShotTexture = screenShotTexture;
        tweetButton.gameObject.SetActive(true);
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

    public async void OnClickTweet()
    {
        if (!tweetButton.isActiveAndEnabled)
        {
            return;
        }

        tweetButton.interactable = false;
        await TweetWithScreenShot.TweetManager.TweetWithScreenShot(tweetMessage, screenShotTexture);
        tweetButton.interactable = true;
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
