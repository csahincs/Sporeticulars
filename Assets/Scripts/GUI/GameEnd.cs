using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    public TMPro.TextMeshProUGUI gameEndText;
    public Button startBtn;
    public Button exitBtn;

    // Start is called before the first frame update
    public void Initialize(GameManager.LevelEndStatus status)
    {
        Open();
        startBtn.onClick.RemoveAllListeners();
        exitBtn.onClick.RemoveAllListeners();

        startBtn.onClick.AddListener(StartBtnClicked);
        exitBtn.onClick.AddListener(ExitBtnClicked);

        if (status == GameManager.LevelEndStatus.Fail)
            gameEndText.text = "You've lost control of the Town :(";
        else
            gameEndText.text = "You'are the leader of the Town";

    }

    void StartBtnClicked()
    {
        GameManager.instance.ResetGame();
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
