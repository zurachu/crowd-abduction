using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private List<Material> defaultMaterials;
    [SerializeField] private List<Material> containedMaterials;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Animator animator;

    private Bounds bounds;

    private void FixedUpdate()
    {
        if (animator.speed <= 0f)
        {
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

        rigidbody.velocity = NormalizedVelocity(velocity);

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

        var position = transform.position;
        position.x = Random.Range(bounds.min.x, bounds.max.x);
        position.z = Random.Range(bounds.min.z, bounds.max.z);
        transform.position = position;
        rigidbody.velocity = NormalizedVelocity(Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward);
    }

    public void UpdateView(AbductionCircle abductionCircle)
    {
        var materialList = defaultMaterials;
        if (abductionCircle.isActiveAndEnabled && abductionCircle.Contains(this))
        {
            materialList = containedMaterials;
        }

        skinnedMeshRenderer.materials = materialList.ToArray();
    }

    public void Stop()
    {
        rigidbody.velocity = Vector3.zero;
        animator.speed = 0f;
    }

    private Vector3 NormalizedVelocity(Vector3 velocity)
    {
        return velocity.normalized * 1f;
    }
}
