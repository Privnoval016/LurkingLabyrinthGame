using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * The FirstPersonController class implements first-person camera and movement. It also includes, jumping, crouching, and head bobbing.
 * @author Pranav Sukesh
 * @author Comp-3 Interactive
 * @version 7/31/2023
 **/
public class FirstPersonController : MonoBehaviour
{
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    public bool ShouldJump => Input.GetKey(jumpKey) && characterController.isGrounded;
    public bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchingAnimation && characterController.isGrounded;

    [Header("Player Statistics")]
    [SerializeField] public int xCoor;
    [SerializeField] public int yCoor;
    [SerializeField] private GameObject monsterBody;
    
    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool useFootsteps = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode flashKey = KeyCode.F;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 100)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private float jumpForce = 8.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    public bool isCrouching;
    private bool duringCrouchingAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.11f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] footstepClips = default;
    GameManager gameManager;
    Monster monsterScript;

    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : IsSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;


    [Header("Flash Parameters")]
    public GameObject flashScreen;
    public GameObject flashObject;
    System.Random gen = new System.Random();
    private bool flashed;
    private float currentAlpha = 0f;
    private float footstepTimer = 0;

    [Header("Jumpscare Parameters")]
    [SerializeField] private GameObject jumpscareImage;
    [SerializeField] private GameObject jumpscareBackground;
    //private Animator jumpscareAnimator;
    [SerializeField] private float xShiftRange;
    [SerializeField] private float yShiftRange;
    [SerializeField] private float shiftDuration;
    [SerializeField] private float zoomDuration;
    [SerializeField] private float zoomZPos = 0.25f;
    [SerializeField] private AudioSource jumpScareSource = default;
    [SerializeField] private AudioClip jumpscareNoise = default;
    private bool jumpscareBegun;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    private Vector3 spawnPoint;

    public enum PlayerState { CanMove, NextLevel, Death};
    public PlayerState playerState;

    MazeRenderer m_renderer;

    /*
     * Called upon initialization; retrieves the necessary components and locks cursor.
     */
    void Start()
    {
        gameManager = GameObject.Find("Maze").GetComponent<GameManager>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        monsterScript = GameObject.Find("Monster").GetComponentInChildren<Monster>();
        spawnPoint = m_renderer.getCellPosition(new Vector2Int(gen.Next(15), gen.Next(15)));
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        //jumpscareAnimator = jumpscareImage.GetComponent<Animator>();
        //jumpscareAnimator.SetBool("startJumpscare", false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        jumpscareImage.SetActive(false);
        jumpscareBackground.SetActive(false);
        playerState = PlayerState.CanMove;

        gameObject.transform.position = spawnPoint;
    }


    /*
     * The update loop. It handles movement every frame.
     */
    void Update()
    {
        switch (playerState)
        {
            case PlayerState.CanMove:
                HandleMovementInput();
                HandleMouseLook();
                if (canJump)
                {
                    HandleJump();
                }
                if (canCrouch)
                {
                    HandleCrouch();
                }
                if (canUseHeadBob)
                {
                    HandleHeadBob();
                }

                if (useFootsteps)
                {
                    HandleFootsteps();
                }
                HandleFlash();
                ApplyFinalMovements();
                break;

            case PlayerState.NextLevel:
                Restart();
                break;

            case PlayerState.Death:
                if (!jumpscareBegun)
                    BeginJumpScare();
                break;
        }
    }

    /*
     * Detects user input and sets the movement vector based on key inputs and speeds based on current actions.
     */
    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), 
            (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    /*
     * Detects mouse movement and rotates the camera accordingly to match vision. Also clamps movement to a certain view range.
     */
    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    /*
     * Applies jump force to the user if they are grounded and the right key was pressed.
     */
    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    /*
     * Handles crouching when the user presses the right key.
     */
    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    /*
     * Moves the Y-position of the camera depending on the speed of the user  to simulate a headbob while moving.
     */
    private void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z); 
        }
    }

    private void HandleFootsteps()
    {
        if (!characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            footstepAudioSource.pitch = Random.Range(0.9f, 1.1f);
            if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
            }

            footstepTimer = GetCurrentOffset;
        }

        
    }

    /*
     * Applies gravity, resets jumping, and moves the character controller.
     */
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (characterController.velocity.y < -0.01f && characterController.isGrounded)
        {
            moveDirection.y = 0;
            //footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);

        }
        characterController.Move(moveDirection * Time.deltaTime);
    }
    /*
     * The coroutine applies the crouching animation when the conditions are met.
     */
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchingAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed == timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = currentCenter;

        isCrouching = !isCrouching;

        duringCrouchingAnimation = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            xCoor = other.GetComponent<MazeCellObject>().xCoor;
            yCoor = other.GetComponent<MazeCellObject>().yCoor;
        }
    }

    private void Restart()
    {
        characterController.enabled = false;
        spawnPoint = m_renderer.getCellPosition(new Vector2Int(gen.Next(15), gen.Next(15)));
        transform.position = spawnPoint;
        characterController.enabled = true;
        playerState = PlayerState.CanMove;
    }

    private void HandleFlash()
    {
        flashScreen.GetComponent<Image>().color = new Color(1f, 1f, 1f, currentAlpha);
        if (Input.GetKeyDown(flashKey) && !flashed && gameManager.currentCharges > 0)
        {
            currentAlpha = 1f;
            Debug.Log("stun");
            HandleStun();
            gameManager.currentCharges--;
            flashed = true;
        }
        if (flashed)
        {
            currentAlpha -= 0.25f * Time.deltaTime; //(0.2f/255);
            if (currentAlpha <= 0)
            {
                Debug.Log("Unstun");
                monsterScript.UnStun();
                flashed = false;
            }
        }
    }
    private void HandleStun()
    {
        GameObject flash = GameObject.Instantiate(flashObject, transform.position, Quaternion.identity);
        Rigidbody rb = flash.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 0.5f);
    }

    private void BeginJumpScare()
    {
        jumpscareBegun = true;
        characterController.enabled = false;
        //transform.LookAt(monsterBody.transform);
        jumpscareImage.SetActive(true);
        jumpscareBackground.SetActive(true);
        //jumpscareAnimator.SetBool("startJumpscare", true);

        //jumpscareImage.transform.localScale = new Vector3(1, 1, 1);

        jumpScareSource.PlayOneShot(jumpscareNoise);
        
        StartCoroutine(JumpScareAnimation());

    }

    public IEnumerator JumpScareAnimation()
    {
        /*
        float elapsedTime = 0;
        Vector3 end = new Vector3(7, 7, 7);
        Vector3 startingScale = jumpscareImage.transform.localScale;
        while (elapsedTime < 0.1f)
        {
            jumpscareImage.transform.localScale = Vector3.Lerp(startingScale, end, (elapsedTime / 0.1f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        jumpscareImage.transform.position = end;
        */

        yield return new WaitForSeconds(0.2f);
        float elapsedTime = 0;
        Vector3 end = new Vector3(36, 36, 36);
        Vector3 startingScale = jumpscareImage.transform.localScale;
        while (elapsedTime < 0.2f)
        {
            jumpscareImage.transform.localScale = Vector3.Lerp(startingScale, end, (elapsedTime / 0.2f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        jumpscareImage.transform.position = end;

        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
