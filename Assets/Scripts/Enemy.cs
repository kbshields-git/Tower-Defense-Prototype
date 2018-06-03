using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [SerializeField] NavMeshAgent agent;
    bool TargetSet = false;
    GameObject targetExit;
    public float speed = 3.5f;
    public int healthMaximum = 20;
    public int currentHealth;

    private void Awake()
    {
        currentHealth = healthMaximum;
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

        if (currentHealth <= 0)
        {
            Death();
        }
	}

    public void TakeDamage(int damage)
    {
        if (currentHealth > damage)
        {
            currentHealth -= damage;
        }
        else Death();
    }

    void Death()
    {
        // <TODO> Object Pool
        Destroy(gameObject);
    }
}
