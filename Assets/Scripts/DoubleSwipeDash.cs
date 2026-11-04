using UnityEngine;
using UnityEngine.XR;

public class DoubleSwipeDash : MonoBehaviour
{
    [Header("Referencias")]
    public Transform headCamera;

    [Header("Configuracion del movimiento")]
    public float dashDistance = 2f;
    public float swipeSpeedThreshold = 1.2f;
    public float releaseSpeedThreshold = 0.35f;

    private InputDevice leftController;

    private bool swipeArmed = true;
    private int swipeCount = 0;
    private int swipeDirection = 0;

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

        Dash(direction, right);
    }

    private void Dash(int direction, Vector3 right)
    {
        Vector3 displacement = right * direction * dashDistance;

        // Desplazamiento horizontal de 1,5 metros.
        transform.position += displacement;
    }
}