using UnityEngine;
using UnityEngine.AI;

public class Enemy : PooledObject {

    
     
    
    [Header("Attributes")]
    public float speed = 3.5f;
    public int healthMaximum = 20;
    public int currentHealth;

    public Color _colAlive, _colDying;

    private Renderer _render;
    private MaterialPropertyBlock _propBlock;
    
    [Header("Navigation")]
    NavMeshAgent agent;
    GameObject targetExit;
    bool targetSet = false;
    bool isAlive = false;

    private void Awake()
    {
        
        _propBlock = new MaterialPropertyBlock();
        _render = gameObject.GetComponent<Renderer>();
        agent = gameObject.GetComponent<NavMeshAgent>();    
        // Search the scene for the object labeled Exit
        targetExit = GameObject.FindGameObjectWithTag("Exit");

    }

    // Update is called once per frame
    void Update () {
        if (gameObject.activeInHierarchy & isAlive)
        {
            if (!targetSet)
            {
                agent.SetDestination(targetExit.gameObject.transform.position);
                if (agent.hasPath)
                {
                    targetSet = true;
                }
            }
            _render.GetPropertyBlock(_propBlock);
            float lerpAmount = (float)currentHealth / (float)healthMaximum;

            _propBlock.SetColor("_Color", Color.Lerp(_colDying, _colAlive, lerpAmount));
            _render.SetPropertyBlock(_propBlock);

            if (currentHealth <= 0)
            {
                Death();
            }
        }
	}

    public void SpawnAlive(Vector3 pos, Quaternion rot)
    {
        //This seems to fix an issue where the agent will teleport to their previous death position or somewhere randomly
        //It's some NavMesh fuckery
        agent.Warp(pos); 
        //transform.position = pos;
        transform.rotation = rot;
        currentHealth = healthMaximum;
        targetSet = false;
        isAlive = true;
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
        agent.ResetPath();
        agent.isStopped = true;
        isAlive = false;
        //transform.position = origin;        
        ReturnToPool();
    }

    private void OnDrawGizmos()
    {
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawEnemyGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}
