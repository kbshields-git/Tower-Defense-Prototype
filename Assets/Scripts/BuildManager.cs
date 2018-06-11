using UnityEngine;

public class BuildManager : MonoBehaviour {
    public static BuildManager instance;
    //Starting with 4 possible turrets. This will become much more fleshed out and UI driven eventually.
    [SerializeField] GameObject turretSlot1;
    [SerializeField] KeyCode turret1HK = KeyCode.Alpha1;
    [SerializeField] GameObject turretSlot2;
    [SerializeField] KeyCode turret2HK = KeyCode.Alpha2;
    [SerializeField] GameObject turretSlot3;
    [SerializeField] KeyCode turret3HK = KeyCode.Alpha3;
    [SerializeField] GameObject turretSlot4;
    [SerializeField] KeyCode turret4HK = KeyCode.Alpha4;

    [SerializeField] KeyCode rotateHK = KeyCode.R;

    GameObject selectedBuild;
    float mouseWheelRotation;

    // Simple singleton instantiation check
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }               
    }
    private void Update()
    {
        HandleNewObjectHotkey();

        if (selectedBuild != null)
        {
            MoveCurrentObjectToMouse();
            Rotate();
            ReleaseIfClicked();
        }
    }

    private void HandleNewObjectHotkey()
    {
        if (Input.GetKeyDown(turret1HK))
        {
            if (selectedBuild != null)
            {
                Destroy(selectedBuild);
            }
            else
            {
                selectedBuild = Instantiate(turretSlot1);
            }
        }
    }

    private void MoveCurrentObjectToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            selectedBuild.transform.position = hitInfo.point;
            //selectedBuild.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
    }

    private void Rotate()
    {
        if (Input.GetKeyDown(rotateHK))
        {
            selectedBuild.transform.Rotate(Vector3.up, 90f);
        }
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedBuild.GetComponent<Turret>().Build();
            selectedBuild = null;
        }
    }
}

