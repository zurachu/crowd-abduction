using UnityEngine;

public class AbductionCircle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circle;

    private void Start()
    {
        var diameter = TitleConstData.Radius * 2;
        circle.transform.localScale = new Vector3(diameter, diameter, 1);
    }

    public bool Contains(Human human)
    {
        var position = circle.transform.position;
        var humanPosition = human.transform.position;
        humanPosition.y = position.y;
        return Vector3.Distance(position, humanPosition) <= transform.localScale.x / 2;
    }
}
