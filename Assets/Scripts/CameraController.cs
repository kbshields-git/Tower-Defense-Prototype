using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Text debugOut;
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;

    private Transform m_Transform;

    [Header("Control")]
    public LayerMask groundMask = -1; //layermask of ground or other objects that affect height
    public float scrollSpeed = 0.5f;

    [Header("Bounding")]
    public float minY = 3f;
    public float maxY = 80f;
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -12f;
    public float maxZ = 4f;
    public float hoverDistance = 3f;
    public Vector3 angleAtMinY;
    public Vector3 angleAtMidY;
    public Vector3 angleAtMaxY;
    public bool mousePan = false;

    private void Start()
    {
        m_Transform = transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (mousePan)
        {
            MouseEnabledPan();
        }
        else NoMousePan();

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        Vector3 pos = m_Transform.position;

        pos.y -= scroll * 1000 * scrollSpeed * Time.deltaTime;
        if (Physics.Raycast(m_Transform.position, Vector3.down, hoverDistance))
        {
            RaycastHit hitInfo;
            Physics.Raycast(m_Transform.position, Vector3.down, out hitInfo, hoverDistance);
            float correction = hoverDistance - hitInfo.distance;
            //Debug.Log(correction);
            //Debug.Log(target.position);
            pos.y += correction;
            //Debug.Log(target.position);

        }
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        

        debugOut.text = m_Transform.position.ToString();
        m_Transform.position = pos;
        float lerpmount = maxY / pos.y;
        //Debug.Log(lerpmount);

        //Find the middle of the y range with a lerp of 50%
        float midY = Mathf.Lerp(minY, maxY, 0.5f);
        if (pos.y > midY)
        {
            m_Transform.rotation = Quaternion.Lerp(Quaternion.Euler(angleAtMidY), Quaternion.Euler(angleAtMaxY), pos.y / maxY);
        }
        else if (pos.y <= midY)
        {
            m_Transform.rotation = Quaternion.Lerp(Quaternion.Euler(angleAtMinY), Quaternion.Euler(angleAtMidY), pos.y / midY);
        }
    }

    private void MouseEnabledPan()
    {
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            m_Transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            m_Transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            m_Transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            m_Transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
    }

    private void NoMousePan()
    {
        if (Input.GetKey("w"))
        {
            m_Transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("s"))
        {
            m_Transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("d"))
        {
            m_Transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("a"))
        {
            m_Transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
        }
    }

    private float DistanceToGround()
    {
        Ray ray = new Ray(m_Transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, groundMask.value))
            return (hit.point - m_Transform.position).magnitude;

        return 0f;
    }
}