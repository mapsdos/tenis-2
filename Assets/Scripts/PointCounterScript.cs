using TMPro;
using UnityEngine;

public class PointCounterScript : MonoBehaviour
{
    public TextMeshPro counter;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball"))
            return;

        BallScoreScript ballScore = other.GetComponent<BallScoreScript>();
        if (ballScore == null || ballScore.hasScored)
            return;

        ballScore.hasScored = true;
        //int currentPoints = int.Parse(counter.text);
        //currentPoints++;
        //counter.text = currentPoints.ToString();
    }
}
