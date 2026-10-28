using System;
using UnityEngine;

public class BallGeneratorScript : MonoBehaviour
{
    public float targetTime;
    public GameObject ball;
    private float counter;
    public GameObject origin1;
    public GameObject origin2;
    public GameObject origin3;
    public BoxCollider rivalField;
    public float launchAngle = 40f;

    private void throwBall(Vector3 origin)
    {
        GameObject ball = Instantiate(this.ball, origin, Quaternion.identity);
        Destroy(ball, 10f);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            Vector3 target = RandomPointInRivalField();
            if (TryGetLaunchVelocity(origin, target, launchAngle, out Vector3 velocity))
            {
                ballRb.linearVelocity = velocity;
            }
        }
    }

    private Vector3 RandomPointInRivalField()
    {
        Bounds b = rivalField.bounds;
        return new Vector3(
            UnityEngine.Random.Range(b.min.x, b.max.x),
            b.center.y,
            UnityEngine.Random.Range(b.min.z, b.max.z)
        );
    }

    private bool TryGetLaunchVelocity(Vector3 origin, Vector3 target, float angleDeg, out Vector3 velocity)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        Vector3 toTarget = target - origin;
        float dy = toTarget.y;
        toTarget.y = 0f;
        float dx = toTarget.magnitude;

        float angle = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float denom = 2f * cos * cos * (dx * Mathf.Tan(angle) - dy);

        if (dx < 0.01f || denom <= 0f)
        {
            velocity = Vector3.zero;
            return false;
        }

        float speed = Mathf.Sqrt(gravity * dx * dx / denom);
        velocity = toTarget.normalized * (speed * cos) + Vector3.up * (speed * sin);
        return true;
    }

    private Vector3 randomOrigin(Array origins)
    {
        int randomIndex = UnityEngine.Random.Range(0, origins.Length);
        return (Vector3)origins.GetValue(randomIndex);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = targetTime;
    }

    // Update is called once per frame
    void Update()
    {
        counter -= Time.deltaTime;

        if (counter <= 0.0f)
        {
            Array origins = new Vector3[] { origin1.transform.position, origin2.transform.position, origin3.transform.position };
            Vector3 ballOrigin = randomOrigin(origins);
            throwBall(ballOrigin);
            counter = targetTime;
        }
    }
}