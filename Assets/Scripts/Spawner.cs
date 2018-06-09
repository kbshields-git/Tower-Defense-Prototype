using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject enemy;
    public GameObject enemyParent;
    public GameObject parentPFab;
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
        GameObject enemyGO = (GameObject)Instantiate(enemy, transform.position, transform.rotation);
        enemyGO.transform.SetParent(transform);
        spawnCount--;
        if (spawnCount <= 0)
        {
            CancelInvoke();
        }
    }
}
