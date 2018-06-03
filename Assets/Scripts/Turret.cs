using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class Turret : MonoBehaviour {



    [Header("Projectiles")]
    /// <summary>
    /// Bullet gameObject
    /// </summary>
    public GameObject bulletPrefab;


    [Header("Attributes")]
    /// <summary>
    /// Range of the turret
    /// </summary>
    public float range = 15f;

    public float pivotSpeed = 10f;

    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public float fireVelocity = 100f;
    public int baseDamage = 1;


    [Header("Turret Parts")]
    [SerializeField] Transform pivotPart;
    [SerializeField] Transform barrelTip;
    /// <summary>
    /// Collider to detect enemies in range
    /// </summary>
    public SphereCollider rangeTrigger;

    [Header("Tracking")]
    /// <summary>
    /// Target currently focused by this turret
    /// </summary>
    public GameObject currentTarget;
    /// <summary>
    /// List<T> of all currently eligible (inside Trigger sphere) targets
    /// </summary>
    [SerializeField] protected List<GameObject> potentialTargets = new List<GameObject>();

    // Use this for initialization
    void Start () {
        currentTarget = null;
        rangeTrigger.radius = range;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (currentTarget == null)
        {
            IdleRotation();
            currentTarget = null;
            if (potentialTargets.Count > 0)
            {
                FindNextTarget();
            }
           
            return;
        }

        // Rotate turret in direction of current target
        LockOnTarget();

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    private void IdleRotation()
    {
        // For now, I'm just letting the turret rotate back to a straight forward position to keep it simple
        // <TODO>: Make turret swivel around slowly when not actively engaging a target
        Vector3 rotation = Quaternion.Lerp(pivotPart.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * pivotSpeed).eulerAngles;
        pivotPart.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    private void LockOnTarget()
    {
        Vector3 dir = currentTarget.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(pivotPart.rotation, lookRotation, Time.deltaTime * pivotSpeed).eulerAngles;
        pivotPart.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        potentialTargets.Add(other.gameObject);
        if (currentTarget == null) { currentTarget = other.gameObject; }
    }

    private void OnTriggerExit(Collider other)
    {
        potentialTargets.Remove(other.gameObject);
        if (currentTarget == other.gameObject)
        {
            currentTarget = null;
            if (potentialTargets.Count > 0)
            {
                FindNextTarget();
            }
        }

    }

    void FindNextTarget()
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestTarget = null;
        // Clean nulls out of the list
        potentialTargets.RemoveAll(item => item == null);
        foreach (var targ in potentialTargets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targ.transform.position);
            if (distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = targ;
            }
        }
        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;
        }
    }

    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            Enemy enemyToShoot = currentTarget.GetComponent<Enemy>();
            bullet.m_Velocity = fireVelocity;
            bullet.m_BaseDamage = baseDamage;
            bullet.AcquireTarget(enemyToShoot);
        }
            
    }

    private void OnDrawGizmos()
    {
        if (GameManager.instance.alwaysDrawGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
            if (currentTarget != null)
            {
                Gizmos.DrawWireSphere(currentTarget.transform.position, 2f);
                
                Gizmos.DrawRay(pivotPart.position, pivotPart.forward);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(pivotPart.position, currentTarget.transform.position);
            }
        }
    }
}
