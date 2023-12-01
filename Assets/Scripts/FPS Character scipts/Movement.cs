using UnityEngine;

public class Movement
{
    private const float inputModifyFactor = 0.70f;
    private Vector2 currentInput;
    private float speed;
    private SmoothInputs smoothImputs = new SmoothInputs();

    public Vector3 HandleMovementImput(float speed, Transform transform, Vector3 moveDirection)
    {
        this.speed = speed;

        float h = InputManager.Instance.input.GetMovementImput().x;
        float v = InputManager.Instance.input.GetMovementImput().y;

        currentInput = smoothImputs.SmoothMoveInputs(v, h) * this.speed;

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;

        return moveDirection;
    }

    public Vector3 ApplyFinalMovements(CharacterController characterController, Vector3 moveDirection, float gravity, bool IsGrounded)
    {
        if (!IsGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        Vector3 specificMoveDirection = (currentInput.x != 0 && currentInput.y != 0) ? moveDirection * inputModifyFactor : moveDirection;
        characterController.Move(specificMoveDirection * Time.deltaTime);

        return moveDirection;
    }

    public Vector3 ApplyDownMovement(CharacterController characterController, Vector3 moveDirection, float gravity, bool IsGrounded)
    {
        if (!IsGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        Vector3 downDirection = new Vector3(0f, moveDirection.y, 0f);
        characterController.Move(downDirection * Time.deltaTime);

        return moveDirection;
    }
}
