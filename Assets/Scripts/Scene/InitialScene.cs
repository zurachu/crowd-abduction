using System;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialScene : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
        TryLoginWithRetry(() =>
        {
            TryGetTitleConstDataWithRetry(() =>
            {
                SceneManager.LoadScene("SampleScene");
            });
        });
    }

    private void TryLoginWithRetry(Action onSuccess)
    {
        PlayFabLoginManagerSingleton.Instance.TryLogin(onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            TryLoginWithRetry(onSuccess);
        });
    }

    private void TryGetTitleConstDataWithRetry(Action onSuccess)
    {
        PlayFabTitleConstDataManagerSingleton.Instance.TryGetData(onSuccess, async (_) =>
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            TryGetTitleConstDataWithRetry(onSuccess);
        });
    }
}
