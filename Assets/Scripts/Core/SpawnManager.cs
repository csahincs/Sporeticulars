using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemies;
    
    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {
        coroutine = WaitAndSpawn(2.0f);
        StartCoroutine(coroutine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private IEnumerator WaitAndSpawn(float waitTime)
    {
        while (true && enemies.Length > 0)
        {
            yield return new WaitForSeconds(waitTime);
            
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, enemies.Length);

            int randomX = random.Next(0, 5);
            int randomY = random.Next(0, 5);


            Instantiate(enemies[randomNumber], new Vector3(5 + randomX, 5 + randomY, 0), Quaternion.identity);
        }
    }
}
