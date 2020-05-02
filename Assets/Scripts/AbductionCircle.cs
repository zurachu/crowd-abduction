using UnityEngine;

public class AbductionCircle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circle;
    [SerializeField] private ParticleSystem particleSystem;

    private void Start()
    {
        var radius = TitleConstData.Radius;
        circle.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        particleSystem.Stop();
        var particleShape = particleSystem.shape;
        particleShape.radius = radius;
    }

    public bool Contains(Human human)
    {
        var position = circle.transform.position;
        var humanPosition = human.transform.position;
        humanPosition.y = position.y;
        return Vector3.Distance(position, humanPosition) <= transform.localScale.x / 2;
    }

    public void StartEffect()
    {
        particleSystem.Play();
    }
}
