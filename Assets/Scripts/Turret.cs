using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class Turret : MonoBehaviour {



    [Header("Projectiles")]
    /// <summary>
    /// Bullet gameObject
    /// </summary>
    public GameObject bulletPrefab;
    public Transform bulletParent;

    /// <summary>
    /// Object pool of bullets, to avoid overhead of creation/destruction at runtime
    /// </summary>
    private Bullet[] bulletClip;
    //private List<>

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
    public AudioSource m_turretAudio;
    public AudioEvent m_Firing;

    [Header("Tracking")]
    /// <summary>
    /// Target currently focused by this turret
    /// </summary>
    public GameObject currentTarget;
    public Transform sightedTarget;

    /// <summary>
    /// Boolean to avoid turrets firing while being placed into the world.
    /// </summary>
    bool hasBeenPlaced = false;

    /// <summary>
    /// Boolean set when a clear shot has been determined.
    /// </summary>
    public bool hasClearShot = false;

    /// <summary>
    /// List<T> of all currently eligible (inside Trigger sphere) targets
    /// </summary>
    [SerializeField] protected List<GameObject> potentialTargets = new List<GameObject>();
    [SerializeField] protected List<GameObject> sortList = new List<GameObject>();

    // Use this for initialization
    void Start () {
        currentTarget = null;
        hasBeenPlaced = false;
        gameObject.layer = 2; // Move to ignore raycast layer until Turret is placed.
        rangeTrigger.radius = range;
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Bail if the turret has not yet been built.
        if (!hasBeenPlaced) { return; }
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
            if (ClearShot(currentTarget))
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            else { currentTarget = null; }
        }

        fireCountdown -= Time.deltaTime;
    }

    public void Build()
    {
        hasBeenPlaced = true;
        gameObject.layer = 10;
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
        if (other.tag == "enemyTag")
        {
            potentialTargets.Add(other.gameObject);
            //If we don't have a valid target yet, check to see if we have LoS on this target and make it currentTarget
            if (currentTarget == null)
            {
                if (ClearShot(other.gameObject))
                {
                    currentTarget = other.gameObject;
                }
            }
        }
    }
    /*
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
    */

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
                if (ClearShot(targ))
                {
                    shortestDistance = distanceToTarget;
                    nearestTarget = targ;
                }
            }

        }
        if (nearestTarget != null)
        {
            currentTarget = nearestTarget;
        }
    }

    void AddSortListBack()
    {
        foreach(var targ in sortList)
        {
            potentialTargets.Remove(targ);
        }
        potentialTargets.AddRange(sortList);
        sortList.Clear();
    }

    bool ClearShot(GameObject targ)
    {
        hasClearShot = false;
        float aimHeightOffset = 0.5f;
        RaycastHit hit;
        Vector3 dir = targ.transform.position - transform.position;
        dir.y -= aimHeightOffset;
        Ray ray = new Ray(barrelTip.position, dir);
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawTurretGizmos)
        {
            Debug.DrawRay(ray.origin, ray.direction * 5000, Color.yellow);
        }
        if(Physics.Raycast(ray, out hit, range))
        {
            sightedTarget = hit.transform;
            if(hit.transform == targ.transform || hit.transform.tag == "enemyTag")
            {
                hasClearShot = true;
            }            
        }

        return hasClearShot;
    }
    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();
        bulletGO.transform.SetParent(transform);

        if (bullet != null)
        {
            Enemy enemyToShoot = currentTarget.GetComponent<Enemy>();
            bullet.m_Velocity = fireVelocity;
            bullet.m_BaseDamage = baseDamage;
            m_Firing.Play(m_turretAudio);
            //bullet.AcquireTarget(enemyToShoot);
            
            bulletGO.GetComponent<Rigidbody>().velocity = fireVelocity * barrelTip.forward;
        }
            
    }

    private void OnDrawGizmos()
    {
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawTurretGizmos)
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
