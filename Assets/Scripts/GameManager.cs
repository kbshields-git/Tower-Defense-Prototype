using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
    public GameObject buildManagerPF;
    GameObject buildManager;
    public bool alwaysDrawGizmos = true;
    public bool drawBuildGizmos = true;
    public bool drawGridGizmos = true;
    public bool drawTurretGizmos = true;
    public bool drawEnemyGizmos = true;
    public bool drawSpawnGizmos = true;

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

        DontDestroyOnLoad(gameObject);
        if (buildManager == null)
        {
            buildManager = Instantiate(buildManagerPF, this.transform);
        }

        //<TODO> Add level init calls etc.. here
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
