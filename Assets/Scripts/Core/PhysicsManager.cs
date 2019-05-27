using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public List<GameObject> boundingSpheres;
    public MovementController player;
    public Town town;

    private Cyclone.ParticleContactResolver contactResolver = new Cyclone.ParticleContactResolver(1);
    // Start is called before the first frame update
    void Start()
    {
        boundingSpheres = new List<GameObject>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for(int i = 0; i < boundingSpheres.Count; i++)
        {
            GameObject sphere = boundingSpheres[i];
            Demon demon = sphere.GetComponent<Demon>();
            if (player.body.Overlaps(demon.body))
            {
                Cyclone.ParticleContact particleContact = new Cyclone.ParticleContact();

                particleContact.particle[0] = demon.particle;
                particleContact.particle[1] = player.particle;

                particleContact.ParticleMovement[0] = demon.particle.GetVelocity();
                particleContact.ParticleMovement[0] = player.particle.GetVelocity();

                particleContact.ContactNormal = demon.body.Center - player.body.Center;
                particleContact.Penetration = (demon.body.Center - player.body.Center).Magnitude - player.body.Radius - demon.body.Radius;

                Cyclone.ParticleContact[] contacts = new Cyclone.ParticleContact[1];
                contacts[0] = particleContact;
                contactResolver.ResolveContacts(contacts, 1, Time.fixedDeltaTime);

                if (demon.demon.GetHealth() <= player.player.GetSelectedWeapon().GetDamage())
                {
                    boundingSpheres.Remove(sphere);
                    if (!demon.waitingDamageDuration)
                    {
                        demon.demon.TakeDamage(player.player.GetSelectedWeapon().GetDamage());
                        StartCoroutine(demon.WaitForDamage());
                    }
                    demon.demon.TakeStun(player.player.GetSelectedWeapon().GetStunPower());
                    continue;
                }
                else
                {
                    if (!demon.waitingDamageDuration)
                    {
                        demon.demon.TakeDamage(player.player.GetSelectedWeapon().GetDamage());
                        StartCoroutine(demon.WaitForDamage());
                    }
                    demon.demon.TakeStun(player.player.GetSelectedWeapon().GetStunPower());
                }
            }

            if(town.body.Overlaps(demon.body))
            {
                boundingSpheres.Remove(sphere);
                demon.demon.TakeDamage(100f);
                town.TakeDamage(demon.demon.GetDamage());
            }
        }
    }
}
