using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemies;

    public int numberOfSpawns;
    private IEnumerator coroutine;

    void Start()
    {
        coroutine = WaitAndSpawn(GetNumberOfSpawn());
        StartCoroutine(coroutine);
    }

    private void Update()
    {
        
    }

    private IEnumerator WaitAndSpawn(float waitTime)
    {
        yield return new WaitForSeconds(5f);
        int spawnCount = 0;
        while (true && enemies.Length > 0 && spawnCount < numberOfSpawns)
        {
            yield return new WaitForSeconds(waitTime / 2f);
            
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, enemies.Length);

            int randomX = random.Next(-5, 5);
            int randomY = random.Next(-5, 5);


            Instantiate(enemies[randomNumber], new Vector3(5 + randomX, 5 + randomY, 0), Quaternion.identity, transform);
            GameManager.instance.aliveCount++;
            spawnCount++;
            waitTime--;
        }

        StartCoroutine(CheckLevelEnd());
    }

    private IEnumerator CheckLevelEnd()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            if(GameManager.instance.aliveCount == 0)
            {
                GameManager.instance.EndLevel(GameManager.LevelEndStatus.Success);
            }
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
