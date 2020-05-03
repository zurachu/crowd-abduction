using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text versionText;

    private void Start()
    {
        descriptionText.text = TitleConstData.TitleDescription;
        versionText.text = $"Ver.{Application.version}";
    }
}
