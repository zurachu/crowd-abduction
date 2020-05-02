﻿using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    [SerializeField] private MeshRenderer ground;
    [SerializeField] private AbductionCircle abductionCircle;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private Canvas hudRoot;
    [SerializeField] private TitleView titleView;
    [SerializeField] private LeaderboardView leaderboardViewPrefab;
    [SerializeField] private Text hudText;
    [SerializeField] private InputManager inputManager;

    private int initialHumanCount;
    private List<Human> humans;

    private void Start()
    {
        if (!PlayFabLoginManagerSingleton.Instance.LoggedIn)
        {
            SceneManager.LoadScene("InitialScene");
        }

        initialHumanCount = TitleConstData.InitialHumanCount;
        humans = new List<Human>();
        for (int i = 0; i < initialHumanCount; i++)
        {
            var human = Instantiate(humanPrefab);
            human.Initialize(ground.bounds);
            humans.Add(human);
        }

        inputManager.Initialize(abductionCircle, null);
        UpdateHudText();
    }

    private void FixedUpdate()
    {
        if (inputManager.isActiveAndEnabled && humans != null)
        {
            humans.ForEach(_human => _human.UpdateView(abductionCircle));
        }
    }

    public void OnClickStartGame()
    {
        titleView.gameObject.SetActive(false);
        inputManager.Initialize(abductionCircle, OnAbduct);
    }

    public void OnClickLeaderboard()
    {
        titleView.gameObject.SetActive(false);
        Instantiate(leaderboardViewPrefab, hudRoot.transform);
    }

    private void OnAbduct(AbductionCircle abductionCircle)
    {
        if (!abductionCircle.isActiveAndEnabled)
        {
            return;
        }

        var remainingHumans = new List<Human>();
        humans.ForEach(_human =>
        {
            _human.UpdateView(abductionCircle);
            if (abductionCircle.Contains(_human))
            {
                _human.Abducted();
            }
            else
            {
                remainingHumans.Add(_human);
            }
        });

        humans = remainingHumans;
        UpdateHudText();

        UpdatePlayerStatisticWithRetry(initialHumanCount - humans.Count);
        _ = EndGame();
    }

    private async UniTask EndGame()
    {
        inputManager.gameObject.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));

        var screenShotTexture = await CaptureScreenShot();
        await UniTask.Delay(TimeSpan.FromSeconds(1));

        inputManager.gameObject.SetActive(true);
        inputManager.Initialize(abductionCircle, null);
        var leaderboardView = Instantiate(leaderboardViewPrefab, hudRoot.transform);
        leaderboardView.InitializeTweetButton(initialHumanCount - humans.Count, screenShotTexture);
    }

    private UniTask<Texture2D> CaptureScreenShot()
    {
        var source = new UniTaskCompletionSource<Texture2D>();
        StartCoroutine(CaptureScreenShot(_texture => source.TrySetResult(_texture)));
        return source.Task;
    }

    private IEnumerator CaptureScreenShot(Action<Texture2D> onCaptured)
    {
        // await UniTask.Yield(PlayerLoopTiming.PostLateUpdate) だとうまくいかなかったのでわざわざ Coroutine を噛ませる
        yield return new WaitForEndOfFrame();
        var texture = ScreenCapture.CaptureScreenshotAsTexture();
        onCaptured?.Invoke(texture);
    }

    private void UpdateHudText()
    {
        hudText.text = $"ホカク {initialHumanCount - humans.Count}/{initialHumanCount}人";
    }

    private void UpdatePlayerStatisticWithRetry(int score)
    {
        PlayFabLeaderboardUtil.UpdatePlayerStatistic(TitleConstData.LeaderboardStatisticName, score, null, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            UpdatePlayerStatisticWithRetry(score);
        });
    }
}
