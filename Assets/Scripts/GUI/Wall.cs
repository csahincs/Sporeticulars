using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Cyclone.Particle particle = new Cyclone.Particle();
    public Cyclone.BoundingSphere body;
    // Start is called before the first frame update
    void Start()
    {
        particle.InverseMass = 0f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(0f, 0f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = 1f;

        body = new Cyclone.BoundingSphere(particle.GetPosition(), 0.45f);
        GameManager.instance.physics.wallBoundingSpheres.Add(gameObject);

        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
