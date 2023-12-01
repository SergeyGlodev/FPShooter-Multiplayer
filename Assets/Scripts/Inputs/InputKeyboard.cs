using UnityEngine;
public class InputKeyboard : PlayerInput
{
    private KeyCode forwardKey = KeyCode.W;
    private KeyCode backwardKey = KeyCode.S;
    private KeyCode leftKey = KeyCode.A;
    private KeyCode rightKey = KeyCode.D;
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode crouchKey = KeyCode.C;
    private KeyCode sprintKey = KeyCode.LeftShift;
    private KeyCode reloadKey = KeyCode.R;
    private KeyCode settingsKey = KeyCode.Escape;

    public override Vector2 GetMovementImput()
    {
        float v = 0;
        float h = 0;

        if (Input.GetKey(forwardKey))
        {
            v = 1f;
        }
        else if (Input.GetKey(backwardKey))
        {
            v = -1f;
        }

        if (Input.GetKey(leftKey))
        {
            h = -1f;
        }
        else if (Input.GetKey(rightKey))
        {
            h = 1f;
        }
        
        return new Vector2(h, v);
    }

    public override Quaternion[] GetLookInput()
    {
        rotationX -= Input.GetAxis("Mouse Y") * sensitivityY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);

        rotationY += Input.GetAxis("Mouse X") * sensitivityX;

        Quaternion playerCameraLocalRotation = Quaternion.Euler(rotationX, 0, 0);
        Quaternion transformRotation = Quaternion.Euler(0, rotationY, 0);

        return new Quaternion[] { playerCameraLocalRotation, transformRotation };
    }

    public override bool SprintPressed() => Input.GetKey(sprintKey);
    
    public override bool JumpPressed() => Input.GetKey(jumpKey);
    
    public override bool CrouchPressed() => Input.GetKey(crouchKey);

    public override bool ShootPressed() => Input.GetMouseButtonDown(0);
    
    public override bool ShootAutoHolding() => Input.GetMouseButton(0);
    
    public override bool ReloadPressed() => Input.GetKeyDown(reloadKey);
    
    public override bool SettingsPressed() => Input.GetKeyDown(settingsKey);
    
    public override bool WeaponChangePressed(SOWeapon SOWeapon) => Input.GetKeyDown(SOWeapon.KeyCode);
}

