using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CnControls;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField]
    private bool isSecondPlayer;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float jumpPower;

    [SerializeField]
    private float gravityMultiplier;

    private float checkGroundDistance = 0.5f;

    [SerializeField]
    private string groundTag = "Ground";

    [SerializeField]
    private float TouchSensitivity;

    private Touch[] touch;
    private Rigidbody rb;
    private Transform selfTransform;

    private Vector3 position;
    private Vector2 positionDiff;

    private IvyScript ivy;

    [HideInInspector]
    public Vector3 lastGroundedPosition;

    // [HideInInspector]
    public bool onGround, holdingIvy, ivyReleased, jump;

    private bool tap, roll, interactableObjectFound, pullableObjectFound, doubleJumpReady, shake, jumpReady = true, footPlay;

    [HideInInspector]
    public int direction;

    private int prevDirection;
    private float divisionRate, originalHeight, originalSpeed;
    private Vector3 originalCenter;

    private GameObject interactableObject;
    private SomeCameraControl camController;

    private Animator animController;

    private CapsuleCollider capsule;

    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        selfTransform = transform;
        ivy = GetComponentInChildren<IvyScript>();
        capsule = GetComponent<CapsuleCollider>();
        originalCenter = capsule.center;
        originalHeight = capsule.height;
        originalSpeed = speed;
        lastGroundedPosition = selfTransform.position;
        audioSource = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Awake()
    {

        Application.targetFrameRate = 60;

        // camController = Camera.main.GetComponent<CameraController>();
        camController = Camera.main.GetComponent<SomeCameraControl>();
        animController = GetComponent<Animator>();


        if (isSecondPlayer)
            enabled = false;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag(groundTag) || coll.gameObject.CompareTag("Jumpable"))
        {
            onGround = true;
            ivyReleased = false;
            animController.SetTrigger("Crouch");
            shake = true;
            doubleJumpReady = false;
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        interactableObject = coll.gameObject;

        if (interactableObject.CompareTag("InteractableObject"))
            interactableObjectFound = true;
        else if (interactableObject.CompareTag("PullableObject"))
            pullableObjectFound = true;
    }

    void OnEnable()
    {
        holdingIvy = false;
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("InteractableObject"))
        {
            interactableObjectFound = false;
        }

        else if (interactableObject.CompareTag("PullableObject"))
        {
            pullableObjectFound = false;
            Invoke("StopInteract", 0.5f);
        }

    }

    void AddJumpForce(float multiplier)
    {
        // jump ready check for fat boy               
        if (jumpReady)
        {
            rb.AddForce(new Vector3(0, jumpPower * multiplier, 0), ForceMode.Impulse);
            onGround = false;
            jump = false;
            if (!isSecondPlayer)
            {
                SoundController.FatJump();
                jumpReady = false;
                Invoke("SetJumpReady", 1f);
            }
            else
            {
                if (!doubleJumpReady)
                    SoundController.SkinnyJumpOne();
                else
                    SoundController.SkinnyJumpTwo();
            }
        }
    }

    void SetJumpReady()
    {
        jumpReady = true;
    }

    void CheckGroundStatus()
    {

        Vector3 down = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        Ray ray = new Ray(transform.position, down);
        Physics.Raycast(ray, out hit, 0.5f);

        if (Physics.Raycast(ray, out hit, checkGroundDistance) && hit.collider.CompareTag("Ground"))
        {
            lastGroundedPosition = transform.position;
            /* onGround = true;
             ivyReleased = false;
             animController.SetTrigger("Crouch");
             shake = true;
             doubleJumpReady = false;
             */
            if (!isSecondPlayer && shake && rb.velocity.y < -0.1f)
            {
                camController.Shake(0.13f);
                shake = false;
            }
        }
    }

    void Jump()
    {
        if (!animController.GetCurrentAnimatorStateInfo(0).IsName("Idle 2"))
        {
            if (doubleJumpReady && jump && !ivyReleased)
            {
                if (isSecondPlayer)
                {
                    AddJumpForce(0.8f);
                    animController.SetTrigger("DoubleJump");
                }

                doubleJumpReady = false;
                jump = false;
            }

            else if (jump)
            {
                AddJumpForce(1);
                animController.SetTrigger("Jump");
                StartCoroutine("CheckJumpPosition");
            }

            else
            {
                AddExtraGravity();
            }

            //  CheckGroundStatus();
        }
    }

    void AddExtraGravity()
    {
        Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        rb.AddForce(extraGravityForce, ForceMode.Force);
    }

    void Roll()
    {
        //animController.applyRootMotion = true;
        animController.SetTrigger("Roll");
        capsule.height /= 2;
        capsule.center = capsule.center - new Vector3(0, capsule.height / 2, 0);
        Invoke("RollApply", 0.15f);
        Invoke("SetCapsule", animController.GetCurrentAnimatorStateInfo(0).length);

       // SoundController.FatRoll();
    }

    void RollApply()
    {
        rb.velocity = new Vector3(direction * 8, rb.velocity.y, 0);
    }

    void StopInteract()
    {
        speed = originalSpeed;
        if (pullableObjectFound && interactableObject.CompareTag("PullableObject"))
        {
            Rigidbody intRb = interactableObject.GetComponent<Rigidbody>();
            if (intRb.velocity.y > -1)
                intRb.isKinematic = true;
        }

        animController.SetBool("Push", false);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void SetCapsule()
    {
        capsule.height = originalHeight;
        capsule.center = originalCenter;
    }

    void Interact(float deltaTime)
    {

        CancelInvoke("StopInteract");
        if (pullableObjectFound && !isSecondPlayer)
        {
            speed = originalSpeed / 2;
            // interactableObject.GetComponent<Rigidbody>().AddForce(new Vector3(deltaTime * 300, 0, 0), ForceMode.Impulse);
            interactableObject.GetComponent<Rigidbody>().isKinematic = false;
            animController.SetBool("Push", true);
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        Invoke("StopInteract", 0.5f);
    }

    IEnumerator CheckJumpPosition()
    {
        float temp;

        while (true)
        {
            temp = rb.velocity.y;
            yield return new WaitForSeconds(.05f);

            if (Mathf.Abs(rb.velocity.y) <= Mathf.Abs(temp))
            {
                doubleJumpReady = true;
                break;
            }
        }

    }

    void OnDisable()
    {
 
        if (rb != null)
            rb.velocity = Vector3.zero;

        footPlay = false;
        if (audioSource != null)
            audioSource.Stop();

    }

    void FixedUpdate()
    {

        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Idle 2"))
        {
            if (!isSecondPlayer)
                SoundController.FiestaSound();

            else SoundController.Sinav();
        }
        else
        {
            if (!isSecondPlayer)
                SoundController.CancelFiesta();
            else
                SoundController.CancelSinav();
        }

        float x = CnInputManager.GetAxis("Horizontal");

        prevDirection = direction;

        if (x > 0)
        {

            direction = 1;

        }

        else if (x < 0)
        {
            direction = -1;
        }

        else
        {
            if (isSecondPlayer)
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }


        if (!holdingIvy && x != 0)
        {
            position.Set(selfTransform.position.x + x * speed * Time.deltaTime, selfTransform.position.y, 0);
            rb.MovePosition(position);

            rb.velocity = new Vector3(x * Time.deltaTime * speed * 10, rb.velocity.y, 0);

            selfTransform.eulerAngles = new Vector3(0, direction * 90, 0);
        }

        else if (holdingIvy && prevDirection != direction)
        {
            selfTransform.Rotate(0, 180, 0);
        }

        if (animController != null)
        {
            if (onGround)
                animController.SetFloat("Speed", Mathf.Abs(x));
            else
                animController.SetFloat("Speed", 0);
        }


#if (UNITY_STANDALONE || UNITY_EDITOR)
        //#if UNITY_ANDROID

        if (Input.GetButtonDown("Jump") && (onGround || doubleJumpReady || holdingIvy) && !ivyReleased)
        {
            if (isSecondPlayer)
            {
                ivy.Jump(true);
                if (holdingIvy)
                    doubleJumpReady = false;
            }
            jump = true;
        }

        else if (Input.GetButton("Fire1"))
            Interact(Time.deltaTime);


        // yuvarlanma
        else if (Input.GetButtonUp("Fire2") && !isSecondPlayer)
            Roll();


        // itme
        // else if (Input.GetButtonUp("Fire1"))
        //  StopInteract();

        // touch part
#else
        touch = Input.touches;

        Debug.Log("asdsad-and");

        for (int i = 0; i < touch.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(touch[i].position, touch[i].position);

            if (!hit || !hit.collider.CompareTag("Joystick"))
            {
                switch (touch[i].phase)
                {
                    case TouchPhase.Began:
                        tap = true;
                        break;

                    case TouchPhase.Moved:

                        positionDiff = touch[i].deltaPosition;
                        divisionRate = Mathf.Abs(positionDiff.y / positionDiff.x);

                        if ((onGround || doubleJumpReady || holdingIvy) && divisionRate > 0.4 && positionDiff.y > TouchSensitivity && !ivyReleased)
                        {
                            tap = false;
                            roll = false;

                            if (isSecondPlayer)
                            {
                                ivy.Jump(true);
                                if (holdingIvy)
                                    doubleJumpReady = false;
                            }

                            jump = true;
                        }

                        else if (divisionRate > 0.4 && positionDiff.y < -TouchSensitivity)
                        {
                            tap = false;
                            roll = true;
                        }

                        break;

                    case TouchPhase.Stationary:
                        if (tap)
                            Interact(x * Time.deltaTime);
                        break;

                    case TouchPhase.Ended:
                        if (tap)
                            Interact(x * Time.deltaTime);
                        else if (roll)
                            if (!isSecondPlayer)
                                Roll();
                        break;
                }
            }
        }
#endif


        if (Mathf.Abs(x) >= 0.5f && !footPlay && onGround)
        {
            if (isSecondPlayer)
                audioSource.clip = SoundController.instance.clips[7];
            else
                audioSource.clip = SoundController.instance.clips[3];
            footPlay = true;
            audioSource.Play();
        }

        else if ((Mathf.Abs(x) < 0.5f || !onGround) && footPlay && !jump)
        {
            footPlay = false;
            audioSource.Stop();
        }


        if (isSecondPlayer)
            ivy.AddVelocityTransfer(x * Time.deltaTime * 2);

        // if (!onGround)
        CheckGroundStatus();

        Jump();
    }
}

