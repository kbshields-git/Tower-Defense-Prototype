using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class RiggedTurret : MonoBehaviour {
    // Created a seperate Turret class to branch away from my goofy Unity Asset turrets to Blender Rigged turrets
    // I was making too many weird code choices to decide if it was one or the other...
    [Header("Projectiles")]
    /// <summary>
    /// Bullet gameObject
    /// </summary>
    //public GameObject bulletPrefab;
    //public Transform bulletParent;
    public PoolBullet bulletPrefab;

    /// <summary>
    /// Object pool of bullets, to avoid overhead of creation/destruction at runtime
    /// </summary>
    //private PoolBullet bulletClip;
    //private List<>

    [Header("Attributes")]
    /// <summary>
    /// Range of the turret
    /// </summary>
    public float range = 15f;

    public float basePivotSpeed = 3f;
    public float elePivotSpeed = 2f;
    public float headPivotSpeed = 20f;

    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public float fireVelocity = 100f;
    public int baseDamage = 1;


    [Header("Turret Parts")]
    [SerializeField] Transform rootSwivel;
    [SerializeField] Transform elevatorArmSwivel;
    [SerializeField] Transform turretHeadSwivel;
    [SerializeField] Transform barrelTip;

    // 3 Transforms for capturing inital resting positions of turret
    [SerializeField] Quaternion restRootSwivel;
    [SerializeField] Quaternion restEleArmSwivel;
    [SerializeField] Quaternion restTurretHeadSwivel;

    /// <summary>
    /// Collider to detect enemies in range
    /// </summary>
    public SphereCollider rangeTrigger;
    public AudioSource m_turretAudio;
    public AudioEvent m_Firing;
    public AudioEvent buildEffect;

    [Header("Tracking")]
    /// <summary>
    /// Target currently focused by this turret
    /// </summary>
    public GameObject currentTarget;
    public Transform sightedTarget;

    /// <summary>
    /// Boolean to avoid turrets firing while being placed into the world.
    /// </summary>
    [SerializeField] bool hasBeenPlaced = false;

    /// <summary>
    /// Boolean set when a clear shot has been determined.
    /// </summary>
    public bool hasClearShot = false;

    /// <summary>
    /// List<T> of all currently eligible (inside Trigger sphere) targets
    /// </summary>
    [SerializeField] protected List<GameObject> potentialTargets = new List<GameObject>();
    [SerializeField] protected List<GameObject> sortList = new List<GameObject>();
    [SerializeField] private LayerMask enemyMask = 1 << 12;
    [SerializeField] private int checkSurroundingsCount;
    [SerializeField] private float checkFrequency=2f;
    [SerializeField] private float timeSinceLastCheck;
    private void Start()
    {
        restRootSwivel = rootSwivel.rotation;
        restEleArmSwivel = elevatorArmSwivel.rotation;
        restTurretHeadSwivel = turretHeadSwivel.rotation;
    }
    // Use this for initialization
    void Awake () {
        currentTarget = null;
        //hasBeenPlaced = true;
        gameObject.layer = 2; // Move to ignore raycast layer until Turret is placed.
        rangeTrigger.radius = range;
        checkSurroundingsCount = 0;

        // Set these one time and then use them to return to rest


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
            else
            {
                //Debug.Log("Checking Surroundings because potentialTargets.Count>0 is false.");
                CheckSurroundings();
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
            else
            {
                currentTarget = null;
                //Debug.Log("Checking Surroundings because no ClearShot()");
                CheckSurroundings();
            }
        }

        fireCountdown -= Time.deltaTime;
    }

    public void Build()
    {
        hasBeenPlaced = true;
        gameObject.layer = 10;
        buildEffect.Play(m_turretAudio);
        CheckSurroundings();
    }

    private void IdleRotation()
    {
        // For now, I'm just letting the turret rotate back to a straight forward position to keep it simple
        // <TODO>: Make turret swivel around slowly when not actively engaging a target
        Vector3 horRotation = Quaternion.Lerp(rootSwivel.rotation, restRootSwivel, Time.deltaTime * basePivotSpeed).eulerAngles;
        Vector3 elevRotation = Quaternion.Lerp(elevatorArmSwivel.rotation, restEleArmSwivel, Time.deltaTime * elePivotSpeed).eulerAngles;
        Vector3 headRotation = Quaternion.Lerp(turretHeadSwivel.rotation, restTurretHeadSwivel, Time.deltaTime * headPivotSpeed).eulerAngles;

        rootSwivel.rotation = Quaternion.Euler(rootSwivel.rotation.x, horRotation.y, rootSwivel.rotation.z);
        elevatorArmSwivel.rotation = Quaternion.Euler(elevRotation.x, elevRotation.y, elevRotation.z);
        turretHeadSwivel.rotation = Quaternion.Euler(turretHeadSwivel.rotation.x, headRotation.y, turretHeadSwivel.rotation.z);
        //turretHeadSwivel.rotation = Quaternion.Euler(turretHeadSwivel.rotation.x, turretHeadSwivel.rotation.y, turretHeadSwivel.rotation.z);
    }

    private void CheckSurroundings()
    {
        LayerMask mask = 1<<12;
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= checkFrequency)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, range, mask.value);
            timeSinceLastCheck = 0;
            checkSurroundingsCount++;
        
        int i = 0;
            if (hitColliders.Length > 0)
            {

                while (i < hitColliders.Length)
                {
#if UNITY_EDITOR
                    if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawTurretGizmos)
                    {
                        Debug.DrawLine(transform.position, hitColliders[i].gameObject.transform.position, Color.red);
                    }
#endif
                    potentialTargets.Add(hitColliders[i].gameObject);
                    i++;
                }
                FindNextTarget();
            }
        }
        else { return; }
    }

    private void LockOnTarget()
    {
        //Pivot Rotor Plate
        Vector3 rootDir = currentTarget.transform.position - rootSwivel.position;
        Quaternion lookRotation = Quaternion.LookRotation(rootDir);
        Vector3 rootRotation = Quaternion.Lerp(rootSwivel.rotation, lookRotation, Time.deltaTime * basePivotSpeed).eulerAngles;
        rootSwivel.rotation = Quaternion.Euler(rootSwivel.rotation.x, rootRotation.y, rootSwivel.rotation.z);

        //Elevate Turret Arm
        Vector3 eleDir = currentTarget.transform.position - elevatorArmSwivel.position;        
        lookRotation = Quaternion.LookRotation(eleDir);
        Vector3 eleRotation = Quaternion.Lerp(elevatorArmSwivel.rotation, lookRotation, Time.deltaTime * elePivotSpeed).eulerAngles;
        //elevatorArmSwivel.rotation = Quaternion.Euler(elevatorArmSwivel.rotation.x, eleRotation.y, elevatorArmSwivel.rotation.z);
        elevatorArmSwivel.rotation = Quaternion.Euler(eleRotation.x, eleRotation.y, eleRotation.z);

        //Aim Turret Head
        Vector3 headDir = currentTarget.transform.position - turretHeadSwivel.position;
        lookRotation = Quaternion.LookRotation(headDir);
        Vector3 headRotation = Quaternion.Lerp(turretHeadSwivel.rotation, lookRotation, Time.deltaTime * headPivotSpeed).eulerAngles;
        //turretHeadSwivel.rotation = Quaternion.Euler(turretHeadSwivel.rotation.x, headRotation.y, turretHeadSwivel.rotation.z);
        //turretHeadSwivel.rotation = Quaternion.Euler(headRotation.x, turretHeadSwivel.rotation.y, turretHeadSwivel.rotation.z);
        turretHeadSwivel.rotation = Quaternion.Euler(headRotation.x, headRotation.y, headRotation.z);


    }

    private void OnTriggerEnter(Collider other)
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
        float shortestDistance = range;
        GameObject nearestTarget = null;
        // Clean nulls out of the list
        //potentialTargets.RemoveAll(item => item == null);
        potentialTargets.RemoveAll(item => item.activeInHierarchy == false);        
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
        else { potentialTargets.Clear(); }
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
        //float aimHeightOffset = 0.5f;
        RaycastHit hit;
        Vector3 dir = targ.transform.position - barrelTip.position;
        //dir.y -= aimHeightOffset;
        Ray ray = new Ray(barrelTip.position, dir);

