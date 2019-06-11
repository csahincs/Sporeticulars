using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{
    public Cyclone.Particle particle = new Cyclone.Particle();
    public Text combatTxt;


    public void Initialize(Cyclone.Vector3 position, string text)
    {
        particle.InverseMass = 0.1f;
        particle.SetPosition(4.5, 20f, 0);
        particle.SetVelocity(0f, 50f, 0f);
        particle.SetAcceleration(0f, 0f, 0f);
        particle.Damping = 0.995f;

        combatTxt.text = text;
        StartCoroutine(CombatTxtLifeTime());

        SetObjectPosition(particle.Position);
    }

    private void FixedUpdate()
    {
        particle.Integrate(Time.deltaTime);
        SetObjectPosition(particle.Position);
    }

    private void SetObjectPosition(Cyclone.Vector3 position)
    {
        transform.localPosition = new Vector3((float)position.x, (float)position.y, (float)position.z);
    }

    private IEnumerator CombatTxtLifeTime()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

}
