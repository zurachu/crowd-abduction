﻿using System;
using PlayFab;
using PlayFab.ClientModels;
using UniRx.Async;
using UnityEngine;

public class PlayFabLoginManagerSingleton
{
    public static PlayFabLoginManagerSingleton Instance
    {
        get
        {
            instance = instance ?? new PlayFabLoginManagerSingleton();
            return instance;
        }
    }

    public string PlayFabId => result.PlayFabId;
    public bool LoggedIn => result != null;

    private static readonly string GuidKey = "Guid";

    private static PlayFabLoginManagerSingleton instance;

    private LoginResult result;

    public async UniTask<LoginResult> TryLoginAsync()
    {
        // Inspector で設定
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            throw new Exception("PlayFabSettings.TitleId is not set");
        }

#if !UNITY_EDITOR && UNITY_ANDROID
        result = await TryLoginAndroidAsync();
#else
        result = await TryLoginDefaultAsync();
#endif
        return result;
    }

    private UniTask<LoginResult> TryLoginAndroidAsync()
    {
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        var secure = new AndroidJavaClass("android.provider.Settings$Secure");
        var androidId = secure.CallStatic<string>("getString", contentResolver, "android_id");

        Debug.Log(androidId);

        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = androidId,
            CreateAccount = true
        };

        var source = new UniTaskCompletionSource<LoginResult>();
        Action<LoginResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, resultCallback, errorCallback);
        return source.Task;
    }

    private UniTask<LoginResult> TryLoginDefaultAsync()
    {
        // WebGL では端末 ID 的なものがなく、スコアランキング程度で Facebook 等連携してもらうのもユーザに手間をかけるので、
        // 簡易な端末 ID もどきとして。
        var guid = PlayerPrefs.GetString(GuidKey);
        if (string.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString("D");
            PlayerPrefs.SetString(GuidKey, guid);
            PlayerPrefs.Save();
        }

        Debug.Log(guid);

        var request = new LoginWithCustomIDRequest
        {
            CustomId = guid,
            CreateAccount = true
        };

        var source = new UniTaskCompletionSource<LoginResult>();
        Action<LoginResult> resultCallback = (_result) => source.TrySetResult(_result);
        Action<PlayFabError> errorCallback = (_error) => source.TrySetException(new Exception(_error.GenerateErrorReport()));
        PlayFabClientAPI.LoginWithCustomID(request, resultCallback, errorCallback);
        return source.Task;
    }
}
