using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Spatial/Utility/Rotate Object")]
public class RotateObject : MonoBehaviour
{
    [Header("Velocidad (grados/seg)")]
    [Tooltip("Grados por segundo en cada eje (X, Y, Z).")]
    public Vector3 rotationSpeed = new Vector3(0f, 50f, 0f);

    [Header("Opciones")]
    [Tooltip("Si es true, rota en el espacio local del objeto; si es false, en espacio mundial.")]
    public bool useLocalSpace = true;

    [Tooltip("Usar Time.unscaledDeltaTime (ignora escalado de tiempo).")]
    public bool unscaledTime = false;

    [Tooltip("Retraso inicial antes de empezar a rotar (segundos).")]
    public float startDelay = 0f;

    float _startTime;

    void OnEnable()
    {
        _startTime = Time.time;
    }

    void Update()
    {
        // Espera el retraso inicial (si lo hay)
        if (startDelay > 0f && Time.time - _startTime < startDelay)
            return;

        float dt = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        if (useLocalSpace)
            transform.Rotate(rotationSpeed * dt, Space.Self);
        else
            transform.Rotate(rotationSpeed * dt, Space.World);
    }
}
