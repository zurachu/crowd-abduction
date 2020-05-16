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
        await LoginAsyncWithRetry();
        await GetTitleConstDataAsyncWithRetry();
        SceneManager.LoadScene("SampleScene");
    }

    private async UniTask LoginAsyncWithRetry()
    {
        while (true)
        {
            try
            {
                await PlayFabLoginManagerSingleton.Instance.TryLoginAsync();
                break;
            }
            catch (Exception)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }

    private async UniTask GetTitleConstDataAsyncWithRetry()
    {
        while (true)
        {
            try
            {
                await PlayFabTitleConstDataManagerSingleton.Instance.TryGetDataAsync();
                break;
            }
            catch (Exception)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
