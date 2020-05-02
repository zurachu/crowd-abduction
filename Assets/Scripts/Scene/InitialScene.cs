using System;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
    private async void Start()
    {
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
        AudioClipManagerSingleton.Instance.Preload();
        await LoginWithRetry();
        await GetTitleConstDataWithRetry();
        SceneManager.LoadScene("SampleScene");
    }

    private UniTask LoginWithRetry()
    {
        var source = new UniTaskCompletionSource();
        Action onSuccess = () => source.TrySetResult();

        PlayFabLoginManagerSingleton.Instance.TryLogin(onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await LoginWithRetry();
            source.TrySetResult();
        });

        return source.Task;
    }

    private UniTask GetTitleConstDataWithRetry()
    {
        var source = new UniTaskCompletionSource();
        Action onSuccess = () => source.TrySetResult();

        PlayFabTitleConstDataManagerSingleton.Instance.TryGetData(onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await GetTitleConstDataWithRetry();
            source.TrySetResult();
        });

        return source.Task;
    }
}
