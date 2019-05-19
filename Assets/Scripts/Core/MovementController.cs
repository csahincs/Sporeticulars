using UnityEngine;

public class MovementController : MonoBehaviour
{
    public Transform anchor;
    
    private Cyclone.ParticleForceRegistry registry = new Cyclone.ParticleForceRegistry();
    private Cyclone.Particle particle = new Cyclone.Particle();
    public double damping = 0.995f;

    private void Start()
    {
        particle.InverseMass = 2.0f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(0f, 0f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = damping;

        Cyclone.Vector3 anchorPosition = new Cyclone.Vector3(anchor.transform.position.x, anchor.transform.position.y, anchor.transform.position.z);
        Cyclone.ParticleAnchoredSpring anchoredSpring = new Cyclone.ParticleAnchoredSpring(anchorPosition, 1.0f, 10.0f);

        SetObjectPosition(particle.Position);
    }

    private void FixedUpdate()
    {
        registry.UpdateForces(Time.deltaTime);
        particle.Integrate(Time.deltaTime);
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
        anchor.transform.position = new Vector3((float)position.x, (float)position.y, -10f);
    }
    
    void OnGUI()
    {
        KeyCode key = Event.current.keyCode;

        switch (key)
        {
            case KeyCode.W: particle.AddForce(new Cyclone.Vector3(0f, 1f, 0f)); break;
            case KeyCode.S: particle.AddForce(new Cyclone.Vector3(0f, -1f, 0f)); break;
            case KeyCode.A: particle.AddForce(new Cyclone.Vector3(-1f, 0f, 0f)); break;
            case KeyCode.D: particle.AddForce(new Cyclone.Vector3(1f, 0f, 0f)); break;
        }
    }

}
