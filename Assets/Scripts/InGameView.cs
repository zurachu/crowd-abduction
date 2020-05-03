using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text remainingCountText;

    public void UpdateView(int score, int remainingCount)
    {
        scoreText.text = $"ホカク {score}/{TitleConstData.InitialHumanCount}人";
        remainingCountText.text = $"ノコリ{remainingCount}カイ";
    }
}
