using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelStart : MonoBehaviour
{
    public TMPro.TextMeshProUGUI levelText;
    public TMPro.TextMeshProUGUI spawnText;
    public TMPro.TextMeshProUGUI influenceText;
    public Button startBtn;
    public Button exitBtn;
    public void Initialize(int level, int spawn, float influnce)
    {
        Open();
        startBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

        startBtn.onClick.AddListener(StartBtnClicked);
        exitBtn.onClick.AddListener(ExitBtnClicked);
        
        levelText.text = "Level : " + level;
        spawnText.text = "Spawn Count = " + spawn;
        influenceText.text = "Influence = " + influnce;
    }

    void StartBtnClicked()
    {
        GameManager.instance.SetUpMap();
        Close();
    }

    void ExitBtnClicked()
    {
        GameManager.instance.tutorial.Open();
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }
}
