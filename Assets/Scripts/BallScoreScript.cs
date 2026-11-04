using UnityEngine;

public class BallScoreScript : MonoBehaviour
{
    public bool hasScored;
    public bool wasHitByRacket;

    private void OnCollisionEnter(Collision collision)
    {
        if (wasHitByRacket || collision.collider == null)
            return;

        if (HasTagInParents(collision.collider.transform, "Racket"))
            wasHitByRacket = true;
    }

    private static bool HasTagInParents(Transform current, string tag)
    {
        while (current != null)
        {
            if (current.CompareTag(tag))
                return true;

            current = current.parent;
        }

        return false;
    }
}
