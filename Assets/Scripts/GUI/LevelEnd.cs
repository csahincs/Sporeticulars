using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEnd : MonoBehaviour
{
    public TMPro.TextMeshProUGUI statusText;
    public Button startBtn;
    public Button exitBtn;
    public void Initialize(GameManager.LevelEndStatus status)
    {
        Open();
        startBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

        startBtn.onClick.AddListener(StartBtnClicked);
        exitBtn.onClick.AddListener(ExitBtnClicked);

        if (status == GameManager.LevelEndStatus.Fail)
            statusText.text = "You have failed";
        else
            statusText.text = "Congratulations";
    }

    void StartBtnClicked()
    {
        GameManager.instance.SetUpMap();
        gameObject.SetActive(false);
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
