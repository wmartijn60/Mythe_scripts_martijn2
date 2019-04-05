using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
    [Header("Init")]
    public GameObject activeModel;

    [Header("Inputs")]
    public float horizontal;

    public float vertical;
    public float moveAmount;
    public Vector3 moveDir;

    [Header("Stats")]
    private float moveSpeed = 4;

    private float runSpeed = 8f;
    private float rotateSpeed = 5;
    private float toGround = 0.5f;

    [SerializeField]
    private BoxCollider Sword;

    public AudioSource walkSound;
    public AudioSource runSound;
    public AudioSource slash;
    private bool isWalking = true;

    [Header("States")]
    public bool run;

    public bool walk;

    private bool Attack;
    private bool onGround;
    private bool lockOn;

    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public Rigidbody rb;

    [HideInInspector]
    public float delta;

    [HideInInspector]
    public LayerMask ignoreLayers;

    public void Start()
    {
        Sword.enabled = false;
    }

    //rigid body setup
    public void Init()
    {
        SetupAnimator();
        rb = GetComponent<Rigidbody>();
        rb.angularDrag = 999;
        rb.drag = 4;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        gameObject.layer = 8;
        ignoreLayers = ~(1 << 10);
    }

    //Animator
    private void SetupAnimator()
    {
        if (activeModel == null)
        {
            anim = activeModel.GetComponent<Animator>();
            if (anim == null)
            {
                Debug.Log("no model found");
            }
            else
            {
                activeModel = anim.gameObject;
            }
        }

        anim = GetComponentInChildren<Animator>();
    }

    public void FixedTick(float d)
    {
        delta = d;

        rb.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

        float targetSpeed = moveSpeed;
        if (run)
            targetSpeed = runSpeed;

        if (onGround)
            rb.velocity = moveDir * (targetSpeed * moveAmount);

        if (run)
            lockOn = false;

        //Lock on mode
        if (!lockOn)
        {
            Vector3 targetDir = moveDir;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }

        HandleMovementAnimations();
    }

    public void Tick(float d)
    {
        delta = d;
        onGround = OnGround();

        anim.SetBool("onGround", onGround);
    }

    [SerializeField]
    private float attackSpeed = 1f;

    private float nextAttack = 1f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextAttack)
        {
            StartCoroutine("PlaySound");
            Attack = true;
            Sword.enabled = true;
            nextAttack = Time.time + attackSpeed;
        }
        else
        {
            Attack = false;
            Sword.enabled = false;
        }

        bool shift = Input.GetKeyDown(KeyCode.LeftShift);
        bool keys = Input.GetKeyDown("w") || Input.GetKeyDown("s") || Input.GetKeyDown("a") || Input.GetKeyDown("d");

        if (keys)
        {
            {
                walkSound.Play();
                walkSound.loop = true;
            }
        }

        if (Input.GetKeyUp("w") || Input.GetKeyUp("s") || Input.GetKeyUp("a") || Input.GetKeyUp("d") || shift)
        {
            isWalking = false;
            walkSound.Stop();
        }

        if (shift && Input.GetKey("w") || shift && Input.GetKey("s") || shift && Input.GetKey("a") || shift && Input.GetKey("d"))
        {
            isWalking = false;
            Debug.Log("run");
            runSound.Play();
            runSound.loop = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            walkSound.Play();
            walkSound.loop = true;
            runSound.Stop();
            isWalking = true;
        }
    }

    private IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(0.4f);
        slash.Play();
    }

    //Run and walk animation
    private void HandleMovementAnimations()
    {
        anim.SetBool("walk", walk);
        anim.SetBool("run", run);
        anim.SetFloat("vertical", moveAmount, 0.03f, delta);
        anim.SetBool("Attack", Attack);
    }

    //Grounded
    public bool OnGround()
    {
        bool r = false;

        Vector3 origin = transform.position + (Vector3.up * toGround);
        Vector3 dir = -Vector3.up;
        float dis = toGround + 0.3f;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
        {
            r = true;
            Vector3 targetPosition = hit.point;
            transform.position = targetPosition;
        }

        return r;
    }
}