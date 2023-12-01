using UnityEngine;
using UnityEngine.EventSystems;

public class UiButtonLogic : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private bool isLeftButton;

    [HideInInspector] public bool isPressed;

    private bool isClicked;


    private void Update()
    {
        GlobalEvents.OnGameStarted += SetClickedFalse;
    }

    public bool IsClicked()
    {
        if (isClicked)
        {
            isClicked = false;
            return true;
        }

        return false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        isPressed = true;
        if (isLeftButton)
        {
            GlobalEvents.OnMovementTouchOverButton?.Invoke();
        }
        else
        {
            GlobalEvents.OnLookTouchOverButton?.Invoke();
        }
        
    }

    public void OnPointerClick(PointerEventData eventData) => isClicked = true;

    public void OnPointerUp(PointerEventData data) => isPressed = false;

    private void SetClickedFalse() => isClicked = false;
}
