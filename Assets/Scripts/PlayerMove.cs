using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [Header("Movement")]
    public float speedRun;
    public float speedWalkSlide;
    public float speedWalkBack;
    float finalSpeed;
    public float jumpForce;
    public float gravityValue;
    public float gravityMultiplier;
    public float rotationSpeed;
    public float rotationSpeedPlayer;
    public Vector2 _look;
    public Transform followTransform;
    public float rollForce;
    public float rollDuration;
    public Vector3 rollDirection;

    public GameObject mago;

    float horizontalInput;
    float verticalInput;
    CharacterController cc;
    Vector3 playerVelocity;

    Animator anim;
    bool isJumping;
    bool isJumpingWalk;
    bool canJump;
    bool isFalling;
    bool isGrounded;
    bool isRolling;
    bool canRoll;
    bool finishRoll = true;
    public Transform cameraTransform;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float groundRaycastLength;
    [SerializeField] float groundRaycastLengthRight;
    [SerializeField] float groundRaycastLengthUp;
    [SerializeField] Vector3 groundRaycastOffset;
    [SerializeField] Vector3 groundRaycastOffsetUp;
    [SerializeField] Vector3 groundRaycastOffsetRight;

    bool haRotado;

    private void OnEnable()
    {
        InputManager.playerControls.Player.Roll.performed += GetRollInput;
        InputManager.playerControls.Player.Roll.canceled += GetRollInput;

        InputManager.playerControls.Player.Jump.performed += GetJumpInput;
        InputManager.playerControls.Player.Jump.canceled += GetJumpInput;

      
    }

    private void OnDisable()
    {
        InputManager.playerControls.Player.Roll.performed -= GetRollInput;
        InputManager.playerControls.Player.Roll.canceled -= GetRollInput;

        InputManager.playerControls.Player.Jump.performed -= GetJumpInput;
        InputManager.playerControls.Player.Jump.canceled -= GetJumpInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        //cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetMoveInput();
        SetAnimation();
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        MoveCharacter();

        if (!isGrounded)
        {
            Fall();
        }
        else
        {
            isFalling = false;
        }
    }

    #region Inputs

    private void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isRolling)
        {

            if (verticalInput == 0 || horizontalInput == 0)
            {

                if (verticalInput > 0 && horizontalInput == 0)
                {
                    isJumpingWalk = false;
                }
                else
                {
                    isJumpingWalk = true;
                }

                
                
            }else
            {
                isJumpingWalk = false;
            }
          
            
            canJump = true;
        }
    }



    private void GetRollInput(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canRoll && !isJumping && finishRoll)
        {
            RollAnimation();
            finishRoll = false;
            isRolling = true;

        }
    }

    public void FinishedRoll() { finishRoll = true; isRolling = false; }

    void RollAnimation()
    {
        if (verticalInput == 0)
        {
            if (horizontalInput > 0)
            {
                //roll para derecha
                rollDirection = transform.right;
            }
            else if (horizontalInput < 0)
            {
                //roll para izquierda
                rollDirection = -transform.right;
            }
        }
        else if (horizontalInput == 0)
        {

            if (verticalInput < 0)
            {
                //roll para atras
                rollDirection = -transform.forward;
            }
        }

        anim.SetTrigger("activeRoll");

    }

    public void PlayRoll()
    {
        StartCoroutine(RollTime());
    }

    IEnumerator RollTime()
    {
        

        

        Vector3 originalPosition = transform.position;
        Vector3 direction = rollDirection.normalized * rollForce;

        

        float startTime = Time.time;
        float endTime = startTime + rollDuration;

        // Mientras estemos dentro del tiempo del roll
        while (Time.time < endTime)
        {
            // Calcular la posición interpolando entre la posición original y la posición del roll
            float normalizedTime = (Time.time - startTime) / rollDuration;
            Vector3 newPosition = Vector3.Lerp(originalPosition, originalPosition + direction, normalizedTime);

           
            // Mover el CharacterController
            cc.Move(newPosition - transform.position);


            yield return null;
        }


        

        
    }

    void GetMoveInput()
    {
        Vector3 movement = InputManager.playerControls.Player.Move.ReadValue<Vector2>();


        horizontalInput = movement.x * finalSpeed;
        verticalInput = movement.y * finalSpeed;

        #region MoveOneDirection

        if (verticalInput == 0)
        {
            canRoll = true;
            anim.SetBool("isRunningLeftDown", false);
            anim.SetBool("isRunningLeftUp", false);
            anim.SetBool("isRunningRightDown", false);
            anim.SetBool("isRunningRightUp", false);
            anim.SetBool("isRunningForward", false);
            anim.SetBool("isRunningBack", false);

            if (horizontalInput > 0)
            {
                finalSpeed = speedWalkSlide;
                anim.SetBool("isWalkingRight", true);
            }
            else if (horizontalInput < 0)
            {
                finalSpeed = speedWalkSlide;
                anim.SetBool("isWalkingLeft", true);

            }

            if (!isGrounded)
            {
                if (isJumpingWalk)
                {
                    finalSpeed = speedWalkSlide;
                }
                else
                {
                    finalSpeed = speedRun;
                }
            }
        }

        if (movement == Vector3.zero)
        {
            anim.SetBool("isRunningLeftDown", false);
            anim.SetBool("isRunningLeftUp", false);
            anim.SetBool("isRunningRightDown", false);
            anim.SetBool("isRunningRightUp", false);
            anim.SetBool("isRunningForward", false);
            anim.SetBool("isRunningBack", false);
            canRoll = false;
        }

        if (horizontalInput == 0)
        {

            anim.SetBool("isWalkingRight", false);
            anim.SetBool("isWalkingLeft", false);

            if (verticalInput > 0)
            {
                canRoll = false;
                finalSpeed = speedRun;
                anim.SetBool("isRunningRightUp", false);
                anim.SetBool("isRunningLeftUp", false);
                anim.SetBool("isRunningForward", true);
            }
            else if (verticalInput < 0)
            {
                canRoll = true;
                finalSpeed = speedWalkBack;
                anim.SetBool("isRunningBack", true);
                anim.SetBool("isRunningRightDown", false);
                anim.SetBool("isRunningLeftDown", false);
            }

            if (!isGrounded)
            {
                if (isJumpingWalk)
                {
                    finalSpeed = speedWalkSlide;
                }
                else
                {
                    finalSpeed = speedRun;
                }
            }

        }

        #endregion

        #region MoveTwoDirection

        if (verticalInput > 0)
        {

            anim.SetBool("isRunningRightUp", false);
            anim.SetBool("isWalkingRight", false);
            anim.SetBool("isRunningLeftUp", false);
            anim.SetBool("isWalkingLeft", false);

            if (horizontalInput > 0)
            {
                canRoll = false;
                finalSpeed = speedRun;
                anim.SetBool("isRunningRightUp", true);

            }
            else if (horizontalInput < 0)
            {
                canRoll = false;
                finalSpeed = speedRun;
                anim.SetBool("isRunningLeftUp", true);

            }
        }
        else if (verticalInput < 0)
        {

            anim.SetBool("isRunningRightUp", false);
            anim.SetBool("isWalkingRight", false);
            anim.SetBool("isRunningLeftUp", false);
            anim.SetBool("isWalkingLeft", false);

            if (horizontalInput > 0)
            {
                canRoll = false;
                finalSpeed = speedWalkBack;
                anim.SetBool("isRunningRightDown", true);
            }
            else if (horizontalInput < 0)
            {
                canRoll = false;
                finalSpeed = speedWalkBack;
                anim.SetBool("isRunningLeftDown", true);
            }
        }

        #endregion


    }

    #endregion

    #region MoveJumpFall

    private void MoveRotation()
    {
        _look = InputManager.playerControls.Player.Mouse.ReadValue<Vector2>();


        followTransform.transform.rotation *= Quaternion.AngleAxis(_look.x * rotationSpeed * Time.deltaTime, Vector3.up);

        followTransform.transform.rotation *= Quaternion.AngleAxis(-_look.y * rotationSpeed * Time.deltaTime, Vector3.right);

        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTransform.transform.localEulerAngles = angles;

        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
    }

    private void MoveCharacter()
    {


        if (playerVelocity.y < 0 && isGrounded)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);

        move = move.x * cameraTransform.right + cameraTransform.forward * move.z;
        move.y = 0f;

        if (!isRolling)
        {
            cc.Move(move * Time.deltaTime * finalSpeed);
        }
        

        MoveRotation();

        Jump();
        

        
    }

    void Jump()
    {
        if (canJump)
        {
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue * gravityMultiplier);
            canJump = false;
            isJumping = true;

        }

        playerVelocity.y += gravityMultiplier * gravityValue * Time.deltaTime;
        cc.Move(playerVelocity * Time.deltaTime);
    }

    void Fall()
    {
        if (cc.velocity.y < 0) // Si estamos cayendo del salto, añadimos multiplicador de gravedad
        {
            isFalling = true;
            isJumping = false;

        }
    }

    #endregion


    public void MovePosition(Vector3 position)
    {
        cc.enabled = false;
        transform.position = position;
        cc.enabled = true;
    }

    void SetAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isFalling", isFalling);
   
    }

    private void CheckCollisions()
    {
        //Ground Collisions
        isGrounded = Physics.Raycast(transform.position + groundRaycastOffset, Vector3.down, groundRaycastLength, groundLayer) ||
                     Physics.Raycast(transform.position - groundRaycastOffset, Vector3.down, groundRaycastLength, groundLayer) ||
                     Physics.Raycast(transform.position - groundRaycastOffset, Vector3.down, groundRaycastLength, collisionLayer)
        || Physics.Raycast(transform.position + groundRaycastOffsetUp, Vector3.down, groundRaycastLengthUp, groundLayer)
        || Physics.Raycast(transform.position - groundRaycastOffsetUp, Vector3.down, groundRaycastLengthUp, groundLayer)
        || Physics.Raycast(transform.position + groundRaycastOffsetRight, Vector3.down, groundRaycastLengthRight, groundLayer)
        || Physics.Raycast(transform.position - groundRaycastOffsetRight, Vector3.down, groundRaycastLengthRight, groundLayer)
        || Physics.Raycast(transform.position + groundRaycastOffsetUp, Vector3.down, groundRaycastLengthUp, collisionLayer)
        || Physics.Raycast(transform.position - groundRaycastOffsetUp, Vector3.down, groundRaycastLengthUp, collisionLayer)
        || Physics.Raycast(transform.position + groundRaycastOffsetRight, Vector3.down, groundRaycastLengthRight, collisionLayer)
        || Physics.Raycast(transform.position - groundRaycastOffsetRight, Vector3.down, groundRaycastLengthRight, collisionLayer);


        //Wall Collisions

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        //Ground Check
        Gizmos.DrawLine(transform.position + groundRaycastOffset, transform.position + groundRaycastOffset + Vector3.down * groundRaycastLength);
        Gizmos.DrawLine(transform.position - groundRaycastOffset, transform.position - groundRaycastOffset + Vector3.down * groundRaycastLength);
        Gizmos.DrawLine(transform.position - groundRaycastOffsetUp, transform.position - groundRaycastOffsetUp + Vector3.down * groundRaycastLengthUp);
        Gizmos.DrawLine(transform.position + groundRaycastOffsetUp, transform.position + groundRaycastOffsetUp + Vector3.down * groundRaycastLengthUp);
        Gizmos.DrawLine(transform.position - groundRaycastOffsetRight, transform.position - groundRaycastOffsetRight + Vector3.down * groundRaycastLengthRight);
        Gizmos.DrawLine(transform.position + groundRaycastOffsetRight, transform.position + groundRaycastOffsetRight + Vector3.down * groundRaycastLengthRight);




    }
}
