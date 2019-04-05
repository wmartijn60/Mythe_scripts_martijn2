using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public bool lockon;
    private float followSpeed = 9;
    private float mouseSpeed = 3;
    private float controllerSpeed = 7;

    public Transform target;

    public GameObject MainCamera;
    public GameObject SecondCamera;

    [HideInInspector]
    public Transform pivot;

    [HideInInspector]
    public Transform camTrans;

    public bool bossfight = false;

    private float turnSmoothing = .1f;
    private float minAngle = 0;
    private float maxAngle = 2;

    private float smoothX;
    private float smoothY;
    private float smoothXvelocity;
    private float smoothYvelocity;
    private float lookAngle;
    private float tiltAngle;

    private void Start()
    {
        MainCamera.SetActive(true);
        SecondCamera.SetActive(false);
    }

    private void Update()
    {
        if (!bossfight)
        {
            SecondCamera.SetActive(true);
        }
        else
        {
            MainCamera.SetActive(true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            transform.LookAt(target);
        }
    }

    public void Init(Transform t)
    {
        target = t;

        camTrans = Camera.main.transform;
        pivot = camTrans.parent;
    }

    public void Tick(float d)
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        float c_h = Input.GetAxis("RightAxis X");
        float c_v = Input.GetAxis("RightAxis Y");

        float targetSpeed = mouseSpeed;

        if (c_h != 0 || c_v != 0)
        {
            h = c_h;
            v = c_v;

            targetSpeed = controllerSpeed;
        }
        FollowTarget(d);
        HandleRotations(d, v, h, targetSpeed);
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        MainCamera.SetActive(false);
        SecondCamera.SetActive(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        MainCamera.SetActive(true);
        SecondCamera.SetActive(false);
    }
    */

    public void FollowTarget(float d)
    {
        float speed = d * followSpeed;
        Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
        transform.position = targetPosition;
    }

    private void HandleRotations(float d, float v, float h, float targetSpeed)
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (turnSmoothing > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXvelocity, turnSmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYvelocity, turnSmoothing);
        }
        else
        {
            smoothX = h;
            smoothY = v;
        }
        if (lockon)
        {
        }

        lookAngle += smoothX * targetSpeed;
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);

        tiltAngle -= smoothY * targetSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

    public static CameraManager singleton;

    private void Awake()
    {
        singleton = this;
    }
}