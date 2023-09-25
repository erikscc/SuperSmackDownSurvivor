using UnityEngine;
using System;

public class PlayerTPController : MonoBehaviour
{
    [Header("Player Camera Settings")]
    [Space(10)]
    public bool mouseRotationEnabled;
    public float rotationSpeedKey;

    [Header("Player Settings")]
    [Space(10)]

    public float WalkSpeed = 4;
    public float ClampMovespeed = 3;
    public float movespeedMultipler;
    public float rotationSpeed;
    [Space(5)]
    public bool lockRotationY;
    public float clampRotationYDegrees;
    [Space(5)]
    public float JumpForce = 10;
    public float Gravity = 20;
    public float FallSpeed = 1;
    public float health = 100;

    [Header("Player Components")]
    [Space(10)]

    public LayerMask shootLayerMask;
    public CharacterController characterController;
    private Rigidbody rb;
    private Transform m_transform;
    private Transform spine;
    public Transform weaponTransform;
    public Animator animator;


    private float vSpeed = 0;
    private Vector3 tempVelocity;
    private Vector3 currentForce;
    public Vector2 HV;
    public Vector2 Rotation;
    public bool IsDead;
    public bool m_PreviouslyGrounded, m_IsGrounded, m_Jumping;
    public Vector3 move;
    private float tmpHorizontal = 0;
    private float tmpVertical = 0;
    public static Action<GameObject> OnPickupItem;
  
    public Vector3 Velocity
    {
        get { return _velocity; }
    }
    private Vector3 _velocity;

    private void Start()
    {

        m_transform = transform;

        if (!rb && GetComponent<Rigidbody>())
            rb = GetComponent<Rigidbody>();

        if (!animator && GetComponent<Animator>())
            animator = GetComponent<Animator>();

        if (!characterController && GetComponent<CharacterController>())
            characterController = GetComponent<CharacterController>();

        //if (!spine)
        //    spine = animator.GetBoneTransform(HumanBodyBones.Spine);
    }
    void Update()
    {

        if (IsDead)
            return;

        GetInputs();
        ApplyMovement();
        ApplyRotate(Rotation.x, Rotation.y);
        ForceApplier();
    }


    private void FixedUpdate()
    {

    }
    void GetInputs()
    {
        HV.x = Input.GetAxis("Horizontal");
        HV.y = Input.GetAxis("Vertical");

        if (mouseRotationEnabled)
        {
            Rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
            Rotation.y += Input.GetAxis("Mouse Y") * rotationSpeed;
        }
        else
        {
            Rotation.x += Input.GetAxis("Horizontal") * rotationSpeedKey;
        }

        if (lockRotationY)
        {
            Rotation.y = -clampRotationYDegrees;
        }
        else
        {
            Rotation.y = Mathf.Clamp(Rotation.y, -clampRotationYDegrees, clampRotationYDegrees);
        }
    }

    void ApplyMovement()
    {
        if (!characterController.enabled)
            return;

        m_IsGrounded = characterController.isGrounded;

        movespeedMultipler = Mathf.MoveTowards(movespeedMultipler, WalkSpeed, Time.deltaTime * 10);

        if (m_IsGrounded)
        {
            move = (HV.x * m_transform.right + HV.y * m_transform.forward).normalized * movespeedMultipler;

            vSpeed = 0;
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetTrigger("Jump");
                vSpeed = JumpForce;
                m_Jumping = true;
                animator.SetBool("Grounded", true);
            }
            tempVelocity = characterController.velocity;
        }
        else
        {
            move = tempVelocity + (HV.x * m_transform.right + HV.y * m_transform.forward).normalized * FallSpeed;
            move.x = Mathf.Clamp(move.x, -WalkSpeed, WalkSpeed);
            move.z = Mathf.Clamp(move.z, -WalkSpeed, WalkSpeed);
        }

        tmpVertical = Mathf.MoveTowards(tmpVertical, movespeedMultipler / WalkSpeed * HV.y, Time.deltaTime * 10);
        tmpHorizontal = Mathf.MoveTowards(tmpHorizontal, movespeedMultipler / WalkSpeed * HV.x, Time.deltaTime * 10);
        animator.SetFloat("Run", HV.magnitude);
        move = Vector3.ClampMagnitude(move, ClampMovespeed);
        vSpeed -= Gravity * Time.deltaTime;
        move.y = vSpeed;

        if (!m_PreviouslyGrounded && m_IsGrounded)
        {
            m_Jumping = false;
            animator.SetBool("Grounded", false);
        }

        m_PreviouslyGrounded = m_IsGrounded;

        if (m_IsGrounded)
        {
            if (characterController.velocity != Vector3.zero && (HV.x == 0f || HV.y == 0f))
            {
                _velocity = Vector3.zero;
            }
        }
        Debug.DrawRay(transform.position + Vector3.up, Camera.main.transform.forward);
    }

    void ApplyRotate(float rotateX, float rotateY)
    {
        m_transform.rotation = Quaternion.RotateTowards(m_transform.rotation, Quaternion.Euler(0, rotateX, 0f), Time.deltaTime * 720);
        Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, Quaternion.Euler(-rotateY, rotateX, 0f), Time.deltaTime * 720);
    }

    void ForceApplier()
    {
        currentForce = Vector3.ClampMagnitude(currentForce, 2);
        characterController.Move(currentForce + move * Time.deltaTime);
        _velocity = characterController.velocity;
    }
   

}
