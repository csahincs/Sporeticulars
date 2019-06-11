using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startBtn;
    public Button exitBtn;
    
    void Start()
    {
        startBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();
        
        startBtn.onClick.AddListener(StartBtnClicked);
        exitBtn.onClick.AddListener(ExitBtnClicked);
    }

    void StartBtnClicked()
    {
        GameManager.instance.tutorial.Open();
        Close();
    }

    void ExitBtnClicked()
    {
        Application.Quit();
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
