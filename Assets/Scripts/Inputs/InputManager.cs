using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private UiPhone uiPhone;

    public static InputManager Instance => instance;
    public PlayerInput input;

    private static InputManager instance;
    private PlayerTouchMovement playerTouchMovement;

    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = FindObjectOfType<InputManager>();
        }
        else
        {

            Destroy(gameObject);
        }

        playerTouchMovement = GetComponent<PlayerTouchMovement>();

    #if UNITY_ANDROID || UNITY_IOS
        input = new InputPhone(uiPhone, playerTouchMovement);
        playerTouchMovement.enabled = true;
    #else
        input = new InputKeyboard();
        playerTouchMovement.enabled = false;
    #endif
    }

}
