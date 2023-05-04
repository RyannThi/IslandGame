using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharControl : MonoBehaviour
{
    private Animator animator;
    private ControlKeys ck;
    private Rigidbody rb;
    public Material material;
    private bool isGrounded;
    private float characterSpeed;
    private float jumpForce = 5f;

    private void Awake()
    {
        ck = new ControlKeys();
    }
    private void OnEnable()
    {
        ck.Enable();
    }
    private void OnDisable()
    {
        ck.Disable();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (ck.Player.Attack.WasPressedThisFrame())
        {
            animator.SetTrigger("ATTACK");
        }

        if (ck.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            animator.SetTrigger("JUMP");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharIdle"))
        {
            material.color = Color.green;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharWalk"))
        {
            material.color = Color.blue;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharRun"))
        {
            material.color = Color.yellow;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharJump"))
        {
            material.color = Color.red;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharAttack"))
        {
            material.color = Color.cyan;
        }

        // Define o ponto de origem do raio logo abaixo do objeto
        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f;

        // Define a direção do raio para baixo
        Vector3 rayDirection = Vector3.down;

        // Define o comprimento máximo do raio
        float rayDistance = 1f;

        // Dispara um raio para baixo e verifica se colide com algum objeto
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance))
        {
            // Se o objeto colidir com algum objeto com a tag "Ground", então está no chão
            if (hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                animator.SetBool("isGrounded", true);
            }
        }
        else
        {
            animator.SetBool("isGrounded", false);
            isGrounded = false;
        }

        float z = ck.Player.ForwardBack.ReadValue<float>();
        float x = ck.Player.LeftRight.ReadValue<float>();
        
        if (ck.Player.Run.IsPressed())
        {
            characterSpeed = 8;
        } else
        {
            characterSpeed = 6;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharAttack"))
        {
            if (ck.Player.ForwardBack.IsPressed() || ck.Player.LeftRight.IsPressed())
            {
                animator.SetBool("WALK", true);
                Vector3 movement = new Vector3(x, 0, z).normalized * characterSpeed;
                rb.velocity = movement;
            }
            else
            {
                animator.SetBool("WALK", false);
            }

            if (ck.Player.Run.IsPressed())
            {
                animator.SetBool("RUN", true);
            }
            else
            {
                animator.SetBool("RUN", false);
            }
        }
    }
}
