using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretArmatureDebugger : MonoBehaviour {
    #region Input
    // Gutted this directly from RTS Camera
    public bool useScreenEdgeInput = true;
    public float screenEdgeBorder = 25f;

    public bool useKeyboardInput = true;
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";

    public bool usePanning = true;
    public KeyCode panningKey = KeyCode.Mouse2;

    public bool useKeyboardZooming = true;
    public KeyCode zoomInKey = KeyCode.E;
    public KeyCode zoomOutKey = KeyCode.Q;

    public bool useScrollwheelZooming = true;
    public string zoomingAxis = "Mouse ScrollWheel";

    public bool useKeyboardRotation = true;
    public KeyCode rotateRightKey = KeyCode.X;
    public KeyCode rotateLeftKey = KeyCode.Z;

    public bool useMouseRotation = true;
    public KeyCode mouseRotationKey = KeyCode.Mouse1;

    private Vector2 KeyboardInput
    {
        get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
    }

    private Vector2 MouseInput
    {
        get { return Input.mousePosition; }
    }

    private float ScrollWheel
    {
        get { return Input.GetAxis(zoomingAxis); }
    }

    private Vector2 MouseAxis
    {
        get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
    }

    private int ZoomDirection
    {
        get
        {
            bool zoomIn = Input.GetKey(zoomInKey);
            bool zoomOut = Input.GetKey(zoomOutKey);
            if (zoomIn && zoomOut)
                return 0;
            else if (!zoomIn && zoomOut)
                return 1;
            else if (zoomIn && !zoomOut)
                return -1;
            else
                return 0;
        }
    }

    private int RotationDirection
    {
        get
        {
            bool rotateRight = Input.GetKey(rotateRightKey);
            bool rotateLeft = Input.GetKey(rotateLeftKey);
            if (rotateLeft && rotateRight)
                return 0;
            else if (rotateLeft && !rotateRight)
                return -1;
            else if (!rotateLeft && rotateRight)
                return 1;
            else
                return 0;
        }
    }

    #endregion
    [Header("Debug Output")]
    [SerializeField] Text debugOut;

    [Header("Turret Parts")]
    [SerializeField] Transform rootPivotBone;
    [SerializeField] Transform elevationBone;
    [SerializeField] Transform turretHeadBone;
    public Vector3 rootPBoneRotation;
    public Vector3 elevationBoneRotation;
    public Vector3 turretHeadBoneRotation;
    public enum PartToRotate
    {
        rootPivotBone,
        elevationBone,
        turretHeadBone
    };
    public PartToRotate partToRotate;
    // Use this for initialization
    void Start () {
        rootPBoneRotation = rootPivotBone.eulerAngles;
        elevationBoneRotation = elevationBone.eulerAngles;
        turretHeadBoneRotation = turretHeadBone.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {

        GetInput();
        UpdateTextUI();
        UpdateQuaternionOut();
	}

    void GetInput()
    {

    }

    void UpdateTextUI()
    {
        debugOut.text = rootPBoneRotation.ToString();
        debugOut.text = elevationBoneRotation.ToString();
        debugOut.text = turretHeadBoneRotation.ToString();

    }
    void UpdateQuaternionOut()
    {
        rootPBoneRotation = rootPivotBone.eulerAngles;
        elevationBoneRotation = elevationBone.eulerAngles;
        turretHeadBoneRotation = turretHeadBone.eulerAngles;
    }
}
