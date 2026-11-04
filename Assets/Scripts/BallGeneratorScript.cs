
using UnityEngine;

public class BallGeneratorScript : MonoBehaviour
{
    [Header("Pelotas")]
    public GameObject ball;
    public BoxCollider rivalField;

    [Header("Puntos de salida")]
    public Transform origin1;
    public Transform origin2;
    public Transform origin3;

    [Header("Ángulo de lanzamiento por dificultad (grados)")]
    [Tooltip("Ángulo alto = pelota más lenta.")]
    public float easyLaunchAngle = 50f;
    [Tooltip("Ángulo medio.")]
    public float normalLaunchAngle = 40f;
    [Tooltip("Ángulo bajo = pelota más rápida.")]
    public float hardLaunchAngle = 28f;

    [Header("Configuración actual")]
    [SerializeField, Range(1, 3)]
    private int numberOfActiveGenerators = 3;

    [SerializeField]
    private float targetTime = 3f;

    [SerializeField]
    private float launchAngle = 50f;

    private float counter = 0f;
    private bool isRunning = false;

    // Configuraciones predefinidas de dificultad (velocidad vía ángulo).
    public void SetEasy()
    {
        SetLaunchAngle(easyLaunchAngle);
    }

    public void SetNormal()
    {
        SetLaunchAngle(normalLaunchAngle);
    }

    public void SetHard()
    {
        SetLaunchAngle(hardLaunchAngle);
    }

    // Ángulo más bajo = trayectoria más rápida al mismo punto de impacto.
    public void SetLaunchAngle(float angleDegrees)
    {
        launchAngle = Mathf.Clamp(angleDegrees, 15f, 70f);
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
        // Evita ráfagas si el intervalo quedó a 0 en la escena.
        targetTime = Mathf.Max(0.1f, targetTime);
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
            counter = Mathf.Max(0.1f, targetTime);
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

        Vector3 target = RandomPointInRivalField();

        // Si no hay trayectoria válida, no spawnea una pelota quieta.
        if (!TryGetLaunchVelocity(
            origin, target, launchAngle, out Vector3 velocity))
        {
            Debug.LogWarning(
                "BallGenerator: trayectoria inválida, se omite el lanzamiento."
            );
            return;
        }

        GameObject newBall = Instantiate(
            ball, origin, Quaternion.identity
        );
        newBall.tag = "Ball";

        BallScoreScript score =
            newBall.GetComponent<BallScoreScript>();
        if (score == null)
            score = newBall.AddComponent<BallScoreScript>();
        score.hasScored = false;
        score.wasHitByRacket = false;

        IgnoreCollisionsWithGenerators(newBall);
        Destroy(newBall, 8f);

        Rigidbody ballRb = newBall.GetComponent<Rigidbody>();

        if (ballRb == null)
        {
            Debug.LogWarning("La pelota no tiene Rigidbody.");
            Destroy(newBall);
            return;
        }

        // Si la pelota plantilla está agarrada, el Instantiate copia
        // isKinematic=true y las nuevas salen estáticas.
        ballRb.isKinematic = false;
        ballRb.useGravity = true;
        ballRb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.linearVelocity = velocity;
    }

    private void IgnoreCollisionsWithGenerators(GameObject newBall)
    {
        Collider[] ballColliders =
            newBall.GetComponentsInChildren<Collider>();

        Transform[] origins = { origin1, origin2, origin3 };
        for (int i = 0; i < origins.Length; i++)
        {
            if (origins[i] == null)
                continue;

            Collider[] generatorColliders =
                origins[i].GetComponentsInChildren<Collider>();

            for (int g = 0; g < generatorColliders.Length; g++)
            {
                if (generatorColliders[g] == null
                    || generatorColliders[g].isTrigger)
                    continue;

                for (int b = 0; b < ballColliders.Length; b++)
                {
                    if (ballColliders[b] == null
                        || ballColliders[b].isTrigger)
                        continue;

                    Physics.IgnoreCollision(
                        ballColliders[b], generatorColliders[g], true
                    );
                }
            }
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
