using UnityEngine;

public class Demon : MonoBehaviour
{
    public Enemy demon;

    private Cyclone.Particle particle = new Cyclone.Particle();
    public double damping = 0.995f;
    
    private Transform target;

    void Start()
    {
        Enemy demon = new Enemy(100f, 5f);
        target = GameManager.instance.BaseTransform;

        particle.InverseMass = 2.0f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(0f, 0f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = damping;

        SetObjectPosition(particle.Position);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Cyclone.Vector3 direction = new Cyclone.Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z);
        direction.Normalize();
        particle.SetVelocity(direction.x, direction.y, direction.z);
        
        particle.Integrate(Time.deltaTime);
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }
}
