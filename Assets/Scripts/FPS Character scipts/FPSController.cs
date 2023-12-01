using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

public class FPSController : MonoBehaviour, IPunObservable
{
    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float crouchSpeed = 3f;

    [Header("Look Parameters")]
    [SerializeField] private Transform firstPersonView;
    [SerializeField] private Camera playerCamera;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = 20f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2.2f;
    [SerializeField] private float timeToCrouch = 0.3f;

    [Header("Other")]
    [SerializeField] private GameObject avatarBody;
    [SerializeField] private FPSPlayerAnimations playerAnimation;
    [SerializeField] private LayerMask nonPlayerMask;

    public Transform FirstPersonView => firstPersonView;

    private bool CanMove { get; set; } = false;
    private bool IsSprinting => canSprint 
                                && InputManager.Instance.input.SprintPressed();
    private bool ShouldJump => canJump 
                               && InputManager.Instance.input.JumpPressed() 
                               && IsGrounded;
    private bool ShouldCrouch => canCrouch 
                                 && InputManager.Instance.input.CrouchPressed()
                                 && IsGrounded && !duringCrouchAnimation;
    private bool IsGrounded => Physics.Raycast(firstPersonView.position, Vector3.down, 
                                firstPersonView.localPosition.y + bufferCheckDistance, 
                                nonPlayerMask);
    private bool IsPistolShoot => !weaponManager.CurrentWeapon.SOWeapon.IsAuto 
                                   && InputManager.Instance.input.ShootPressed();
    private bool IsAutoShoot => weaponManager.CurrentWeapon.SOWeapon.IsAuto 
                                && InputManager.Instance.input.ShootAutoHolding();
    private bool CanShoot => Time.time > weaponManager.CurrentWeapon.NextTimeToFire 
                             && weaponManager.CurrentWeapon.CurrentAmmo > 0 
                             && !isReloading;
    private bool ShouldReload => CanReload 
                                 && (InputManager.Instance.input.ReloadPressed()
                                     || weaponManager.CurrentWeapon.CurrentAmmo == 0);
    private bool CanReload => !isReloading 
                               && !weaponManager.CurrentWeapon.IsAmmoFull();
    
    private List<SOWeapon> SOWeaponList = new List<SOWeapon>();
    private CharacterController characterController;
    private PlayerHealth playerHealth;
    private FPSShootingControls shootingControls;
    private Tween reloadWaiter;
    private PhotonView view;
    private CapsuleCollider characterCollider;
    private UiGameplay uiGameplay;
    private Movement movement;
    private WeaponManager weaponManager;
    private UiManager uiManager;
    private Vector3 firstPersonViewRotation = Vector3.zero;
    private Vector3 defaultCamPos;
    private Vector3 moveDirection;
    private Vector3 defaultPlayerPosition;
    private Quaternion defaultPlayerRotation;
    private Quaternion defaultCameraRotation;
    private float rotationX = 0f;
    private float bufferCheckDistance = 0.1f;
    private float speed;
    private bool isCrouching;
    private bool duringCrouchAnimation;
    private bool isReloading;
    private bool inPause;


    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        shootingControls = GetComponent<FPSShootingControls>();
        characterCollider = GetComponent<CapsuleCollider>();
        weaponManager = GetComponent<WeaponManager>();
        playerHealth = GetComponent<PlayerHealth>();
        view = GetComponent<PhotonView>();
        uiManager = FindObjectOfType<UiManager>();

        movement = new Movement();
        InitView();

        for (int i = 0; i < weaponManager.weaponStorage.weapons.Length; i++)
        {
            SOWeaponList.Add(weaponManager.SOWeaponList[i]);
            weaponManager.firstViewWeaponStorage.weapons[i].GetComponent<FPSHandsWeapon>()
                .delayMuzzleFlashBeforeFade = SOWeaponList[i].DelayMuzzleFlashBeforeFade;
            weaponManager.firstViewWeaponStorage.weapons[i].GetComponent<FPSHandsWeapon>()
                .delayToPlayReloadSound = SOWeaponList[i].DelayToPlayReloadSound;
        }
        weaponManager.weaponStorage.weapons[0].gameObject.SetActive(true);
        weaponManager.firstViewWeaponStorage.weapons[0].gameObject.SetActive(true);

