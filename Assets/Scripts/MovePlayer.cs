using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    #region Variables 
    Rigidbody newRigidbody;
    public GameObject Current_Weapon;   
    GameObject Swappable_Weapon;        
    bool swap_weapon;                   

    [Space]
    [SerializeField]
    float rotSpeed = 3f;
    [SerializeField]
    float groundedThreshold = 0.15f;
    bool grounded;
    Transform cameraTransform;
    Animator animator;
    CapsuleCollider capsule;
    Vector3 moveDir;
    Vector3 initialPos;
    public float jumpPower;
    public float minPossibleY = -50f; 
    private Animator m_animator; 
    private Animator n_animator;

    #endregion

    void Start()
    {
        cameraTransform = Camera.main.transform;
        newRigidbody = GetComponent<Rigidbody>();
        initialPos = transform.position;
        animator = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        m_animator = GetComponent<Animator>();
        swap_weapon = false;
        n_animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetMoveDirection();
         
        UpdateAnimatorParameters();

        HandleAttack();

        HandleJump();

        SwordAttack();

        AxeAttack();

        Switch_Weapon();
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
            animator.SetTrigger("Punch");
    }

    private void OnAnimatorMove()
    {
        ApplyRootMotion();

        ApplyRootRotation();
    }

    private void UpdateAnimatorParameters()
    {
        Vector3 characterSpaceMoveDir = transform.InverseTransformDirection(moveDir);
        if (!Input.GetKey(KeyCode.LeftShift))
            characterSpaceMoveDir *= 0.5f;
        animator.SetFloat("Forward", characterSpaceMoveDir.z, 0.15f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceMoveDir.x, 0.15f, Time.deltaTime);
    }

    private void SwordAttack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_animator.SetTrigger("Attack");
        }
    }
    private void AxeAttack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            n_animator.SetTrigger("AxeAttack");
        }
    }

    private void HandleJump()
    {
        if (transform.position.y < minPossibleY)
            transform.position = initialPos; 

        Ray ray = new Ray(); 
        Vector3 centerRayOrigin = transform.position + Vector3.up * groundedThreshold;
        ray.direction = Vector3.down;

        grounded = false; 

        for(float offsetX = -1f; offsetX <= 1f; offsetX += 1)
        {
            for (float offsetZ = -1f; offsetZ <= 1f; offsetZ += 1)
            {
                Vector3 rayOffset = new Vector3(offsetX, 0, offsetZ).normalized;
                ray.origin = centerRayOrigin + rayOffset * capsule.radius;
                if (Physics.Raycast(ray, 2f * groundedThreshold))
                {
                    grounded = true; 
                    //break;
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2 * groundedThreshold, Color.green);
                }
                else
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2 * groundedThreshold, Color.red);
                }
            }
        }
       
        Color rayColor = grounded ? Color.green : Color.red;
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 2 * groundedThreshold, rayColor);
        animator.SetBool("Midair", !grounded);

        if (grounded && Input.GetButtonDown("Jump"))
            newRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
    }

    private void ApplyRootRotation()
    {
        if (moveDir.magnitude < 10e-3f) 
            return;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
        
    }

    private void ApplyRootMotion()
    {
        if (!grounded)
        {
            animator.applyRootMotion = false;
            return;
        }
        animator.applyRootMotion = true;
       
        float velY = GetComponent<Rigidbody>().velocity.y; 
        GetComponent<Rigidbody>().velocity = animator.deltaPosition / Time.deltaTime;
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, velY, GetComponent<Rigidbody>().velocity.z); 
    }

    private void GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDir = cameraTransform.right * h + cameraTransform.forward * v; 
        moveDir = Vector3.ProjectOnPlane(moveDir, Vector3.up).normalized; 
    }

    private void Switch_Weapon()
    {
        if(swap_weapon)
            if(Input.GetKeyDown(KeyCode.E))
            {
               
                GameObject tmp = Current_Weapon;
                Vector3 c_pos = Current_Weapon.transform.position;
                Current_Weapon.transform.position = Swappable_Weapon.transform.position;
                Swappable_Weapon.transform.position = c_pos;

                Quaternion c_rot = Current_Weapon.transform.rotation;
                Current_Weapon.transform.rotation = Swappable_Weapon.transform.rotation;
                Swappable_Weapon.transform.rotation = c_rot;

                Transform Prnt = Current_Weapon.transform.parent;
                Current_Weapon.transform.parent = Swappable_Weapon.transform.parent;
                Swappable_Weapon.transform.parent = Prnt;

                Current_Weapon = Swappable_Weapon;
                Swappable_Weapon = tmp;

                swap_weapon = false;
                Debug.Log("DONE");
            }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon") && Current_Weapon != collision.gameObject)
        {
            swap_weapon = true;
            Swappable_Weapon = collision.gameObject;
            Debug.Log("Swap it");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Weapon") && Current_Weapon != collision.gameObject)
        {
            swap_weapon = false;
            Debug.LogWarning("Don't swap");
        }
    }

}
