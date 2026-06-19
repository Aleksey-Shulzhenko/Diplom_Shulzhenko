using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxSpeed = 120f;
    public float acceleration = 6f;
    public float brakeForce = 35f;
    public float reverseSpeed = 40f;
    public float reverseAcceleration = 5f;
    public float turnSpeed = 140f;

    [Header("Steering Realism")]
    public float speedSteeringFactor = 0.7f;

    [Header("Coasting")]
    public float drag = 2f;

    [Header("Wheels Visuals")]
    public Transform wheelFrontLeft;
    public Transform wheelFrontRight;
    public Transform wheelRearLeft;
    public Transform wheelRearRight;
    public float maxWheelAngle = 35f;
    public float wheelTurnSpeed = 5f;
    public float wheelRotateSpeed = 2000f;

    private float currentWheelRotation = 0f;
    private float currentWheelAngle = 0f;
    private float currentSpeed = 0f;
    private Rigidbody rb;
    private bool controlsEnabled = true;

    private KeyCode gasKey;
    private KeyCode brakeKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 2000f;
        rb.linearDamping = 1f;
        rb.angularDamping = 8f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        rb.constraints = RigidbodyConstraints.None;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        LoadKeys();
    }

    void LoadKeys()
    {
        gasKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Gas", "W"));
        brakeKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Brake", "S"));
        leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "A"));
        rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D"));
    }

    public void SetControlEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }

    public void StopCar()
    {
        currentSpeed = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    void FixedUpdate()
    {
        if (!controlsEnabled) return;

        // ========== ПРИТИСКАННЯ ДО ЗЕМЛІ ТА НАХИЛ ==========
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 1.5f))
        {
            // Ставимо на землю
            Vector3 pos = transform.position;
            pos.y = hit.point.y + 0.05f;
            transform.position = pos;

            // НАХИЛ (машина повторює кут поверхні)
            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.eulerAngles.y, targetRotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }

        // ========== ДОДАТКОВЕ ПРИТИСКАННЯ ЩОБ НЕ ПІДЛІТАЛА ==========
        if (currentSpeed > 5f)
        {
            RaycastHit downHit;
            if (Physics.Raycast(transform.position, Vector3.down, out downHit, 1.2f))
            {
                float angle = Vector3.Angle(downHit.normal, Vector3.up);
                if (angle > 10f)
                {
                    rb.AddForce(Vector3.down * 15f, ForceMode.Acceleration);
                }
            }
        }

        // ========== КЕРУВАННЯ ==========
        float gas = 0f;
        float turnInput = 0f;

        if (Input.GetKey(gasKey)) gas = 1f;
        if (Input.GetKey(brakeKey)) gas = -1f;
        if (Input.GetKey(leftKey)) turnInput = 1f;
        if (Input.GetKey(rightKey)) turnInput = -1f;

        if (gas > 0)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        }
        else if (gas < 0)
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= brakeForce * Time.fixedDeltaTime;
                if (currentSpeed < 0) currentSpeed = 0;
            }
            else
            {
                currentSpeed -= reverseAcceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Max(currentSpeed, -reverseSpeed);
            }
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= drag * Time.fixedDeltaTime;
                if (currentSpeed < 0) currentSpeed = 0;
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += drag * Time.fixedDeltaTime;
                if (currentSpeed > 0) currentSpeed = 0;
            }
        }

        rb.linearVelocity = transform.forward * currentSpeed;

        // ========== ОБЕРТАННЯ КОЛІС ==========
        float rotationDelta = (Mathf.Abs(currentSpeed) / maxSpeed) * wheelRotateSpeed * Time.fixedDeltaTime;
        float direction = Mathf.Sign(currentSpeed);
        currentWheelRotation += rotationDelta * direction;

        if (wheelRearLeft != null)
            wheelRearLeft.localRotation = Quaternion.Euler(currentWheelRotation, 0f, 0f);
        if (wheelRearRight != null)
            wheelRearRight.localRotation = Quaternion.Euler(currentWheelRotation, 0f, 0f);

        float targetWheelAngle = turnInput * maxWheelAngle;
        currentWheelAngle = Mathf.Lerp(currentWheelAngle, targetWheelAngle, wheelTurnSpeed * Time.fixedDeltaTime);

        if (wheelFrontLeft != null)
            wheelFrontLeft.localRotation = Quaternion.Euler(currentWheelRotation, currentWheelAngle, 0f);
        if (wheelFrontRight != null)
            wheelFrontRight.localRotation = Quaternion.Euler(currentWheelRotation, currentWheelAngle, 0f);

        // ========== ПОВОРОТ МАШИНИ ==========
        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(currentSpeed) / maxSpeed);
            float steeringMultiplier = 1f - (speedFactor * speedSteeringFactor);
            float turnAmount = turnInput * turnSpeed * steeringMultiplier * Time.fixedDeltaTime;
            transform.Rotate(0f, turnAmount, 0f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!controlsEnabled) return;

        if (collision.relativeVelocity.magnitude > 10f)
        {
            currentSpeed *= 0.7f;
            Vector3 direction = (transform.position - collision.transform.position).normalized;
            rb.AddForce(direction * 8f, ForceMode.Impulse);
        }
    }
}