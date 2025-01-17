﻿using System;
using System.Collections;
using System.Collections.Generic;
using KanKikuchi.AudioManager;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SampleScene : MonoBehaviour
{
    [SerializeField] private MeshRenderer ground;
    [SerializeField] private AbductionCircle abductionCircle;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private Canvas hudRoot;
    [SerializeField] private TitleView titleView;
    [SerializeField] private InGameView inGameView;
    [SerializeField] private LeaderboardView leaderboardViewPrefab;
    [SerializeField] private InputManager inputManager;

    private int initialHumanCount;
    private List<Human> humans;
    private int remainingCount;

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

        remainingCount = TitleConstData.AbductionCount;
        inputManager.Initialize(abductionCircle, null);
        titleView.gameObject.SetActive(true);
        inGameView.gameObject.SetActive(false);
        BGMManager.Instance.Play(BGMPath.UFO);
    }

    private void FixedUpdate()
    {
        if (inputManager.isActiveAndEnabled && humans != null)
        {
            humans.ForEach(_human => _human.UpdateView(abductionCircle));
        }
    }

    public async void OnClickStartGame()
    {
        titleView.gameObject.SetActive(false);
        inGameView.gameObject.SetActive(true);
        UpdateHudText();
        await UniTask.DelayFrame(1); // ボタンタップが初回入力に誤爆しないよう
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

        abductionCircle.StartEffect();

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
        remainingCount--;
        UpdateHudText();
        SEManager.Instance.Play(SEPath.UKU);

        ChallengeNextOrEndGame().Forget();
    }

    private async UniTask ChallengeNextOrEndGame()
    {
        inputManager.gameObject.SetActive(false);

        if (remainingCount > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            inputManager.gameObject.SetActive(true);
        }
        else
        {
            var score = initialHumanCount - humans.Count;
            UpdatePlayerStatisticWithRetry(score);

            await UniTask.Delay(TimeSpan.FromSeconds(0.4));
            var screenShotTexture = await CaptureScreenShot();
            await UniTask.Delay(TimeSpan.FromSeconds(1.6));

            inputManager.gameObject.SetActive(true);
            inputManager.Initialize(abductionCircle, null);
            inGameView.gameObject.SetActive(false);
            var leaderboardView = Instantiate(leaderboardViewPrefab, hudRoot.transform);
            leaderboardView.InitializeTweetButton(score, screenShotTexture);
            BGMManager.Instance.Stop();
            SEManager.Instance.Play(SEPath.SPACESHIP2);
        }
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
        inGameView.UpdateView(initialHumanCount - humans.Count, remainingCount);
    }

    private async void UpdatePlayerStatisticWithRetry(int score)
    {
        while (true)
        {
            try
            {
                await PlayFabLeaderboardUtil.UpdatePlayerStatisticAsync(TitleConstData.LeaderboardStatisticName, score);
                break;
            }
            catch (Exception)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
