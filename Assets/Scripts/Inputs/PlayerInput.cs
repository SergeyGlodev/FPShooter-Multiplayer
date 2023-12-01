using UnityEngine;

public abstract class PlayerInput
{
    public float SensitivityX => sensitivityX;
    public float SensitivityY => sensitivityY;

    protected float upperLookLimit = 90.0f;
    protected float lowerLookLimit = 90.0f;
    protected float sensitivityX;
    protected float sensitivityY;
    protected float rotationX = 0f;
    protected float rotationY = 0f;


    public abstract Vector2 GetMovementImput();
    public abstract Quaternion[] GetLookInput();
    public abstract bool SprintPressed();
    public abstract bool JumpPressed();
    public abstract bool CrouchPressed();
    public abstract bool ShootPressed();
    public abstract bool ShootAutoHolding();
    public abstract bool ReloadPressed();
    public abstract bool SettingsPressed();
    public abstract bool WeaponChangePressed(SOWeapon SOWeapon);

    public void SetStartRotateY(float startRotateY) => rotationY = startRotateY;
    public void SetStartRotateX(float startRotateX) => rotationX = startRotateX;
    public void SetSensitivityX(float value)
    {
        sensitivityX = value;
        PlayerPrefs.SetFloat("Sensitivity X", value);
    }
    public void SetSensitivityY(float value)
    {
        sensitivityY = value;
        PlayerPrefs.SetFloat("Sensitivity Y", value);
    }
    public void GetSensitivity()
    {
        sensitivityX = PlayerPrefs.GetFloat("Sensitivity X", defaultValue: 5f);
        sensitivityY = PlayerPrefs.GetFloat("Sensitivity Y", defaultValue: 5f);
    }
}
