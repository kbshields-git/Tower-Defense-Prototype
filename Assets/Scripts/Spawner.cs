using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject enemy;
    public float spawnRate;
    public int spawnCount;

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnRate);
    }

    private void Update()
    {
        
    }

    void SpawnEnemy()
    {
        Instantiate(enemy, transform.position, transform.rotation);
        spawnCount--;
        if (spawnCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
