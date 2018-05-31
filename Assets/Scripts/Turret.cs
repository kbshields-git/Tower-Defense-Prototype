using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class Turret : MonoBehaviour {

    /// <summary>
    /// Target currently focused by this turret
    /// </summary>
    public GameObject currentTarget;

    /// <summary>
    /// List<T> of all currently eligible (inside Trigger sphere) targets
    /// </summary>
    [SerializeField] protected List<GameObject> potentialTargets = new List<GameObject>();
        
    /// <summary>
    /// Range of the turret
    /// </summary>
    public float range = 15f;

    public float pivotSpeed = 10f;

    /// <summary>
    /// Collider to detect enemies in range
    /// </summary>
    public SphereCollider rangeTrigger;

    [SerializeField] Transform pivotPart;

	// Use this for initialization
	void Start () {
        rangeTrigger.radius = range;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (currentTarget == null)
        {
            IdleRotation();
            return;
        }

        // Rotate turret in direction of current target
        LockOnTarget();
    }

    private void IdleRotation()
    {
        // For now, I'm just letting the turret rotate back to a straight forward position to keep it simple
        // <TODO>: Make turret swivel around slowly when not actively engaging a target
        Vector3 rotation = Quaternion.Lerp(pivotPart.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * pivotSpeed).eulerAngles;
        pivotPart.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    private void LockOnTarget()
    {
        Vector3 dir = currentTarget.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(pivotPart.rotation, lookRotation, Time.deltaTime * pivotSpeed).eulerAngles;
        pivotPart.rotation = Quaternion.Euler(0f, rotation.y, 0f);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        if (currentTarget != null)
        {
            Gizmos.DrawWireSphere(currentTarget.transform.position, 2f);
        }
    }
}
