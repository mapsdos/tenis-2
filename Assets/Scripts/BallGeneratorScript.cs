
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

    [Header("Intervalos por dificultad (segundos)")]
    public float easyInterval = 3f;
    public float normalInterval = 1.8f;
    public float hardInterval = 1f;

    [Header("Configuración actual")]
    [SerializeField, Range(1, 3)]
    private int numberOfActiveGenerators = 1;

    [SerializeField]
    private float targetTime = 3f;

    private float counter = 0f;
    private bool isRunning = false;

    // Configuraciones predefinidas de dificultad.
    public void SetEasy()
    {
        SetDifficulty(1, easyInterval);
    }

    public void SetNormal()
    {
        SetDifficulty(2, normalInterval);
    }

    public void SetHard()
    {
        SetDifficulty(3, hardInterval);
    }

    private void SetDifficulty(int generators, float interval)
    {
        numberOfActiveGenerators = Mathf.Clamp(generators, 1, 3);
        targetTime = Mathf.Max(0.1f, interval);

        // Reinicia el contador para aplicar la nueva configuración.
        counter = targetTime;
    }

    // Permite que el slider cambie el número de generadores activos.
    public void SetNumberOfActiveGenerators(float value)
    {
        numberOfActiveGenerators = Mathf.Clamp(
            Mathf.RoundToInt(value), 1, 3
        );
    }

    // Permite que el slider cambie el intervalo entre pelotas.
    public void SetLaunchInterval(float interval)
    {
        targetTime = Mathf.Max(0.1f, interval);
        counter = Mathf.Min(counter, targetTime);
    }

    // Inicia la generación de pelotas.
    public void StartGeneration()
    {
        counter = 0f;
        isRunning = true;
    }

    // Detiene los nuevos lanzamientos.
    public void StopGeneration()
    {
        isRunning = false;
        counter = 0f;
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

    // Elige aleatoriamente uno de los generadores activos.
    private void SpawnBalls()
    {
        Transform[] origins = { origin1, origin2, origin3 };

        // Construye la lista de generadores que realmente existen.
        Transform[] activeOrigins = new Transform[numberOfActiveGenerators];
        int availableCount = 0;

        for (int i = 0; i < numberOfActiveGenerators; i++)
        {
            if (origins[i] != null)
            {
                activeOrigins[availableCount] = origins[i];
                availableCount++;
            }
        }

        if (availableCount == 0)
        {
            Debug.LogWarning(
                "BallGenerator: no hay generadores activos asignados."
            );
            return;
        }

        int index = Random.Range(0, availableCount);
        Transform selectedGenerator = activeOrigins[index];

        ThrowBall(selectedGenerator.position);
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
