using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private List<Material> defaultMaterials;
    [SerializeField] private List<Material> containedMaterials;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Animator animator;


    private bool UnderAbstruction => animator.speed <= 0;
    private bool CompletedAbstruction => transform.position.y >= gamingCameraY;

    private Bounds bounds;
    private float gamingCameraY;

    private void FixedUpdate()
    {
        if (UnderAbstruction)
        {
            if (CompletedAbstruction)
            {
                Destroy(gameObject);
            }

            return;
        }

        var position = transform.position;
        var velocity = rigidbody.velocity;

        if (position.x < bounds.min.x && velocity.x < 0 || position.x > bounds.max.x && velocity.x > 0)
        {
            velocity.x = -velocity.x;
        }

        if (position.z < bounds.min.z && velocity.z < 0 || position.z > bounds.max.z && velocity.z > 0)
        {
            velocity.z = -velocity.z;
        }

        rigidbody.velocity = velocity;

        var cross = Vector3.Cross(Vector3.forward, rigidbody.velocity);
        var angle = Vector3.Angle(Vector3.forward, rigidbody.velocity);
        angle *= cross.y > 0 ? +1 : -1;
        animator.transform.localRotation = Quaternion.Euler(0f, angle, 0f);

        animator.SetBool("Grounded", true);
        animator.SetFloat("MoveSpeed", rigidbody.velocity.magnitude);
    }

    public void Initialize(Bounds bounds)
    {
        this.bounds = bounds;
        gamingCameraY = Camera.main.transform.position.y;

        var position = transform.position;
        position.x = Random.Range(bounds.min.x, bounds.max.x);
        position.z = Random.Range(bounds.min.z, bounds.max.z);
        transform.position = position;
        rigidbody.velocity = NormalizedVelocity(Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward);
    }

    public void UpdateView(AbductionCircle abductionCircle)
    {
        var inCircle = abductionCircle.isActiveAndEnabled && abductionCircle.Contains(this);
        rigidbody.velocity = NormalizedVelocity(rigidbody.velocity, inCircle);
        var materialList = inCircle ? containedMaterials : defaultMaterials;
        skinnedMeshRenderer.materials = materialList.ToArray();
    }

    public void Abducted()
    {
        transform.localRotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        rigidbody.velocity = Vector3.up * TitleConstData.HumanVelocityOnAbducted;
        animator.speed = 0f;
    }

    private Vector3 NormalizedVelocity(Vector3 velocity, bool inCircle = false)
    {
        return velocity.normalized * (inCircle ? TitleConstData.HumanVelocityInCircle : TitleConstData.HumanVelocity);
    }
}
