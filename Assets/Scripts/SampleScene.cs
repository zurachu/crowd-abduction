using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    [SerializeField] private MeshRenderer ground;
    [SerializeField] private AbductionCircle abductionCircle;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private TitleView titleView;
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
        SceneManager.LoadScene("LeaderboardScene");
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

//        inputManager.gameObject.SetActive(false);
    }

    private void UpdateHudText()
    {
        hudText.text = $"ホカク {initialHumanCount - humans.Count}/{initialHumanCount}人";
    }
}
