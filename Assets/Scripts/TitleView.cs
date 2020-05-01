using UnityEngine;
using UnityEngine.UI;

public class TitleView : MonoBehaviour
{
    [SerializeField] Text descriptionText;
    [SerializeField] Text versionText;

    private void Start()
    {
        descriptionText.text = TitleConstData.TitleDescription;
        versionText.text = $"Ver.{Application.version}";
    }
}
