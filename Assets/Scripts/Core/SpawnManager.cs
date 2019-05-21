using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemies;

    public int numberOfSpawns;
    private IEnumerator coroutine;

    void Start()
    {
        coroutine = WaitAndSpawn(2.0f);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitAndSpawn(float waitTime)
    {
        int spawnCount = 0;
        while (true && enemies.Length > 0 && spawnCount < numberOfSpawns)
        {
            yield return new WaitForSeconds(waitTime);
            
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, enemies.Length);

            int randomX = random.Next(0, 5);
            int randomY = random.Next(0, 5);


            Instantiate(enemies[randomNumber], new Vector3(5 + randomX, 5 + randomY, 0), Quaternion.identity);
            spawnCount++;
        }
    }

    public void SetNumberOfSpawn(int number)
    {
        numberOfSpawns = number;
    }
    public int GetNumberOfSpawn()
    {
        return numberOfSpawns;
    }

}
