using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private RectTransform mouseMoveRange;

    private AbductionCircle abductionCircle;
    Action<AbductionCircle> onAbduct;

    public void Initialize(AbductionCircle abductionCircle, Action<AbductionCircle> onAbduct)
    {
        this.abductionCircle = abductionCircle;
        this.onAbduct = onAbduct;
    }

    private void Update()
    {
        var mousePosition = Input.mousePosition;
        if (0 <= mousePosition.x && mousePosition.x < mouseMoveRange.rect.width &&
            0 <= mousePosition.y && mousePosition.y < mouseMoveRange.rect.height)
        {
            abductionCircle.gameObject.SetActive(true);
            var mainCamera = Camera.main;
            var screenPosition = new Vector3(mousePosition.x, mousePosition.y, mainCamera.transform.position.y);
            var worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
            worldPoint.y = abductionCircle.transform.position.y;
            abductionCircle.transform.position = worldPoint;

            if (Input.GetMouseButtonUp(0))
            {
                onAbduct?.Invoke(abductionCircle);
            }
        }
        else
        {
            abductionCircle.gameObject.SetActive(false);
        }
    }
}
