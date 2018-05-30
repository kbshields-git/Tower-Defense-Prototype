using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [SerializeField] NavMeshAgent agent;
    bool TargetSet = false;
    GameObject targetExit;

    private void Awake()
    {
        // Search the scene for the object labeled Exit
        targetExit = GameObject.FindGameObjectWithTag("Exit");
        
    }
    // Update is called once per frame
    void Update () {
        if (!TargetSet) {
            agent.SetDestination(targetExit.gameObject.transform.position);
            if (agent.hasPath)
            {
                TargetSet = true;
            }            
        }
	}
}
