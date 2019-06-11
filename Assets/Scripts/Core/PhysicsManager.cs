using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public List<GameObject> boundingSpheres;
    public List<GameObject> wallBoundingSpheres;
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

                bool isKilled = false;
                if (!demon.demon.GetOnCollision())
                {
                    if(demon.demon.GetHealth() <= player.player.GetSelectedWeapon().GetDamage())
                    {
                        boundingSpheres.Remove(sphere);
                        GameObject combatTxt = Instantiate(GameManager.instance.combatTxt);
                        combatTxt.transform.SetParent(GameManager.instance.canvasTransform);
                        combatTxt.GetComponent<RectTransform>().localScale = Vector3.one / 2;
                        combatTxt.GetComponent<CombatText>().Initialize(player.particle.GetPosition(), "KILLED");
                        isKilled = true;
                    }
                    
                    demon.demon.TakeDamage(player.player.GetSelectedWeapon().GetDamage());
                    
                }
                
                demon.demon.TakeStun(player.player.GetSelectedWeapon().GetStunPower());
                if(!demon.demon.GetOnCollision() && demon.demon.GetStatus() == Enemy.StatusEnum.Stunned)
                {
                    GameObject combatTxt = Instantiate(GameManager.instance.combatTxt);
                    combatTxt.transform.SetParent(GameManager.instance.canvasTransform);
                    combatTxt.GetComponent<RectTransform>().localScale = Vector3.one / 2;
                    combatTxt.GetComponent<CombatText>().Initialize(player.particle.GetPosition(), "STUNNED");
                }
                else if (!demon.demon.GetOnCollision() && !isKilled)
                {
                    GameObject combatTxt = Instantiate(GameManager.instance.combatTxt);
                    combatTxt.transform.SetParent(GameManager.instance.canvasTransform);
                    combatTxt.GetComponent<RectTransform>().localScale = Vector3.one / 2;
                    combatTxt.GetComponent<CombatText>().Initialize(player.particle.GetPosition(), "DAMAGED");
                }
                demon.demon.SetOnCollision(true);
            }
            else
            {
                demon.demon.SetOnCollision(false);
            }

            if(town.body.Overlaps(demon.body))
            {
                boundingSpheres.Remove(sphere);
                demon.demon.TakeDamage(100f);
                town.TakeDamage(demon.demon.GetDamage());
            }
        }


        for (int i = 0; i < wallBoundingSpheres.Count; i++)
        {
            GameObject sphere = wallBoundingSpheres[i];
            Wall wall = sphere.GetComponent<Wall>();
            if (player.body.Overlaps(wall.body))
            {
                Cyclone.ParticleContact particleContact = new Cyclone.ParticleContact();

                particleContact.particle[0] = wall.particle;
                particleContact.particle[1] = player.particle;

                particleContact.ParticleMovement[0] = wall.particle.GetVelocity();
                particleContact.ParticleMovement[0] = player.particle.GetVelocity();

                particleContact.ContactNormal = wall.body.Center - player.body.Center;
                particleContact.Penetration = (wall.body.Center - player.body.Center).Magnitude - player.body.Radius - wall.body.Radius;

                Cyclone.ParticleContact[] contacts = new Cyclone.ParticleContact[1];
                contacts[0] = particleContact;
                contactResolver.ResolveContacts(contacts, 1, Time.fixedDeltaTime);
            }

        }
    }
}
