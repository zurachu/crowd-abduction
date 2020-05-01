using UnityEngine;

public class AbductionCircle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circle;

    public bool Contains(Human human)
    {
        var position = circle.transform.position;
        var humanPosition = human.transform.position;
        humanPosition.y = position.y;
        return Vector3.Distance(position, humanPosition) <= transform.localScale.x / 2; // 半径
    }
}
