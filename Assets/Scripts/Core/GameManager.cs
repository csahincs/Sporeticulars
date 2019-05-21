using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Managers
    public static GameManager instance = null;
    public SpawnManager spawner;
    //public PhysicsManager physics;
    #endregion

    #region Town
    public GameObject town;
    #endregion

    #region Player
    public GameObject player;
    #endregion

    #region Map
    public GameObject groundGrid;
    public GameObject trapGrid;
    #endregion

    #region Parameters
    public int SpawnCoefficient = 1000;
    private int Level;
    private float Influence;
    #endregion

    #region GUI Elements
    public MainMenu mainMenu;
    #endregion


    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
            instance = this;

        Level = GetLevel();
        Influence = GetInfluence();
    }
    
    public void LevelPassed(int killCount)
    {
        SetLevel(Level + 1);
        SetInfluence(Influence - (killCount / 10));
    }

    public void SetUpMap()
    {
        Instantiate(spawner);
        //Instantiate(physics);
        Instantiate(player);
        Instantiate(town);
        Instantiate(groundGrid);
        Instantiate(trapGrid);

        spawner.SetNumberOfSpawn((int)(SpawnCoefficient / Influence) * Level);
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
