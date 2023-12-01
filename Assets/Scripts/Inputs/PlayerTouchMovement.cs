using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlayerTouchMovement : MonoBehaviour
{
    [SerializeField] private FloatingJoystick movementJoystick;

    [HideInInspector] public Vector2 lookAmount;
    [HideInInspector] public Vector2 startLookPosition;
    [HideInInspector] public bool isTouching;

    public Vector2 MovementAmount => movementAmount;

    private Finger lookFinger;
    private Finger movementFinger;
    private Vector2 joystickSize;
    private Vector2 movementAmount;


    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleLoseFinger;
        ETouch.Touch.onFingerMove += HandleFingerMove;
    }

    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleLoseFinger;
        ETouch.Touch.onFingerMove -= HandleFingerMove;

        EnhancedTouchSupport.Disable();
    }

    private void Awake()
    {
        GlobalEvents.OnMovementTouchOverButton += OffTouchMovement;
        GlobalEvents.OnLookTouchOverButton += OffTouchLook;
    }

    private void HandleFingerMove(Finger movedFinger)
    {
        if (movedFinger == movementFinger)
        {
            Vector2 knobPosition;
            ETouch.Touch currentTouch = movedFinger.currentTouch;
            Vector3 currentTouchPosition = currentTouch.screenPosition;
            float maxMovement = joystickSize.x / 2f;
            

            if (Vector3.Distance(currentTouchPosition, movementJoystick.transform.position) > maxMovement)
            {
                knobPosition = (currentTouchPosition - movementJoystick.transform.position).normalized * maxMovement;
            }
            else
            {
                knobPosition = currentTouchPosition - movementJoystick.transform.position;
            }

            movementJoystick.knob.localPosition = knobPosition;
            movementAmount = knobPosition / maxMovement;
        }

        if (movedFinger == lookFinger)
        {
            float treshold = 10f;
            float senceModify = 0.05f;

            if (Vector3.Distance(movedFinger.currentTouch.screenPosition, startLookPosition) > treshold)
            {
                lookAmount = (movedFinger.currentTouch.screenPosition - startLookPosition).normalized;
            }
            else
            {
                lookAmount = (movedFinger.currentTouch.screenPosition - startLookPosition) * senceModify;
            }
            startLookPosition = movedFinger.currentTouch.screenPosition;

        }
    }

    private void HandleLoseFinger(Finger lostFinger)
    {
        if (lostFinger == movementFinger)
        {
            OffTouchMovement();
        }
        if (lostFinger == lookFinger)
        {
            lookFinger = null;
            lookAmount = Vector2.zero;
            isTouching = false;
        }
    }

    private void HandleFingerDown(Finger touchedFinger)
    {
        if (movementFinger == null && touchedFinger.screenPosition.x <= Screen.width / 2f)
        {
            movementFinger = touchedFinger;
            movementAmount = Vector2.zero;

            movementJoystick.gameObject.SetActive(true);

            joystickSize = new Vector2(Screen.height / 2f, Screen.height / 2f);
            movementJoystick.rectTransform.sizeDelta = joystickSize;
            movementJoystick.rectTransform.position = touchedFinger.screenPosition;
        }

        if (lookFinger == null && touchedFinger.screenPosition.x >= Screen.width / 2f)
        {
            lookFinger = touchedFinger;
            lookAmount = Vector2.zero;
            startLookPosition = touchedFinger.screenPosition;
            isTouching = true;
        }
    }

    private void OffTouchMovement()
    {
        movementFinger = null;

        movementJoystick.knob.localPosition = Vector2.zero;
        movementJoystick.gameObject.SetActive(false);
        movementAmount = Vector2.zero;
    }

    private void OffTouchLook()
    {
        lookFinger = null;
        lookAmount = Vector2.zero;
        isTouching = false;
    }
}