
using UnityEngine;

public class BallGeneratorScript : MonoBehaviour
{
    [Header("Pelotas")]
    public GameObject ball;
    public BoxCollider rivalField;
    public float launchAngle = 40f;

    [Header("Puntos de salida")]
    public Transform origin1;
    public Transform origin2;
    public Transform origin3;

    [Header("Dificultad")]
    public float easyInterval = 3f;
    public float normalInterval = 1.5f;
    public float hardInterval = 0.8f;

    private float targetTime;
    private float counter;
    private int difficulty = 0;
    private int selectedOrigin = 0;
    private bool isRunning = false;

    // Empieza siempre en dificultad fácil.
    public void StartGeneration()
    {
        SetDifficulty(0);
        counter = 0f;
        isRunning = true;
    }

    // Detiene inmediatamente los nuevos lanzamientos.
    public void StopGeneration()
    {
        isRunning = false;
        counter = 0f;
    }

    public void SetEasy()
    {
        SetDifficulty(0);
    }

    public void SetNormal()
    {
        SetDifficulty(1);
    }

    public void SetHard()
    {
        SetDifficulty(2);
    }

    private void SetDifficulty(int newDifficulty)
    {
        difficulty = newDifficulty;

        switch (difficulty)
        {
            case 0:
                targetTime = easyInterval;
                break;

            case 1:
                targetTime = normalInterval;
                break;

            case 2:
                targetTime = hardInterval;
                break;
        }

        // Si ya está funcionando, aplica la nueva frecuencia.
        counter = targetTime;
    }

    // El Slider selecciona el punto inicial: 1, 2 o 3.
    public void SetSelectedOrigin(float sliderValue)
    {
        selectedOrigin = Mathf.Clamp(
            Mathf.RoundToInt(sliderValue) - 1, 0, 2
        );
    }

    private void Update()
    {
        if (!isRunning)
            return;

        counter -= Time.deltaTime;

        if (counter <= 0f)
        {
            SpawnBalls();
            counter = targetTime;
        }
    }

    private void SpawnBalls()
    {
        // La dificultad determina cuántos puntos participan.
        int numberOfOrigins = difficulty + 1;

        Transform[] origins = { origin1, origin2, origin3 };

        for (int i = 0; i < numberOfOrigins; i++)
        {
            // El Slider indica desde qué punto empezamos.
            int index = (selectedOrigin + i) % origins.Length;
            Transform spawn = origins[index];

            if (spawn != null)
                ThrowBall(spawn.position);
        }
    }

    private void ThrowBall(Vector3 origin)
    {
        if (ball == null || rivalField == null)
        {
            Debug.LogWarning(
                "BallGenerator: falta asignar la pelota o rivalField."
            );
            return;
        }

        GameObject newBall = Instantiate(
            ball, origin, Quaternion.identity
        );

        Destroy(newBall, 10f);

        Rigidbody ballRb = newBall.GetComponent<Rigidbody>();

        if (ballRb == null)
        {
            Debug.LogWarning("La pelota no tiene Rigidbody.");
            return;
        }

        Vector3 target = RandomPointInRivalField();

        if (TryGetLaunchVelocity(
            origin, target, launchAngle, out Vector3 velocity))
        {
            ballRb.linearVelocity = velocity;
        }
    }

    private Vector3 RandomPointInRivalField()
    {
        Bounds b = rivalField.bounds;

        return new Vector3(
            Random.Range(b.min.x, b.max.x),
            b.center.y,
            Random.Range(b.min.z, b.max.z)
        );
    }

    private bool TryGetLaunchVelocity(
        Vector3 origin,
        Vector3 target,
        float angleDeg,
        out Vector3 velocity)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 toTarget = target - origin;
        float dy = toTarget.y;
        toTarget.y = 0f;

        float dx = toTarget.magnitude;
        float angle = angleDeg * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        float denom =
            2f * cos * cos * (dx * Mathf.Tan(angle) - dy);

        if (dx < 0.01f || denom <= 0f || gravity <= 0f)
        {
            velocity = Vector3.zero;
            return false;
        }

        float speed = Mathf.Sqrt(
            gravity * dx * dx / denom
        );

        velocity =
            toTarget.normalized * (speed * cos)
            + Vector3.up * (speed * sin);

        return true;
    }
}
