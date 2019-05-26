using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Town : MonoBehaviour
{
    public Cyclone.Particle particle = new Cyclone.Particle();
    public Cyclone.BoundingSphere body;
    public double damping = 0.995f;
    private double Health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        Health = 100f;
        particle.InverseMass = 0f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(0f, 0f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = damping;
        
        SetObjectPosition(particle.Position);

        body = new Cyclone.BoundingSphere(particle.GetPosition(), 0.75f);
    }

    // Update is called once per frame
    void Update()
    {
        particle.Integrate(Time.deltaTime);
        body.SetCenter(particle.GetPosition());
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }

    public void TakeDamage(double damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            GameManager.instance.EndLevel(GameManager.LevelEndStatus.Fail);
        }
    }
}