        firstPersonView.localEulerAngles = firstPersonViewRotation;
        defaultCamPos = firstPersonView.localPosition;

        defaultCameraRotation = playerCamera.transform.localRotation;
        defaultPlayerPosition = transform.position;
        defaultPlayerRotation = transform.rotation;

        playerHealth.OnDeadBool += DeathScreenOffOn;
        playerHealth.OnDeadBool += ColliderOnOff;
        playerHealth.OnResurrectBool += ColliderOnOff;
        weaponManager.OnSelectWeaponStart += OffReloadLogic;
        weaponManager.OnSelectWeaponEnd += PlayChangeWeaponAnimation;
        GlobalEvents.OnGameStarted += StartMove;
        GlobalEvents.OnGameRestarted += SetStartPositionRotation;
        GlobalEvents.OnGameRestarted += SetAmmoToFull;


        void InitView()
        {
            if (view.IsMine)
            {
                avatarBody.SetActive(false);
                for (int i = 0; i < weaponManager.weaponStorage.weapons.Length; i++)
                {
                    weaponManager.weaponStorage.weapons[i].GetComponent<FPSWeapon>()
                        .weaponModel.SetActive(false);
                }
                uiGameplay = uiManager.UiGameplay;
                uiGameplay.restartButton.onClick.AddListener(RestartGame);
                UiUpdate();

                InputManager.Instance.input.SetStartRotateY(transform.eulerAngles.y);
                InputManager.Instance.input.SetStartRotateX(transform.eulerAngles.x);

                playerHealth.OnDeadBool += playerAnimation.PlayerDeath;
                playerHealth.OnResurrectBool += playerAnimation.PlayerDeath;

                GlobalEvents.OnPlayerSet?.Invoke(this);
            }
            else
            {
                Destroy(GetComponent<CharacterController>());
                firstPersonView.gameObject.SetActive(false);

                gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
        }
    }

    private void OnDestroy()
    {
        playerHealth.OnDeadBool -= DeathScreenOffOn;
        playerHealth.OnDeadBool -= ColliderOnOff;
        playerHealth.OnResurrectBool -= ColliderOnOff;
        weaponManager.OnSelectWeaponStart -= OffReloadLogic;
        weaponManager.OnSelectWeaponEnd -= PlayChangeWeaponAnimation;
        GlobalEvents.OnGameStarted -= StartMove;
        GlobalEvents.OnGameRestarted -= SetStartPositionRotation;
        GlobalEvents.OnGameRestarted -= SetAmmoToFull;

        if (view.IsMine)
        {
            playerHealth.OnDeadBool -= playerAnimation.PlayerDeath;
            playerHealth.OnResurrectBool -= playerAnimation.PlayerDeath;
        }
    }

    private void Update()
    {
        if (!view.IsMine)
        {
            return;
        }

        UiUpdate();
        OpenSettings();

        if (playerHealth.IsDead)
        {
            return;
        }

        if (!CanMove)
        {
            moveDirection = movement.ApplyDownMovement(characterController, moveDirection,
                                                       gravity, IsGrounded);
            return;
        }

        if (uiManager.IsSettingsOpen)
        {
            return;
        }

        SpeedCheck();
        moveDirection = movement.HandleMovementImput(speed, transform, moveDirection);
        HandleJump();
        HandleCrouch();
        moveDirection = movement.ApplyFinalMovements(characterController, moveDirection,
                                                     gravity, IsGrounded);
        HandleMouseLook();
        view.RPC(nameof(HandleAnimationsRpc), RpcTarget.All, 
                 characterController.velocity.magnitude, characterController.velocity.y, 
                 isCrouching, isReloading);
        HandleShooting();
        SelectWeapon();
    }

