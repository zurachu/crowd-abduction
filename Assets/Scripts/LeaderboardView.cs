﻿using System;
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

    private async UniTask<List<PlayerLeaderboardEntry>> GetLeaderboardWithRetry()
    {
        var statisticName = TitleConstData.LeaderboardStatisticName;
        var maxResultsCount = TitleConstData.LeaderboardEntryCount;

        while (true)
        {
            try
            {
                return await PlayFabLeaderboardUtil.GetLeaderboardAsync(statisticName, maxResultsCount);
            }
            catch (Exception)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

    private void SetupScrollView(List<PlayerLeaderboardEntry> playerLeaderboardEntries)
    {
        var content = scrollRect.content;
        RectTransform myself = null;
        foreach (var entry in playerLeaderboardEntries)
        {
            var isMyself = entry.PlayFabId == PlayFabLoginManagerSingleton.Instance.PlayFabId;
            var entryItem = Instantiate(leaderboardEntryItemPrefab, content.transform);
            entryItem.Initialize(entry, isMyself);
            if (isMyself)
            {
                myself = entryItem.GetComponent<RectTransform>();
            }
        }

        if (myself != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            var scrollRectHeight = scrollRect.GetComponent<RectTransform>().rect.height;
            var y = -myself.localPosition.y - scrollRectHeight / 2;
            y = Mathf.Max(Mathf.Min(y, content.rect.height - scrollRectHeight), 0);
            scrollRect.content.localPosition = new Vector3(0, y, 0);
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
