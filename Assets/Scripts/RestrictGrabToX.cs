using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class RestrictGrabToX : MonoBehaviour
{
    [Header("Referencia")]
    public XRGrabInteractable grabInteractable;

    [Header("Limites del eje X (coordenadas del mundo)")]
    public float minX = -4f;
    public float maxX = 1.5f;

    private bool wasGrabbed;
    private float lockedY;
    private float lockedZ;

    private void LateUpdate()
    {
        if (grabInteractable == null)
            return;

        bool isGrabbed = grabInteractable.isSelected;

        if (isGrabbed)
        {
            if (!wasGrabbed)
            {
                // Guardar la altura y profundidad al empezar a agarrar.
                lockedY = transform.position.y;
                lockedZ = transform.position.z;
            }

            Vector3 pos = transform.position;

            // Permitir movimiento solo en X.
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = lockedY;
            pos.z = lockedZ;

            transform.position = pos;
        }

        wasGrabbed = isGrabbed;
    }
}