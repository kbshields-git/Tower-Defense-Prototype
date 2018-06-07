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
    private float m_RandLightFalloff;
    private float m_RemainingTime;
    public Light m_BulletGlow;

    /// <summary>
    /// Holds the starting intesnity of a bullets light, if it has one.
    /// </summary>
    private float m_Intensity;

 
	void Awake()
    {
        if (m_IsLit)
        {
            m_Intensity = m_BulletGlow.intensity;
            m_RandLightFalloff = Random.Range(m_RandLightFalloffRange.minValue, m_RandLightFalloffRange.maxValue);
            m_RemainingTime = m_RandLightFalloff;
        }
    }

	// Update is called once per frame
	void Update ()
    {

        if (target == null)
        {
            // <TODO> Use an object pool to manage bullets for efficiency
            Destroy(gameObject);
            return;
        }

        if (m_IsLit)
        {
            RolloffGlow();
        }

        Fly();
    }

    private void RolloffGlow()
    {       
        m_RemainingTime -= Time.deltaTime;
        Debug.Log(m_RemainingTime / m_RandLightFalloff);
        m_BulletGlow.intensity = m_Intensity * (m_RemainingTime / m_RandLightFalloff);
        
    }

    private void Fly()
    {
        Vector3 dir = target.gameObject.transform.position - transform.position;
        float distanceThisFrame = m_Velocity * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    public void AcquireTarget(Enemy _target)
    {
        target = _target;
    }

    void HitTarget()
    {
        int damageToDo = m_BaseDamage;
        target.TakeDamage(damageToDo);
        Destroy(gameObject);
    }
}
