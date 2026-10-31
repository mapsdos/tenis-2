using UnityEngine;
using UnityEngine.XR;

public class DoubleSwipeDash : MonoBehaviour
{
    [Header("Referencias")]
    public Transform headCamera;

    [Header("Configuracion del movimiento")]
    public float dashDistance = 1.5f;
    public float swipeSpeedThreshold = 1.2f;
    public float releaseSpeedThreshold = 0.35f;

    [Header("Deteccion del doble gesto")]
    public float doubleSwipeWindow = 1.3f;
    public float dashCooldown = 0.7f;

    private InputDevice leftController;

    private bool swipeArmed = true;
    private int swipeCount = 0;
    private int swipeDirection = 0;

    private float lastSwipeTime = -10f;
    private float lastDashTime = -10f;

    private void Start()
    {
        if (headCamera == null && Camera.main != null)
            headCamera = Camera.main.transform;

        FindLeftController();
    }

    private void FindLeftController()
    {
        leftController = InputDevices.GetDeviceAtXRNode(
            XRNode.LeftHand
        );
    }

    private void Update()
    {
        if (!leftController.isValid)
            FindLeftController();

        if (!leftController.isValid || headCamera == null)
            return;

        if (!leftController.TryGetFeatureValue(
            CommonUsages.deviceVelocity,
            out Vector3 velocity))
            return;

        // Direccion horizontal del usuario.
        Vector3 right = Vector3.ProjectOnPlane(
            headCamera.right,
            Vector3.up
        ).normalized;

        if (right.sqrMagnitude < 0.01f)
            return;

        // Velocidad del controlador hacia la derecha/izquierda.
        float lateralSpeed = Vector3.Dot(velocity, right);

        // Preparar la deteccion para el siguiente gesto.
        if (Mathf.Abs(lateralSpeed) < releaseSpeedThreshold)
            swipeArmed = true;

        // Evitar detectar varias veces el mismo movimiento.
        if (!swipeArmed ||
            Mathf.Abs(lateralSpeed) < swipeSpeedThreshold)
            return;

        swipeArmed = false;

        // Direccion: -1 izquierda, +1 derecha.
        int direction = lateralSpeed < 0f ? -1 : 1;

        // Si ha pasado demasiado tiempo, empezar de nuevo.
        if (Time.time - lastSwipeTime > doubleSwipeWindow)
        {
            swipeCount = 0;
            swipeDirection = 0;
        }

        // Los dos gestos deben ser en la misma direccion.
        if (swipeCount == 0 || direction != swipeDirection)
        {
            swipeCount = 1;
            swipeDirection = direction;
        }
        else
        {
            swipeCount++;
        }

        lastSwipeTime = Time.time;

        // Dos gestos en la misma direccion.
        if (swipeCount >= 2 &&
            Time.time - lastDashTime >= dashCooldown)
        {
            Dash(direction, right);

            lastDashTime = Time.time;
            swipeCount = 0;
            swipeDirection = 0;
        }
    }

    private void Dash(int direction, Vector3 right)
    {
        Vector3 displacement = right * direction * dashDistance;

        // Desplazamiento horizontal de 1,5 metros.
        transform.position += displacement;
    }
}