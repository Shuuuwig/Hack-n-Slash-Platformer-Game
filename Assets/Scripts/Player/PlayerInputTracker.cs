using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputTracker : MonoBehaviour
{
    protected List<string> inputSequence = new List<string>();
    [SerializeField] protected Timer inputStayTime;
    [SerializeField] protected Timer multipleInputPrevention;

    protected bool inputLeft;
    protected bool inputRight;
    protected bool inputUp;
    protected bool inputDown;

    protected bool quarterCircleForwardMotion;
    protected bool quarterCircleBackwardMotion;
    protected bool downDownMotion;
    protected bool halfCircleForwardMotion;
    protected bool halfCircleBackwardMotion;
    protected bool dragonPunchMotion;

    protected Vector2 playerDirectionalInput;

    [SerializeField] protected KeyCode cameraLock;
    [SerializeField] protected KeyCode lightButton;
    [SerializeField] protected KeyCode mediumButton;
    [SerializeField] protected KeyCode heavyButton;
    [SerializeField] protected KeyCode parryButton;
    [SerializeField] protected KeyCode dashButton;

    public bool InputLeft { get { return inputLeft; } }
    public bool InputRight { get { return inputRight; } }
    public bool InputUp { get { return inputUp; } }
    public bool InputDown { get { return inputDown; } }
    public bool QuarterCircleForwardMotion { get { return quarterCircleForwardMotion; } }
    public bool QuarterCircleBackwardMotion { get { return quarterCircleBackwardMotion; } }
    public bool DownDownMotion { get { return downDownMotion; } }
    public bool HalfCircleForwardMotion { get { return halfCircleForwardMotion; } }
    public bool HalfCircleBackwardMotion { get { return halfCircleBackwardMotion; } }
    public bool DragonPunchMotion { get { return dragonPunchMotion; } }

    public Vector2 PlayerDirectionalInput { get { return playerDirectionalInput; } }

    public KeyCode CameraLock { get { return cameraLock; } }
    public KeyCode LightButton { get { return lightButton; } }
    public KeyCode MediumButton {  get { return mediumButton; } }
    public KeyCode HeavyButton {  get { return heavyButton; } }
    public KeyCode ParryButton { get { return parryButton; } }
    public KeyCode DashButton { get { return dashButton; } }


    protected PlayerAnimationHandler animationHandler;

    private void Start()
    {
        animationHandler = GetComponent<PlayerAnimationHandler>();
    }

    void Update()
    {
        HandleInput();
        RemoveInput();
        RegisteredMotions();

        //Debug.Log("Current Input Sequence: " + string.Join(", ", inputSequence));
    }

    protected void HandleInput()
    {
        playerDirectionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        inputLeft = playerDirectionalInput.x < 0;
        inputRight = playerDirectionalInput.x > 0;
        inputUp = playerDirectionalInput.y > 0;
        inputDown = playerDirectionalInput.y < 0;

        // Numpad notation
        if (inputLeft && !inputDown && !inputUp) 
            RegisterInput("4");
        if (inputRight && !inputDown && !inputUp)
            RegisterInput("6");
        if (inputDown && !inputLeft && !inputRight)
            RegisterInput("2");
        if (inputUp && !inputLeft && !inputRight)
            RegisterInput("8");
        if (inputRight && inputDown)
            RegisterInput("3");
        if (inputLeft && inputDown)
            RegisterInput("1");
        if (inputRight && inputUp)
            RegisterInput("9");
        if (inputLeft && inputUp)
            RegisterInput("7");

        if (Input.GetKeyDown(lightButton))
            RegisterInput("light");
        if (Input.GetKeyDown(mediumButton))
            RegisterInput("medium");
        if (Input.GetKeyDown(heavyButton))
            RegisterInput("heavy");
        if (Input.GetKeyDown(parryButton))
            RegisterInput("parry");
        if (Input.GetKeyDown(dashButton))
            RegisterInput("dash");
    }

    private void RegisterInput(string input)
    {
        
        if (inputSequence.Count == 0 || inputSequence[inputSequence.Count - 1] != input)
        {
            //Debug.Log(input);
            inputSequence.Add(input);
        }

    }

    private void RemoveInput()
    {
        if (inputSequence.Count > 5)
        {
            inputSequence.RemoveAt(0);
        }
        if (inputSequence.Count > 0 && inputStayTime.CurrentProgress == Timer.Progress.Finished)
        {
            inputSequence.RemoveAt(0);
            inputStayTime.ResetCooldown();
            inputStayTime.StartCooldown();
            //Debug.Log("Removed");

        }
        if (Input.anyKeyDown && inputStayTime.CurrentProgress == Timer.Progress.Ready)
        {
            inputStayTime.StartCooldown();
        }
    }

    private void RegisteredMotions()
    {
        if (animationHandler.FacingRight)
        {
            quarterCircleForwardMotion = string.Join(" ", inputSequence).Contains("2 3 6");
            quarterCircleBackwardMotion = string.Join(" ", inputSequence).Contains("2 1 4");
            downDownMotion = string.Join(" ", inputSequence).Contains("2 2");
            halfCircleForwardMotion = string.Join(" ", inputSequence).Contains("4 1 2 3 6");
            halfCircleBackwardMotion = string.Join(" ", inputSequence).Contains("6 3 2 1 4");
            dragonPunchMotion = string.Join(" ", inputSequence).Contains("6 2 3");
        }
        else
        {
            quarterCircleForwardMotion = string.Join(" ", inputSequence).Contains("2 1 4");
            quarterCircleBackwardMotion = string.Join(" ", inputSequence).Contains("2 3 6");
            downDownMotion = string.Join(" ", inputSequence).Contains("2 2");
            halfCircleForwardMotion = string.Join(" ", inputSequence).Contains("6 3 2 1 4");
            halfCircleBackwardMotion = string.Join(" ", inputSequence).Contains("4 1 2 3 6");
            dragonPunchMotion = string.Join(" ", inputSequence).Contains("4 2 1");
        }
        

        if (quarterCircleForwardMotion)
        {
            Debug.Log("Quarter Circle Forward");
        }

        if (quarterCircleBackwardMotion)
        {
            Debug.Log("Quarter Circle Backward");

        }

        if (downDownMotion) // Does not work
        {
            Debug.Log("Down-Down Motion");

        }

        if (halfCircleForwardMotion)
        {
            Debug.Log("Half-Circle Forward");

        }

        if (halfCircleBackwardMotion)
        {
            Debug.Log("Half-Circle Backward");

        }

        if (dragonPunchMotion)
        {
            Debug.Log("Dragon Punch");

        }
    }
}
