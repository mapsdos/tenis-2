using UnityEngine;

public class TennisRacket : MonoBehaviour
{
    [Header("Configuración del Golpe")]
    [Tooltip("Multiplicador de fuerza transmitida a la pelota al golpear")]
    //public float hitPowerMultiplier = 2.0f;

    [Header("Ajustes de Personalización (Control)")]
    //public Renderer racketRenderer; // Asigna aquí el Mesh Renderer para cambiar el color

    private Vector3 previousPosition;
    private Vector3 racketVelocity;

    public Rigidbody racquetRb;
    public Transform controllerTransform;

    void FixedUpdate()
    {
        // Mover la raqueta hacia el controlador respetando la física
        racquetRb.MovePosition(controllerTransform.position);
        racquetRb.MoveRotation(controllerTransform.rotation);
    }

    void Update()
    {
        // Calcular la velocidad de la raqueta entre fotogramas
        racketVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        /* Verificar si colisiona con la pelota
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                // Transferir la velocidad de la raqueta multiplicada
                Vector3 force = racketVelocity * hitPowerMultiplier;
                ballRb.AddForce(force, ForceMode.Impulse);
            }
        }*/
    }
}