#if UNITY_EDITOR
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawTurretGizmos)
        {
            Debug.DrawRay(ray.origin, ray.direction * 5000, Color.yellow);
        }
#endif

        if(Physics.Raycast(ray, out hit, range, enemyMask))
        { 
            sightedTarget = hit.transform;
            if(hit.transform == targ.transform)
            {
                hasClearShot = true;
            }            
        }

        return hasClearShot;
    }
    void Shoot()
    {
        //GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);
        //Bullet bullet = bulletGO.GetComponent<Bullet>();
        //bulletGO.transform.SetParent(transform);
        PoolBullet bullet = bulletPrefab.GetPooledInstance<PoolBullet>();
        
        if (bullet != null)
        {
            //Enemy enemyToShoot = currentTarget.GetComponent<Enemy>();
            float randomVelocityModifier = Random.Range(0.9f, 1.0f);
            bullet.m_BaseDamage = baseDamage;
            m_Firing.Play(m_turretAudio);
            //bullet.AcquireTarget(enemyToShoot);
    
            bullet.transform.position = barrelTip.transform.position;
            bullet.transform.rotation = barrelTip.transform.rotation;

            bullet.FireBullet();
            //bullet.GetComponent<Rigidbody>().AddForce(fireVelocity * barrelTip.forward);
            bullet.GetComponent<Rigidbody>().velocity = (fireVelocity * barrelTip.forward) * randomVelocityModifier;
        }
            
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (GameManager.instance.alwaysDrawGizmos & GameManager.instance.drawTurretGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
            if (currentTarget != null)
            {
                Gizmos.DrawWireSphere(currentTarget.transform.position, 2f);
                
                Gizmos.DrawRay(rootSwivel.position, rootSwivel.forward);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(turretHeadSwivel.position, currentTarget.transform.position);


            }
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(turretHeadSwivel.position, turretHeadSwivel.forward);
        }
    }
#endif
}
