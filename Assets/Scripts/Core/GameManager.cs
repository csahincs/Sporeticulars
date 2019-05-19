using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    public Transform BaseTransform;
    public GameObject Player;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
       
        /*else if (instance != this)
            Destroy(gameObject);
        */
        DontDestroyOnLoad(gameObject);
        
    }

    //Update is called every frame.
    void Update()
    {

    }
}
