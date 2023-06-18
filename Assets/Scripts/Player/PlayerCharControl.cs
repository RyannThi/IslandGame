using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharControl : MonoBehaviour
{
    public static PlayerCharControl instance;

    #region Keys
    private bool gotFireKey;
    private bool gotIceKey;
    #endregion

    public Transform orientation;
    public Transform combatLookAt;
    public Transform camera;

    public CinemachineFreeLook cameraCinemachine;
    //public GameObject cameraCombat;

    public Animator animator;
    private ControlKeys ck; // usado pra verificação de input
    private Rigidbody rb;
    public Material material; // para debug, pra pintar a capsula
    private bool isGrounded; // se o player estiver no chão, true
    private float rotationSpeed = 5f;

    public float characterHealth = 100;

    public float characterSpeed; //{ get; private set; } // velocidade padrão do player

    [SerializeField]
    private float characterSpeedModifier = 1f;

    [SerializeField]
    private float characterSpeedModifierTimer = 0f;

    private float jumpForce = 1f; // força do pulo do player,

    public State currentState;
    public bool combatMode = false;

    private CinemachineComposer cmRig0;
    private CinemachineComposer cmRig1;
    private CinemachineComposer cmRig2;

    [Header("SFX")]
    [SerializeField]
    private AudioSource walkSfx;

    private bool sfxOnce = false;
    public enum State
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack
    }

    public GameObject gameOverDebug;

    private void Awake() { DontDestroyOnLoad(gameObject); ck = new ControlKeys(); }
    private void OnEnable() { ck.Enable(); }
    private void OnDisable() { ck.Disable(); }

    void Start()
    {
        instance = this;

        //animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        ck.Player.Aim.performed += Aim_performed;
        ck.Player.Aim.canceled += Aim_canceled;
        ck.Player.Noclip.started += Noclip_started;

        cmRig0 = cameraCinemachine.GetRig(0).GetCinemachineComponent<CinemachineComposer>();
        cmRig1 = cameraCinemachine.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        cmRig2 = cameraCinemachine.GetRig(2).GetCinemachineComponent<CinemachineComposer>();
    }

    private void Noclip_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
        Collider col = GetComponent<Collider>();
        if (col.enabled)
        {
            col.enabled = false;
            rb.useGravity= false;
            characterSpeedModifier = 4;
        }
        else
        {
            col.enabled = true;
            rb.useGravity= true;
            characterSpeedModifier = 1;
        }
            
    }

    #region Input Methods

    private void Aim_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        combatMode = !combatMode;
        //cameraNormal.SetActive(!cameraNormal.activeSelf);
        //cameraCombat.SetActive(!cameraCombat.activeSelf);
    }
    private void Aim_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        combatMode = !combatMode;
        //cameraNormal.SetActive(!cameraNormal.activeSelf);
        //cameraCombat.SetActive(!cameraCombat.activeSelf);
    }

    #endregion
    private void Update()
    {
        if (combatMode)
        {
            cmRig0.m_ScreenX = Mathf.Lerp(cmRig0.m_ScreenX, 0.35f, 4f * Time.deltaTime);
            cmRig1.m_ScreenX = Mathf.Lerp(cmRig0.m_ScreenX, 0.35f, 4f * Time.deltaTime);
            cmRig2.m_ScreenX = Mathf.Lerp(cmRig0.m_ScreenX, 0.35f, 4f * Time.deltaTime);
        } else
        {
            cmRig0.m_ScreenX = Mathf.Lerp(cmRig0.m_ScreenX, 0.5f, 4f * Time.deltaTime);
            cmRig1.m_ScreenX = Mathf.Lerp(cmRig0.m_ScreenX, 0.5f, 4f * Time.deltaTime);
            cmRig2.m_ScreenX = Mathf.Lerp(cmRig0.m_ScreenX, 0.5f, 4f * Time.deltaTime);
        }

        if (characterSpeedModifierTimer > 0)
        {
            characterSpeedModifierTimer -= Time.deltaTime; 
            if (characterSpeedModifierTimer <= 0)
            {
                characterSpeedModifier = 1f;
            }
        }
        if (resistanceModifierTimer> 0)
        {
            resistanceModifierTimer -= Time.deltaTime;
            if (resistanceModifierTimer <= 0)
            {
                Resistance(1) ;
            }
        }

        /*if (ck.Player.Attack.WasPressedThisFrame())
        {
            animator.SetTrigger("ATTACK");
            currentState = State.Attack;
        }*/

        if (ck.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            animator.SetTrigger("JUMP");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            currentState = State.Jump;
        }

        /*if (ck.Player.Jump.WasPressedThisFrame() && isGrounded)
        {
            Instantiate(gameOverDebug);
        }*/

        /*if (ck.Player.Combat.WasPressedThisFrame())
        {
            combatMode = !combatMode;
            cameraNormal.SetActive(!cameraNormal.activeSelf);
            cameraCombat.SetActive(!cameraCombat.activeSelf);
        }*/

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
        /*if (ck.Player.Confirm.WasPressedThisFrame())
        {
            characterHealth -= 20;
            TakeDamage(20);
        }*/
        #endregion

        #region Ground check
        // Verificação se o player está colidindo com o chão
        Vector3 rayOrigin = transform.position + Vector3.down * 0.5f;
        Vector3 rayDirection = Vector3.down;
        float rayDistance = 1.5f;
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
                if (!sfxOnce)
                {
                    Debug.Log("Audio");
                    walkSfx.Play();
                    sfxOnce= true;
                }
                    
                // Direção de movimento

                if (combatMode == false)
                {
                    Vector3 inputDir = orientation.forward * z + orientation.right * x;
                    if (inputDir != Vector3.zero)
                    {
                        transform.forward = inputDir.normalized;
                    }
                    rb.AddForce(inputDir.normalized * (characterSpeed * characterSpeedModifier), ForceMode.Force);
                } 
                else
                {
                    Vector3 inputDir = orientation.forward * z + orientation.right * x;

                    Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(camera.position.x, combatLookAt.position.y, camera.position.z);
                    orientation.forward = viewDir.normalized;
                    //combatLookAt.forward = dirToCombatLookAt.normalized;

                    transform.forward = inputDir.normalized;

                    
                    rb.AddForce(inputDir.normalized * (characterSpeed * characterSpeedModifier), ForceMode.Force);
                }
                

                

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
                walkSfx.Stop();
                sfxOnce = false;
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
    public void Respawn()
    {
        SaveInfo save = SaveInfo.instance;
        
        transform.position = save.lastSavePosition;
        characterHealth = save.lastSaveHealth;
        gotFireKey = save.hasFireKey;
        gotIceKey = save.hasIceKey;
        PlayerStats.instance.UpdateHealthGauge(Mathf.Abs(characterHealth));
    }

    #region Change Player Methods

    public void ChangeCharacterSpeed(float speedModifier, float time = 0f)
    {
        characterSpeedModifier = speedModifier;
        if (time != 0)
        {
            characterSpeedModifierTimer = time;
        }
    }

    public float GetHealth()
    {
        return characterHealth;
    }
    private float resistanceModifierTimer;
    public void Resistance(float resistanceModifier, float time = 0f)
    {       
            resistance = resistanceModifier;       
            
        if(time != 0)
        {
            resistanceModifierTimer = time;
        }
    }
    private float resistance = 1;
    public void TakeDamage(float damage)
    {
        
        characterHealth -= damage / resistance;
        PlayerStats.instance.UpdateHealthGauge(Mathf.Abs(damage) * -1);
        if(characterHealth <= 0)
        {
            Instantiate(gameOverDebug);
        }

        Debug.Log(damage);
    }

    public void HealHealth(float healAmount)
    {
        characterHealth += healAmount;
        PlayerStats.instance.UpdateHealthGauge(Mathf.Abs(healAmount));
    }

    #region Keys
    public void SetFireKey(bool key)
    {
        gotFireKey = key;
    }

    public void SetIceKey(bool key)
    {
        gotIceKey = key;
    }

    public bool GetFireKey()
    {
        return gotFireKey;
    }
    public bool GetIceKey()
    {
        return gotIceKey;
    }
    #endregion


    #endregion
}
