using UnityEngine;

public class InputPhone : PlayerInput
{
    private UiPhone uiPhone;
    private PlayerTouchMovement playerTouchMovement;

    public InputPhone (UiPhone uiPhone, PlayerTouchMovement playerTouchMovement)
    {
        this.uiPhone = uiPhone;
        this.playerTouchMovement = playerTouchMovement;
        uiPhone.gameObject.SetActive(true);
    }

    public override Vector2 GetMovementImput() => playerTouchMovement.MovementAmount;
    
    public override Quaternion[] GetLookInput()
    {
        rotationX -= playerTouchMovement.lookAmount.y * sensitivityY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);

        rotationY += playerTouchMovement.lookAmount.x * sensitivityX;

        Quaternion playerCameraLocalRotation = Quaternion.Euler(rotationX, 0, 0);
        Quaternion transformRotation = Quaternion.Euler(0, rotationY, 0);
        
        playerTouchMovement.lookAmount = Vector2.zero;

        return new Quaternion[] { playerCameraLocalRotation, transformRotation };
    }

    public override bool JumpPressed() => uiPhone.Jump.isPressed;
    
    public override bool CrouchPressed() => uiPhone.Crouch.IsClicked();

    public override bool ReloadPressed() => uiPhone.Reload.isPressed;

    public override bool ShootPressed() => uiPhone.Shoot.IsClicked();

    public override bool ShootAutoHolding() => uiPhone.Shoot.isPressed;

    public override bool SettingsPressed() => uiPhone.Settings.IsClicked();

    public override bool SprintPressed() =>  uiPhone.Sprint.isPressed;
    
    public override bool WeaponChangePressed(SOWeapon SOWeapon)
    {
        for (int i = 0; i < uiPhone.weapons.Length; i++)
        {
            if (SOWeapon.WeaponIndex == i && uiPhone.weapons[i].isPressed)
            {
                return true;
            }
        }
        return false;
    }
}
