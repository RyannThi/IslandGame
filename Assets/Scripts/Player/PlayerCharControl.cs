using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharControl : MonoBehaviour
{
    private Animator animator;
    private ControlKeys ck; // usado pra verificação de input
    private Rigidbody rb;
    public Material material; // para debug, pra pintar a capsula
    private bool isGrounded; // se o player estiver no chão, true


    public float characterSpeed; //{ get; private set; } // velocidade padrão do player

    private float jumpForce = 1f; // força do pulo do player,

    public State currentState;

    public enum State
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack
    }

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
            currentState = State.Attack;
        }

        if (ck.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            animator.SetTrigger("JUMP");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            currentState = State.Jump;
        }
    }

    void FixedUpdate()
    {
        #region Debug
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
        #endregion

        // Verificação se o player está colidindo com o chão

        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f;
        Vector3 rayDirection = Vector3.down;
        float rayDistance = 1f;
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

        // Captura valores para serem utilizados nos calculos de movimentação

        float z = ck.Player.ForwardBack.ReadValue<float>();
        float x = ck.Player.LeftRight.ReadValue<float>();

        var camForward = Camera.main.transform.forward;
        var camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = z * camForward;
        Vector3 rightRelative = x * camRight;

        Vector3 moveDir = forwardRelative + rightRelative;

        // Verifica se o player quer correr e seta a velocidade de movimento

        if (ck.Player.Run.IsPressed())
        {
            characterSpeed = 8;
        } else
        {
            characterSpeed = 6;
        }

        // Caso o player não esteja atacando, executar a movimentação se o player se mover

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharAttack"))
        {
            if (ck.Player.ForwardBack.IsPressed() || ck.Player.LeftRight.IsPressed())
            {
                animator.SetBool("WALK", true);
                Vector3 movement = new Vector3(moveDir.x, 0, moveDir.z).normalized * characterSpeed;
                var rbVel = rb.velocity;
                rbVel.x = movement.x;
                rbVel.z = movement.z;
                rb.velocity = rbVel;
                currentState = State.Walk;
                // Verifica se o player quer correr
                if (ck.Player.Run.IsPressed())
                {
                    animator.SetBool("RUN", true);
                    currentState = State.Run;
                    //characterSpeed *= 1.5f;
                }
                else
                {
                    animator.SetBool("RUN", false);
                    //characterSpeed /= 1.5f;
                }

                if (!isGrounded)
                {
                    currentState = State.Jump;
                }
            }
            else
            {
                animator.SetBool("WALK", false);
                currentState = State.Idle;
            }

            if (!isGrounded)
            {
                currentState = State.Jump;
            }
        }
        else {
            currentState = State.Attack;
        }
    }

    public void ChangeCharacterSpeed(float speedToChange)
    {
        float startSpeed = characterSpeed;
        characterSpeed = speedToChange;


    }
}
