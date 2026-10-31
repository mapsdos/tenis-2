using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

/// <summary>
/// Limita el agarre XR a desplazamiento en el eje X,
/// </summary>
public class RestrictGrabToX : XRBaseGrabTransformer
{
    [Header("Limites del eje X")]
    [Tooltip("Si está asignado, minX/maxX son coordenadas locales de este transform. Si no, se usa el padre al iniciar.")]
    public Transform limitsSpace;

    public float minX = -4f;
    public float maxX = 1.5f;

    private Transform cachedLimitsSpace;
    private XRGrabInteractable cachedGrab;
    private float lockedY;
    private float lockedZ;
    private Quaternion lockedRotation;
    private Rigidbody cachedBody;

    protected override RegistrationMode registrationMode =>
        RegistrationMode.SingleAndMultiple;

    protected void Awake()
    {
        cachedLimitsSpace = limitsSpace != null ? limitsSpace : transform.parent;
        TryGetComponent(out cachedBody);

        if (!TryGetComponent(out cachedGrab))
            return;

        XRGrabInteractable grab = cachedGrab;

        // Solo movimiento en X del mundo; sin rotar ni escalar al agarrar.
        if (TryGetComponent(out XRGeneralGrabTransformer general))
        {
            general.permittedDisplacementAxes =
                XRGeneralGrabTransformer.ManipulationAxes.X;
            general.constrainedAxisDisplacementMode =
                XRGeneralGrabTransformer.ConstrainedAxisDisplacementMode
                    .WorldAxisRelative;
            general.allowOneHandedScaling = false;
            general.allowTwoHandedScaling = false;
        }

        grab.throwOnDetach = false;
        grab.trackRotation = false;
        grab.trackScale = false;

        if (cachedBody != null)
        {
            cachedBody.useGravity = false;
            cachedBody.constraints =
                RigidbodyConstraints.FreezePositionY
                | RigidbodyConstraints.FreezePositionZ
                | RigidbodyConstraints.FreezeRotation;
        }

        ClampTransformNow();
    }

    public override void OnGrab(XRGrabInteractable grabInteractable)
    {
        base.OnGrab(grabInteractable);

        Vector3 pos = grabInteractable.transform.position;
        lockedY = pos.y;
        lockedZ = pos.z;
        lockedRotation = grabInteractable.transform.rotation;
    }

    public override void Process(
        XRGrabInteractable grabInteractable,
        XRInteractionUpdateOrder.UpdatePhase updatePhase,
        ref Pose targetPose,
        ref Vector3 localScale)
    {
        if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic
            && updatePhase != XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender)
        {
            return;
        }

        GetWorldXLimits(out float worldMinX, out float worldMaxX);

        targetPose.position = new Vector3(
            Mathf.Clamp(targetPose.position.x, worldMinX, worldMaxX),
            lockedY,
            lockedZ
        );
        targetPose.rotation = lockedRotation;
    }

    private void FixedUpdate()
    {
        // Durante el agarre el transformer ya aplica los límites.
        if (cachedGrab != null && cachedGrab.isSelected)
            return;

        ClampTransformNow();
    }

    private void ClampTransformNow()
    {
        GetWorldXLimits(out float worldMinX, out float worldMaxX);

        Vector3 pos = transform.position;
        float clampedX = Mathf.Clamp(pos.x, worldMinX, worldMaxX);

        if (Mathf.Approximately(pos.x, clampedX))
            return;

        pos.x = clampedX;

        if (cachedBody != null)
            cachedBody.MovePosition(pos);
        else
            transform.position = pos;
    }

    private void GetWorldXLimits(out float worldMinX, out float worldMaxX)
    {
        if (cachedLimitsSpace == null)
        {
            worldMinX = Mathf.Min(minX, maxX);
            worldMaxX = Mathf.Max(minX, maxX);
            return;
        }

        float a = cachedLimitsSpace.TransformPoint(new Vector3(minX, 0f, 0f)).x;
        float b = cachedLimitsSpace.TransformPoint(new Vector3(maxX, 0f, 0f)).x;
        worldMinX = Mathf.Min(a, b);
        worldMaxX = Mathf.Max(a, b);
    }
}
