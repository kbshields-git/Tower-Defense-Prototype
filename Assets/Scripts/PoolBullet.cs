using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoolBullet : PooledObject {
    public bool useTags = true;
    public bool bulletParticle = false;
    private Enemy target;
    
    public int m_BaseDamage = 1;

    public bool m_IsLit;
    [MinMaxRange(0,2)]
    public RangedFloat m_RandLightFalloffRange;
    [MinMaxRange(0,40)]
    public RangedFloat m_RandIntensityRange;
    private float m_RandLightFalloff;
    private float m_RemainingTime;
    public Light m_BulletGlow;
    public GameObject m_HitEffect;

    /// <summary>
    /// Time to wait before destroying the bullet, in the case that it misses a legitimate target and rolls off into the level
    /// </summary>
    [SerializeField] float reapTime = 10f;
    float remainingTime;

    /// <summary>
    /// Holds the starting intesnity of a bullets light, if it has one.
    /// </summary>
    private float m_Intensity;


    public void FireBullet()
    {
        if (m_IsLit)
        {
            m_Intensity = Random.Range(m_RandIntensityRange.minValue, m_RandIntensityRange.maxValue);
            m_RandLightFalloff = Random.Range(m_RandLightFalloffRange.minValue, m_RandLightFalloffRange.maxValue);
            m_RemainingTime = m_RandLightFalloff;
            InvokeRepeating("RolloffGlow", 0f, m_RemainingTime / 5);
        }
        remainingTime = reapTime;
        //Invoke("Reap", reapTime);
    }

    private void Update()
    {
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0) { ReturnToPool(); }
    }
    private void RolloffGlow()
    {       
        m_RemainingTime -= Time.deltaTime;
        m_BulletGlow.intensity = m_Intensity * (m_RemainingTime / m_RandLightFalloff);
        
    }

    void HitTarget(GameObject target)
    {
        int damageToDo = m_BaseDamage;
        target.GetComponent<Enemy>().TakeDamage(damageToDo);
        if (bulletParticle)
        {
            GameObject bullHit = (GameObject)Instantiate(m_HitEffect, target.transform.position, target.transform.rotation);
            bullHit.transform.SetParent(gameObject.transform.parent);
        }
        //Destroy(gameObject);
        ReturnToPool();

    }

    private void OnTriggerEnter(Collider col)
    {
        if (useTags)
        {
            if (col.gameObject.tag == "enemyTag")
            {
                HitTarget(col.gameObject);
            }
            if (col.gameObject.tag == "Barrier")
            {
                //Destroy(gameObject);
                ReturnToPool();
            }
        }
        else
        {
            HitTarget(col.gameObject);
        }
    }

    private void Reap()
    {
        //Destroy(gameObject);
        ReturnToPool();
    }
}
