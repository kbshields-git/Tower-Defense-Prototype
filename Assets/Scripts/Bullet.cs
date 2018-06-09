using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour {

    private Enemy target;
    

    public float m_Velocity = 10f;
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
    /// Holds the starting intesnity of a bullets light, if it has one.
    /// </summary>
    private float m_Intensity;

 
	void Awake()
    {
        if (m_IsLit)
        {
            m_Intensity = Random.Range(m_RandIntensityRange.minValue, m_RandIntensityRange.maxValue);
            m_RandLightFalloff = Random.Range(m_RandLightFalloffRange.minValue, m_RandLightFalloffRange.maxValue);
            m_RemainingTime = m_RandLightFalloff;
        }
    }

    private void RolloffGlow()
    {       
        m_RemainingTime -= Time.deltaTime;
        Debug.Log(m_RemainingTime / m_RandLightFalloff);
        m_BulletGlow.intensity = m_Intensity * (m_RemainingTime / m_RandLightFalloff);
        
    }

    void HitTarget(GameObject target)
    {
        int damageToDo = m_BaseDamage;
        target.GetComponent<Enemy>().TakeDamage(damageToDo);
        GameObject bullHit = (GameObject)Instantiate(m_HitEffect, target.transform.position, target.transform.rotation);        
        Destroy(gameObject);        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "enemyTag")
        {
            HitTarget(col.gameObject);
        }
        //Destroy(gameObject);
    }
}