    public void ReduceHealth(float damage) => playerHealth.TakeDamage(damage);

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            Vector3 pos = transform.localPosition;
            stream.Serialize(ref pos);
        }
        else
        {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);
        }
    }

    #region OnlyInEventMethods

    private void DeathScreenOffOn(bool isDead)
    {
        if (view.IsMine)
        {
            uiGameplay.deathScreen.SetActive(isDead);
        }
    }

    private void ColliderOnOff(bool isDead)
    {
        characterCollider.enabled = !isDead;

        if (view.IsMine)
        {
            uiGameplay.deathScreen.SetActive(isDead);
            weaponManager.firstViewWeaponStorage.gameObject.SetActive(!isDead);
            if (!isDead)
            {
                weaponManager.CurrentWeapon.SetNextTimeToFire(Time.time
                    + weaponManager.CurrentWeapon.DelayAfterTakeWeapon);
            }
        }
    }

    private void OffReloadLogic()
    {
        CancelDelayCall(reloadWaiter);
        isReloading = false;

        void CancelDelayCall(Tween DelayedCallTween)
        {
            if (DelayedCallTween != null && DelayedCallTween.IsActive())
            {
                DelayedCallTween.Kill();
            }
        }
    }

    private void PlayChangeWeaponAnimation(int index) =>
        playerAnimation.ChangeController(isPistol: SOWeaponList[index].IsPistol);

    private void StartMove() => CanMove = true;

    private void SetStartPositionRotation()
    {
        if (view.IsMine)
        {
            SetStartPosition();
            InputManager.Instance.input.SetStartRotateY(transform.eulerAngles.y);
            InputManager.Instance.input.SetStartRotateX(transform.eulerAngles.x);
            CanMove = false;
        }

        void SetStartPosition()
        {
            if (!view.IsMine)
            {
                return;
            }
            characterController.enabled = false;
            transform.position = defaultPlayerPosition;
            transform.rotation = defaultPlayerRotation;
            playerCamera.transform.localRotation = defaultCameraRotation;
            characterController.enabled = true;
        }
    }

    private void SetAmmoToFull()
    {
        if (view.IsMine)
        {
            for (int i = 0; i < weaponManager.weaponStorage.weapons.Length; i++)
            {
                weaponManager.weaponStorage.weapons[i]
                    .GetComponent<FPSWeapon>().SetAmmoFull();
            }
            weaponManager.SelectTargetWeapon(SOWeaponList[0].WeaponIndex);
        }
    }

    private void RestartGame() => view.RPC(nameof(RestartGameRpc), RpcTarget.All);

    #endregion

    #region InUpdateMethods

    private void UiUpdate()
    {
        uiGameplay.currentAmmo.text = weaponManager.CurrentWeapon.CurrentAmmo.ToString();
        uiGameplay.maximumAmmo.text = weaponManager.CurrentWeapon.MaximumAmmo.ToString();
        uiGameplay.health.text = playerHealth.Health.ToString();
    }

    private void OpenSettings()
    {
        if (InputManager.Instance.input.SettingsPressed() && !uiManager.IsSettingsOpen)
        {
            GlobalEvents.OnInGameplaySettingsOpen?.Invoke();
        }
        else if (InputManager.Instance.input.SettingsPressed() && uiManager.IsSettingsOpen)
        {
            GlobalEvents.OnInGameplaySettingsClose?.Invoke();
        }
    }

    private void SpeedCheck()
    {
        if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else if (IsSprinting)
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }

    private void HandleJump()
    {
        if (ShouldJump && !isCrouching)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            if (isCrouching && Physics.Raycast(playerCamera.transform.position,
                Vector3.up, 1f))
            {
                return;
            }
            view.RPC(nameof(StandCrouch), RpcTarget.All);
        }
    }

    private void HandleMouseLook()
    {
        playerCamera.transform.localRotation = InputManager.Instance.input.GetLookInput()[0];
        transform.rotation = InputManager.Instance.input.GetLookInput()[1];
    }

    private void HandleShooting()
    {
        if (!weaponManager.CurrentWeapon.WeaponIsReady())
        {
            return;
        }

        if ((IsPistolShoot && CanShoot) || (IsAutoShoot && CanShoot))
        {
            view.RPC(nameof(DoShoot), RpcTarget.All);
        }

        if (ShouldReload)
        {
            DoReload();
        }


        void DoReload()
        {
            isReloading = true;
            weaponManager.CurrentFirstViewWeapon.PlayReloadVisual();

            reloadWaiter = DOVirtual.DelayedCall(weaponManager.CurrentWeapon.ReloadTime,
                WaitReload);
        }

        void WaitReload()
        {
            if (isReloading)
            {
                weaponManager.CurrentWeapon.SetAmmoFull();
                isReloading = false;
            }
        }
    }

    private void SelectWeapon() => weaponManager.SelectWeapon();

    #endregion

    #region PunRPCMethods

    [PunRPC]
    private void RestartGameRpc() => GlobalEvents.OnGameRestarted?.Invoke();

    [PunRPC]
    private void StandCrouch()
    {
        duringCrouchAnimation = true;

        Vector3 crouchCenter = new Vector3(0, crouchHeight / 2, 0);
        Vector3 standingCenter = new Vector3(0, standingHeight / 2, 0);
        Vector3 crouchCamPos = new Vector3(0, defaultCamPos.y / 2, 0);

        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchCenter;
        Vector3 targerCamPos = isCrouching ? defaultCamPos : crouchCamPos;

        isCrouching = !isCrouching;
        if (view.IsMine)
        {
            DOTween.To(() => characterController.height, x =>
                characterController.height = x, targetHeight, timeToCrouch);
            DOTween.To(() => characterController.center, x =>
                characterController.center = x, targetCenter, timeToCrouch);
        }
        DOTween.To(() => characterCollider.height, x =>
            characterCollider.height = x, targetHeight, timeToCrouch);
        DOTween.To(() => characterCollider.center, x =>
            characterCollider.center = x, targetCenter, timeToCrouch);
        DOTween.To(() => firstPersonView.localPosition, x =>
            firstPersonView.localPosition = x, targerCamPos, timeToCrouch)
            .OnComplete(() =>
            {
                if (view.IsMine)
                {
                    characterController.height = targetHeight;
                    characterController.center = targetCenter;
                }
                duringCrouchAnimation = false;
            });
    }

    [PunRPC]
    private void HandleAnimationsRpc(float velocityMagnitude, float velocityY,
        bool isCrouching, bool isReloading)
    {
        playerAnimation.Movement(velocityMagnitude);
        playerAnimation.PlayerCrouchWalk(velocityMagnitude);
        playerAnimation.PlayerJump(velocityY);
        playerAnimation.PlayerCrouch(isCrouching);
        playerAnimation.IsReloading(isReloading);
    }

    [PunRPC]
    private void DoShoot()
    {
        weaponManager.CurrentWeapon.SetNextTimeToFire(Time.time
            + 1f / weaponManager.CurrentWeapon.FireRate);
        playerAnimation.Shoot(isCrouching);

        if (!view.IsMine)
        {
            weaponManager.CurrentWeapon.MuzzleFlashShoot();
        }
        if (view.IsMine)
        {
            shootingControls.ShootVisual();
            weaponManager.CurrentFirstViewWeapon.OnShootMuzzleFlash();
            weaponManager.CurrentFirstViewWeapon.OnShootAudio();
            weaponManager.CurrentFirstViewWeapon.OnShootAnim();
            weaponManager.CurrentWeapon.DecreaseAmmoInfo();
            shootingControls.ShootDamage(weaponManager.CurrentWeapon.Damage);
        }
    }
    #endregion
}