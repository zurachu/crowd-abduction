using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using UniRx.Async;
using System;

public class PlayFabLeaderboardEntryItem : MonoBehaviour
{
    [SerializeField] private Image baseImage;
    [SerializeField] private Text rankText;
    [SerializeField] private Text nameText;
    [SerializeField] private Button inputNameButton;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Button updateNameButton;
    [SerializeField] private Text scoreText;

    [SerializeField] private string scoreFormat;
    [SerializeField] private Color mySelfBaseColor;
    [SerializeField] private Color minusScoreColor;

    private static readonly int nameMinimumLength = 3;

    public void Initialize(PlayerLeaderboardEntry entry, bool isMyself)
    {
        rankText.text = (entry.Position + 1).ToString();
        nameText.text = nameInputField.text = entry.DisplayName;
        scoreText.text = string.Format(scoreFormat, entry.StatValue);
        if (entry.StatValue < 0)
        {
            scoreText.color = minusScoreColor;
        }

        nameText.gameObject.SetActive(false);
        inputNameButton.gameObject.SetActive(false);
        nameInputField.gameObject.SetActive(false);
        nameInputField.text = entry.DisplayName;
        updateNameButton.gameObject.SetActive(false);
        updateNameButton.interactable = false;
        if (isMyself)
        {
            baseImage.color = mySelfBaseColor;
            if (string.IsNullOrEmpty(nameText.text))
            {
                nameInputField.gameObject.SetActive(true);
                updateNameButton.gameObject.SetActive(true);
            }
            else
            {
                nameText.gameObject.SetActive(true);
                inputNameButton.gameObject.SetActive(true);
            }
        }
        else
        {
            nameText.gameObject.SetActive(true);
        }
    }

    public void OnClickInputName()
    {
        nameText.gameObject.SetActive(false);
        inputNameButton.gameObject.SetActive(false);
        nameInputField.gameObject.SetActive(true);
        updateNameButton.gameObject.SetActive(true);
        updateNameButton.interactable = false;
    }

    public void OnInputFieldValueChanged(string text)
    {
        var lineFeedRemoved = text.Replace("\r", "").Replace("\n", "");
        nameInputField.text = lineFeedRemoved;
        updateNameButton.interactable = IsValidName(lineFeedRemoved);
    }

    public void OnClickUpdateName()
    {
        var newName = nameInputField.text;
        if (!IsValidName(newName))
        {
            return;
        }

        nameText.text = newName;
        nameText.gameObject.SetActive(true);
        inputNameButton.gameObject.SetActive(true);
        nameInputField.gameObject.SetActive(false);
        updateNameButton.gameObject.SetActive(false);
        UpdateUserTitleDisplayNameWithRetry(newName);
    }

    private bool IsValidName(string name)
    {
        return !string.IsNullOrEmpty(name) &&
            nameMinimumLength <= name.Length && name.Length <= nameInputField.characterLimit &&
            !name.Contains("\r") && !name.Contains("\n");
    }

    private async void UpdateUserTitleDisplayNameWithRetry(string newName)
    {
        while (true)
        {
            try
            {
                await PlayFabLeaderboardUtil.UpdateUserTitleDisplayNameAsync(newName);
                break;
            }
            catch (Exception)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
