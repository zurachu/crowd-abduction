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
        await TryLoginWithRetry();
        await TryGetTitleConstDataWithRetry();
        SceneManager.LoadScene("SampleScene");
    }

    private UniTask TryLoginWithRetry()
    {
        var source = new UniTaskCompletionSource();
        Action onSuccess = () => source.TrySetResult();

        PlayFabLoginManagerSingleton.Instance.TryLogin(onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await TryLoginWithRetry();
        });

        return source.Task;
    }

    private UniTask TryGetTitleConstDataWithRetry()
    {
        var source = new UniTaskCompletionSource();
        Action onSuccess = () => source.TrySetResult();

        PlayFabTitleConstDataManagerSingleton.Instance.TryGetData(onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await TryGetTitleConstDataWithRetry();
        });

        return source.Task;
    }
}
