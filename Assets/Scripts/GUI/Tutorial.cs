using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Button backBtn;
    public Button playBtn;
    // Start is called before the first frame update
    void Start()
    {
        backBtn.onClick.RemoveAllListeners();
        playBtn.onClick.RemoveAllListeners();

        backBtn.onClick.AddListener(BackBtnClicked);
        playBtn.onClick.AddListener(PlayBtnClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackBtnClicked()
    {
        GameManager.instance.mainMenu.Open();
        Close();
    }

    public void PlayBtnClicked()
    {
        GameManager.instance.SetUpLevelStart();
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
