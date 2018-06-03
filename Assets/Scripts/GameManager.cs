using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    public bool alwaysDrawGizmos = true;
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
        
        
        //<TODO> Add level init calls etc.. here
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
