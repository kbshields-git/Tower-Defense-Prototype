using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    
    [SerializeField] GameObject enemyObject;
    
    
    
    [Header("Attributes")]
    public float speed = 3.5f;
    public int healthMaximum = 20;
    public int currentHealth;
    public Color _colAlive, _colDying;

    private Renderer _render;
    private MaterialPropertyBlock _propBlock;

    [Header("Navigation")]
    [SerializeField] NavMeshAgent agent;
    GameObject targetExit;
    bool TargetSet = false;

    private void Awake()
    {
        currentHealth = healthMaximum;
        _propBlock = new MaterialPropertyBlock();
        _render = enemyObject.GetComponent<Renderer>();
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
        _render.GetPropertyBlock(_propBlock);
        float lerpAmount = (float)currentHealth / (float)healthMaximum;
        
        _propBlock.SetColor("_Color", Color.Lerp(_colDying, _colAlive, lerpAmount));
        _render.SetPropertyBlock(_propBlock);

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
