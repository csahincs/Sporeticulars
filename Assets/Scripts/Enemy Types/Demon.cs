using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Demon : MonoBehaviour
{
    public Enemy demon;
    public Cyclone.BoundingSphere body;

    public Cyclone.Particle particle = new Cyclone.Particle();
    public double damping = 0.995f;
    
    private Transform target;

    private IEnumerator coroutine;
    private bool waitingStunDuration = false;

    void Start()
    {
        demon = new Enemy(100f, 10f);
        
        target = GameManager.instance.town.transform;

        particle.InverseMass = 2.0f;
        particle.SetPosition(transform.position.x, transform.position.y, transform.position.z);
        particle.SetVelocity(0f, 0f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = damping;

        SetObjectPosition(particle.Position);
        body = new Cyclone.BoundingSphere(particle.GetPosition(), 0.25f);
        GameManager.instance.physics.boundingSpheres.Add(gameObject);
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (demon.GetHealth() <= 0)
        {
            GameManager.instance.killCount++;
            GameManager.instance.aliveCount--;
            Destroy(gameObject);
        }

        if (demon.GetStatus() == Enemy.StatusEnum.Stunned && !waitingStunDuration)
        {
            coroutine = WaitForStun(3.5f);
            StartCoroutine(coroutine);
        }

        Cyclone.Vector3 direction = new Cyclone.Vector3(target.position.x - transform.position.x, target.position.y - transform.position.y, target.position.z - transform.position.z);
        direction.Normalize();


        Cyclone.Vector3 normalizedVelocity = particle.GetVelocity();
        normalizedVelocity.Normalize();


        if (demon.GetStatus() != Enemy.StatusEnum.Stunned)
        {
            if (normalizedVelocity.Magnitude == 0)
            {
                particle.AddForce(direction);
            }
            else if (!normalizedVelocity.IsEqualWithEpsilon(direction))
            {
                Cyclone.Vector3 reversedVelocity = new Cyclone.Vector3(-1 * particle.GetVelocity().x, -1 * particle.GetVelocity().y, -1 * particle.GetVelocity().z);
                particle.AddForce(reversedVelocity);
            }
            else if(particle.GetVelocity().Magnitude < 2f)
            {
                particle.AddForce(direction);
            }
        }
        

        particle.Integrate(Time.deltaTime);
        body.SetCenter(particle.GetPosition());
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.position = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }

    private IEnumerator WaitForStun(float waitTime)
    {
        waitingStunDuration = true;
        yield return new WaitForSeconds(waitTime);
        waitingStunDuration = false;

        demon.ResetStunMeter();
        demon.SetStatus(Enemy.StatusEnum.Normal);
    }
}
