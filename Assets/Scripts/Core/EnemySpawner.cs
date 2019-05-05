using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform enemyParent;
    public Enemy enemy;




    public float waitTime;


    private void Start()
    {
        
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            Enemy enemyClone = (Instantiate(enemyPrefab) as GameObject).GetComponent<Enemy>();
            enemyClone.transform.SetParent(enemyParent, false);
            enemyClone.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

}
