using UnityEngine;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public Transform anchor;
    
    private Cyclone.ParticleForceRegistry registry = new Cyclone.ParticleForceRegistry();
    public Cyclone.Particle particle = new Cyclone.Particle();

    public double damping = 0.995f;

    public Player player;
    public Cyclone.BoundingSphere body;
    
    private void Start()
    {
        player = new Player();
        player.SetSelectedWeapon(GameManager.instance.weapons[0]);
        
        particle.InverseMass = 0.1f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(0f, 0f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = damping;

        Cyclone.Vector3 anchorPosition = new Cyclone.Vector3(anchor.transform.position.x, anchor.transform.position.y, anchor.transform.position.z);
        Cyclone.ParticleAnchoredSpring anchoredSpring = new Cyclone.ParticleAnchoredSpring(anchorPosition, 1.0f, 10.0f);

        SetObjectPosition(particle.Position);

        body = new Cyclone.BoundingSphere(particle.GetPosition(), 0.3f);
    }

    private void FixedUpdate()
    {
        registry.UpdateForces(Time.deltaTime);
        particle.Integrate(Time.deltaTime);
        body.SetCenter(particle.GetPosition());
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
        anchor.transform.position = new Vector3((float)position.x, (float)position.y, -10f);
    }

    public void PrintPosition()
    {
        Debug.Log("x : " + particle.Position.x + "y : " + particle.Position.y + "z : " + particle.Position.z);
    }
    
    void OnGUI()
    {
        KeyCode key = Event.current.keyCode;

        switch (key)
        {
            case KeyCode.W: particle.AddForce(new Cyclone.Vector3(0f, 20f, 0f)); break;
            case KeyCode.S: particle.AddForce(new Cyclone.Vector3(0f, -20f, 0f)); break;
            case KeyCode.A: particle.AddForce(new Cyclone.Vector3(-20f, 0f, 0f)); break;
            case KeyCode.D: particle.AddForce(new Cyclone.Vector3(20f, 0f, 0f)); break;

            case KeyCode.Space: Debug.Log(player.GetSelectedWeapon()); break;
        }
    }



}
