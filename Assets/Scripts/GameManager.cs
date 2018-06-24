using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    #region Managers
    public static GameManager instance = null;
    public GameObject buildManagerPF;
    GameObject buildManager;

    //public GameObject menuSystemPF;
    public GameObject menuSystem;
    UIManager uiManager;
    #endregion

    #region DebugVars
    public bool alwaysDrawGizmos = true;
    public bool drawBuildGizmos = true;
    public bool drawGridGizmos = true;
    public bool drawTurretGizmos = true;
    public bool drawEnemyGizmos = true;
    public bool drawSpawnGizmos = true;
    public bool hideCursor = true;
    #endregion

    // Use this for initialization
    void Awake () {
		if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (hideCursor)
        {
            Cursor.visible = false;
        }
        DontDestroyOnLoad(gameObject);
        if (buildManager == null)
        {
            buildManager = Instantiate(buildManagerPF, this.transform);
        }


        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        uiManager = menuSystem.GetComponent<UIManager>();
           
        if(sceneName == "Main Menu")
        {
            //Put uiManager into Main Menu mode.
            uiManager.MainMenu();
        }
        else
        {
            //Put the uiManager into Level mode.
            uiManager.PlayMode();
        }

        //<TODO> Add level init calls etc.. here
    }
	
	// Update is called once per frame
	void Update () {
        GetInput();
	}

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            uiManager.PlayMode();
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0;
            uiManager.PauseMenu();
            Cursor.visible = true;
        }
    }
}
