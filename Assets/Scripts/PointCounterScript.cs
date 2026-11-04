using TMPro;
using UnityEngine;

public class PointCounterScript : MonoBehaviour
{
    public TextMeshPro counter;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        BallScoreScript ballScore =
            other.GetComponent<BallScoreScript>()
            ?? other.GetComponentInParent<BallScoreScript>();

        if (ballScore == null || ballScore.hasScored)
            return;

        // Solo cuenta si la raqueta tocó la pelota antes.
        if (!ballScore.wasHitByRacket)
            return;

        ballScore.hasScored = true;
        int currentPoints = int.Parse(counter.text);
        currentPoints++;
        counter.text = currentPoints.ToString();
    }
}
