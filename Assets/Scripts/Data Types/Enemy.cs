using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Cyclone.Particle particle = new Cyclone.Particle();
    private Cyclone.Vector3 target;

    public Transform baseTransform;

    public void Start()
    {
        target = new Cyclone.Vector3(baseTransform.position.x, baseTransform.position.y, baseTransform.position.z);

        Initialize(target);
    }

    public void Initialize(Cyclone.Vector3 target)
    {
        Cyclone.Vector3 directionVector = target - particle.Position;
        directionVector.Normalize();

        particle.InverseMass = 2.0f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(directionVector.x, directionVector.y, directionVector.z);
        particle.SetAcceleration(0f, 0f, 0f);
    }


    private void FixedUpdate()
    {
        particle.Integrate(Time.deltaTime);
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }

    private void PrintVector3(Cyclone.Vector3 position)
    {
        Debug.LogError((float)position.x + ", " + (float)position.y +", " + (float)position.z);
    }
}
