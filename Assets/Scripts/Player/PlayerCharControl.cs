using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharControl : MonoBehaviour
{
    public Transform orientation;
    public Transform camera;

    private Animator animator;
    private ControlKeys ck; // usado pra verificação de input
    private Rigidbody rb;
    public Material material; // para debug, pra pintar a capsula
    private bool isGrounded; // se o player estiver no chão, true
    private float rotationSpeed = 5f;

    public int characterHealth = 100;

    public float characterSpeed; //{ get; private set; } // velocidade padrão do player

    private float characterSpeedModifier = 1f;
    private float characterSpeedModifierTimer = 0f;

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
        if (characterSpeedModifierTimer > 0)
        {
            characterSpeedModifierTimer -= Time.deltaTime; 
            if (characterSpeedModifierTimer <= 0)
            {
                characterSpeedModifier = 1f;
            }
        }

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

        #region Ground check
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
        #endregion

        
        // Captura valores para serem utilizados nos calculos de movimentação

        Vector3 viewDir = transform.position - new Vector3(camera.position.x, transform.position.y, camera.position.z);
        orientation.forward = viewDir.normalized;

        float z = ck.Player.ForwardBack.ReadValue<float>();
        float x = ck.Player.LeftRight.ReadValue<float>();

        // Verifica se o player quer correr e seta a velocidade de movimento

        // Caso o player não esteja atacando, executar a movimentação se o player se mover

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerCharAttack"))
        {
            if (ck.Player.ForwardBack.IsPressed() || ck.Player.LeftRight.IsPressed())
            {
                animator.SetBool("WALK", true);

                // Direção de movimento
                Vector3 inputDir = orientation.forward * z + orientation.right * x;
                if (inputDir != Vector3.zero)
                {
                    transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
                }

                rb.AddForce(inputDir.normalized * (characterSpeed * characterSpeedModifier), ForceMode.Force);

                Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                // Limite de velocidade
                if (flatVel.magnitude > (characterSpeed * characterSpeedModifier))
                {
                    Vector3 limitedVel = flatVel.normalized * (characterSpeed * characterSpeedModifier);
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }

                if (currentState == State.Run)
                {
                    characterSpeed = 8;
                }
                else if (currentState == State.Walk)
                {
                    characterSpeed = 6;
                }

                // Verifica se o player quer correr
                if (ck.Player.Run.IsPressed())
                {
                    animator.SetBool("RUN", true);
                    currentState = State.Run;
                }
                else
                {
                    animator.SetBool("RUN", false);
                    currentState = State.Walk;
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

    public void ChangeCharacterSpeed(float speedModifier, float time = 0f)
    {
        characterSpeedModifier = speedModifier;
        if (time != 0)
        {
            characterSpeedModifierTimer = time;
        }
    }

    public void TakeDamage(int damage)
    {
        characterHealth -= damage;
    }

    public void HealHealth(int healAmount)
    {
        characterHealth += healAmount;
    }
}
