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
    public GameEnd gameEnd;
    public MainMenu mainMenu;
    public Tutorial tutorial;
    public LevelEnd levelEnd;
    public LevelStart levelStart;
    public GameObject inventory;
    public GameObject timerParent;
    public Transform canvasTransform;
    public TMPro.TextMeshProUGUI timer;
    #endregion

    #region Prefabs
    public GameObject combatTxt;
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
        
        CloseGUIElements();
        CloseGameElements();
        mainMenu.Open();
    }

    public void CloseGUIElements()
    {
        mainMenu.Close();
        levelEnd.Close();
        levelStart.Close();
        gameEnd.Close();
        tutorial.Close();
        inventory.gameObject.SetActive(false);
        timerParent.gameObject.SetActive(false);
        timer.gameObject.SetActive(false);
    }

    public void CloseGameElements()
    {
        spawner.gameObject.SetActive(false);
        physics.gameObject.SetActive(false);
        player.gameObject.SetActive(false);
        town.gameObject.SetActive(false);
        groundGrid.gameObject.SetActive(false);
        trapGrid.gameObject.SetActive(false);
    }

    public void OpenGameElements()
    {
        spawner.gameObject.SetActive(true);
        physics.gameObject.SetActive(true);
        player.gameObject.SetActive(true);
        town.gameObject.SetActive(true);
        groundGrid.gameObject.SetActive(true);
        trapGrid.gameObject.SetActive(true);
        inventory.gameObject.SetActive(true);
        timerParent.gameObject.SetActive(true);
        timer.gameObject.SetActive(true);
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
        OpenGameElements();

        for (int i = 0; i < spawner.transform.childCount; i++)
            Destroy(spawner.transform.GetChild(i));

        spawner.SetNumberOfSpawn((int)(SpawnCoefficient / Influence) * Level);
        killCount = 0;
        StartCoroutine(RoundTime());
    }

    public void EndLevel(LevelEndStatus status)
    {
        CloseGameElements();
        CloseGUIElements();
        levelEnd.Initialize(status);

        LevelPassed(killCount);
        SetLevel(Level);
        SetInfluence(Influence);
        killCount = 0;
    }
    public void ResetGame()
    {
        CloseGameElements();
        CloseGUIElements();

        Level = 1;
        Influence = 100;
        SetLevel(1);
        SetInfluence(100f);
        mainMenu.Open();
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
        for(int i = 0; i < 60; i++)
        {
            timer.text = (60 - i).ToString();
            yield return new WaitForSeconds(1f);
        }
        EndLevel(LevelEndStatus.Success);
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
}
