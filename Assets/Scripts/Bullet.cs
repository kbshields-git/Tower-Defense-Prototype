using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Enemy target;

    public float m_Velocity = 10f;
    public int m_BaseDamage = 10;

    public void AcquireTarget(Enemy _target)
    {
        target = _target;
    }
	
	// Update is called once per frame
	void Update () {

        if (target == null)
        {
            // <TODO> Use an object pool to manage bullets for efficiency
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.gameObject.transform.position - transform.position;
        float distanceThisFrame = m_Velocity * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);		
	}

    void HitTarget()
    {
        int damageToDo = m_BaseDamage;
        target.TakeDamage(damageToDo);
        Destroy(gameObject);
    }
}
