using UnityEngine;

public class Spawner : MonoBehaviour {
    public Enemy enemyPfab;
    public float spawnRate;
    public int spawnCount;
    public int burstCount;
    private int burstRemain;
    public float coolDown;
    private float coolRemain;
    private float remainingTime;
    private Transform lastEnemy;


    private void Start()
    {
        remainingTime = spawnRate;
        burstRemain = burstCount;
    }

    private void Update()
    {
        //This is some crude spawning code... but it'll work for now
        if (burstRemain <= 0)
        {
            coolRemain -= Time.deltaTime;
            if (coolRemain <= 0)
            {
                burstRemain = burstCount;
                coolRemain = coolDown;
            }
        }
        if (burstRemain > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                SpawnEnemy();
                remainingTime = spawnRate;
                burstRemain -= 1;
            }
        }
        if (spawnCount <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void SpawnEnemy()
    {
        // Spawn new enemy from ObjectPool
        // ToDo - Expand this to handle multiple enemy types, maybe randomly etc...
        Enemy enemy = enemyPfab.GetPooledInstance<Enemy>();
        
        //enemy.gameObject.transform.position = transform.position;
        Debug.Log(transform.position);
        //enemy.gameObject.transform.rotation = transform.rotation;
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawSpawnGizmos)
        {
            Debug.DrawLine(transform.position, enemy.gameObject.transform.position, Color.blue);
        }
        lastEnemy = enemy.transform;
        enemy.SpawnAlive(transform.position,transform.rotation);
        

        spawnCount--;
    }

    private void OnDrawGizmos()
    {
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawSpawnGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 2f);
            if (lastEnemy != null)
            {
                Gizmos.DrawLine(transform.position, lastEnemy.position);
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(lastEnemy.position, 2f);
            }
        }
    }
}
