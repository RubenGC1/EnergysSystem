using UnityEngine;
using UnityEngine.Video;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class AudioArea : MonoBehaviour
{
    [Header("Trigger del área")]
    [SerializeField] private Collider triggerCollider;        // Déjalo vacío y se auto-asigna
    [SerializeField] private bool autoMakeTrigger = true;     // Forzar IsTrigger = true
    [SerializeField] private bool ensureKinematicRB = true;   // Asegurar Rigidbody kinematic

    [Header("Detección del jugador")]
    [Tooltip("Se aceptará cualquier collider cuyo rig contenga una Camera con este tag.")]
    [SerializeField] private string requiredCameraTag = "MainCamera";

    [Header("Control multimedia")]
    [SerializeField] private AudioSource audioSource;         // Opcional: si no hay, no controla audio
    [SerializeField] private VideoPlayer videoPlayer;         // Opcional: si no hay, no controla video

    private int insideCount = 0;
    private bool audioHasStarted = false;
    private bool videoHasStarted = false;

    private void Awake()
    {
        // Trigger del área
        if (!triggerCollider) triggerCollider = GetComponent<Collider>();
        if (!triggerCollider)
        {
            Debug.LogError("[AudioArea] No hay Collider en el objeto del área. Añade uno (Box/Sphere/etc).");
            enabled = false;
            return;
        }

        if (autoMakeTrigger && !triggerCollider.isTrigger)
            triggerCollider.isTrigger = true;

        if (ensureKinematicRB)
        {
            var rb = triggerCollider.GetComponent<Rigidbody>();
            if (!rb) rb = triggerCollider.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Audio settings (no reproducir aún)
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.loop = true; // si quieres loop
            // No tocamos volume; usaremos Pause/UnPause
        }

        // Video settings (no reproducir aún)
        if (!videoPlayer) videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.isLooping = true; // quítalo si no quieres loop
            // Evitamos autoPlay fuera del área
            videoPlayer.playOnAwake = false;
        }
    }

    private bool IsValidActivator(Collider other)
    {
        // 1) Si el collider tiene el tag directamente
        if (!string.IsNullOrEmpty(requiredCameraTag) && other.CompareTag(requiredCameraTag))
            return true;

        // 2) Buscar una Camera con ese tag en la jerarquía del rig que entra
        Camera cam = other.GetComponentInChildren<Camera>();
        if (!cam && other.transform.root) cam = other.transform.root.GetComponentInChildren<Camera>();

        return cam && cam.CompareTag(requiredCameraTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidActivator(other)) return;

        insideCount++;
        if (insideCount == 1)
        {
            // AUDIO: reanudar o empezar
            if (audioSource)
            {
                if (audioHasStarted)
                {
                    // Si ya se inició antes, reanudar
                    audioSource.UnPause();
                }
                else
                {
                    audioSource.Play();
                    audioHasStarted = true;
                }
            }

            // VIDEO: reanudar o empezar
            if (videoPlayer)
            {
                if (videoHasStarted)
                {
                    // Si ya se inició antes, reanudar
                    videoPlayer.Play(); // VideoPlayer.Play() reanuda desde donde quedó tras Pause()
                }
                else
                {
                    videoPlayer.Play();
                    videoHasStarted = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidActivator(other)) return;

        insideCount = Mathf.Max(0, insideCount - 1);
        if (insideCount == 0)
        {
            // AUDIO: pausar (no bajar volumen)
            if (audioSource)
            {
                audioSource.Pause();
            }

            // VIDEO: pausar
            if (videoPlayer)
            {
                videoPlayer.Pause();
            }
        }
    }
}
