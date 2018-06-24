using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject mainMenuOptions;

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject pauseMenuOptions;
    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void PlayMode()
    {
        SetAllMenusInactive();
    }

    public void PauseMenu()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    void SetAllMenusInactive()
    {
        mainMenu.SetActive(false);
        mainMenuOptions.SetActive(false);
        pauseMenu.SetActive(false);
        pauseMenuOptions.SetActive(false);
    }
}
