using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleScene : MonoBehaviour
{
    [SerializeField] private MeshRenderer ground;
    [SerializeField] private Human humanPrefab;
    [SerializeField] private Text hudText;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AbductionCircle abductionCircle;

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

        inputManager.Initialize(abductionCircle, OnAbduct);
    }

    private void FixedUpdate()
    {
        if (inputManager.isActiveAndEnabled && humans != null)
        {
            humans.ForEach(_human => _human.UpdateView(abductionCircle));
        }
    }

    private void OnAbduct(AbductionCircle abductionCircle)
    {
        if (!abductionCircle.isActiveAndEnabled)
        {
            return;
        }

        humans.ForEach(_human => _human.Stop());

        var containedHumanCount = humans.FindAll(_human => abductionCircle.Contains(_human)).Count;

        hudText.text = $"{containedHumanCount}";

        inputManager.gameObject.SetActive(false);
    }
}
