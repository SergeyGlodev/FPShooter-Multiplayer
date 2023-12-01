using UnityEngine;

public class SmoothInputs
{
    private float slidingV;
    private float slidingH;

    public Vector2 SmoothMoveInputs(float targetV, float targetH)
    {
        float sensitivity = 4.5f;
        float deadZone = 0.001f;

        slidingV = Mathf.MoveTowards(slidingV,
                      targetV, sensitivity * Time.deltaTime);

        slidingH = Mathf.MoveTowards(slidingH,
                      targetH, sensitivity * Time.deltaTime);

        return new Vector2(
               (Mathf.Abs(slidingV) < deadZone) ? 0f : slidingV,
               (Mathf.Abs(slidingH) < deadZone) ? 0f : slidingH);
    }
}
