using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [Header("Target (Car)")]
    public Transform target;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 2.5f, -4f);

    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 2f;

    // Параметри обертання камери (ПКМ)
    private float rmbX = 0f;
    private float rmbY = 15f;
    private bool isRotating = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ========== РЕЖИМ "ВИД НАЗАД" (ЗАТИСНУТЕ КОЛЕСИКО) ==========
        if (Input.GetMouseButton(2))
        {
            // Камера слідкує за машиною (повертається разом з нею)
            Quaternion carRotation = target.rotation;
            // Розвертаємо камеру на 180 градусів відносно машини (вид назад)
            Quaternion cameraRotation = carRotation * Quaternion.Euler(0f, 180f, 0f);

            // Позиція: машина + офсет, але офсет теж розвернутий
            // Офсет має бути позаду, тому при виді назад використовуємо -offset.z
            Vector3 backOffset = new Vector3(offset.x, offset.y, -offset.z);
            Vector3 rotatedOffset = cameraRotation * backOffset;

            transform.position = target.position + rotatedOffset;
            transform.rotation = cameraRotation;
            return;
        }

        // ========== ОСНОВНИЙ РЕЖИМ ==========
        // ПКМ обертання
        if (Input.GetMouseButton(1))
        {
            isRotating = true;
            rmbX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rmbY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }
        else if (isRotating)
        {
            isRotating = false;
            rmbX = 0f;
            rmbY = 15f;
        }

        // Застосовуємо кут для основного режиму
        Quaternion carRotationNormal = target.rotation;
        Quaternion cameraRotationNormal = carRotationNormal * Quaternion.Euler(rmbY, rmbX, 0f);

        Vector3 rotatedOffsetNormal = cameraRotationNormal * offset;
        transform.position = target.position + rotatedOffsetNormal;
        transform.rotation = cameraRotationNormal;
    }
}