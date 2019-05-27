using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Managers
    public static GameManager instance = null;
    public SpawnManager spawner;
    public PhysicsManager physics;
    #endregion

    #region Town
    public GameObject town;
    #endregion

    #region Player
    public GameObject player;
    public List<Weapon> weapons;
    #endregion

    #region Map
    public GameObject groundGrid;
    public GameObject trapGrid;
    #endregion

    #region Parameters
    public int SpawnCoefficient = 1000;
    public int killCount = 0;
    public int aliveCount = 0;
    private int Level;
    private float Influence;
    #endregion

    #region GUI Elements
    public MainMenu mainMenu;
    public LevelEnd levelEnd;
    public LevelStart levelStart;
    public GameEnd gameEnd;
    public GameObject inventory;
    #endregion

    public enum LevelEndStatus { Success, Fail };

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;

        Level = GetLevel();
        Influence = GetInfluence();

        weapons = new List<Weapon>();
        Sword sword = new Sword();
        DullBlade dullBlade = new DullBlade();

        weapons.Add(sword);
        weapons.Add(dullBlade);
    }
    
    public void LevelPassed(int killCount)
    {
        Level += 1;
        Influence -= (killCount * Level);

        SetLevel(Level);
        SetInfluence(Influence);
    }

    public void SetUpLevelStart()
    {
        Debug.Log(Level);
        if(Level >= 5)
        {
            if (Influence < 50)
                gameEnd.Initialize(LevelEndStatus.Fail);
            else
                gameEnd.Initialize(LevelEndStatus.Success);
            return;
        }
        levelStart.Initialize(Level, (int)(SpawnCoefficient / Influence) * Level, Influence);
    }

    public void SetUpMap()
    {
        spawner.gameObject.SetActive(true);
        physics.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        town.gameObject.SetActive(true);
        groundGrid.gameObject.SetActive(true);
        trapGrid.gameObject.SetActive(true);
        inventory.gameObject.SetActive(true);


        for (int i = 0; i < spawner.transform.childCount; i++)
            Destroy(spawner.transform.GetChild(i));

        spawner.SetNumberOfSpawn((int)(SpawnCoefficient / Influence) * Level);
        killCount = 0;
        StartCoroutine(RoundTime());
    }

    public void EndLevel(LevelEndStatus status)
    {
        spawner.gameObject.SetActive(false);
        physics.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        town.gameObject.SetActive(false);
        groundGrid.gameObject.SetActive(false);
        trapGrid.gameObject.SetActive(false);
        inventory.gameObject.SetActive(false);
        levelEnd.Initialize(status);

        LevelPassed(killCount);
        SetLevel(Level);
        SetInfluence(Influence);
        killCount = 0;
    }

    public int GetLevel()
    {
        return PlayerPrefs.GetInt("Level", 1);
    }
    public float GetInfluence()
    {
        return PlayerPrefs.GetFloat("Influence", 100);
    }
    public void SetLevel(int level)
    {
        PlayerPrefs.SetInt("Level", level);
    }
    public void SetInfluence(float influence)
    {
        PlayerPrefs.SetFloat("Influence", influence);
    }

    public void ResetGame()
    {
        spawner.gameObject.SetActive(false);
        physics.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        town.gameObject.SetActive(false);
        groundGrid.gameObject.SetActive(false);
        trapGrid.gameObject.SetActive(false);
        inventory.gameObject.SetActive(false);

        Level = 1;
        Influence = 100;
        SetLevel(1);
        SetInfluence(100f);
        mainMenu.gameObject.SetActive(true);
    }


    void OnGUI()
    {
        KeyCode key = Event.current.keyCode;
        
        switch (key)
        {
            case KeyCode.Alpha1: player.GetComponent<MovementController>().player.SetSelectedWeapon(weapons[0]);  break;
            case KeyCode.Alpha2: player.GetComponent<MovementController>().player.SetSelectedWeapon(weapons[1]); break;
        }
    }

    private IEnumerator RoundTime()
    {
        yield return new WaitForSeconds(60f);
        EndLevel(LevelEndStatus.Success);
    }
}
