using UnityEngine;

public class IvySetup : MonoBehaviour
{
    private Rigidbody[] IvyPart;

    private Vector3 calculatedVelocity;

    void Start()
    {
        IvyPart = GetComponentsInChildren<Rigidbody>();
    }

    public void ApplyVelocity(Vector3 momentum)
    {
        for (int i = 0; i < IvyPart.Length; i++)
        {
            calculatedVelocity = momentum / IvyPart[i].mass;
            IvyPart[i].velocity = calculatedVelocity;


        }
    }

    public void AddVelocity(Vector3 velocity)
    {
        if (IvyPart[IvyPart.Length - 1].velocity.magnitude < 1000)
        {
            for (int i = 0; i < IvyPart.Length; i++)
            {
                IvyPart[i].velocity += velocity / IvyPart[i].mass;
            }
        }
    }
}
