using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Animations;

[DisallowMultipleComponent]
public class UIPlayClipOnClick : MonoBehaviour
{
    [Header("Asignación en Inspector")]
    [SerializeField] private Button button;              // Botón UI que dispara la animación
    [SerializeField] private Animator targetAnimator;    // Animator del objeto a animar (no necesita Controller)
    [SerializeField] private AnimationClip clip;         // Clip a reproducir

    [Header("Opciones")]
    [Tooltip("Si la animación ya está en curso: true=reinicia, false=ignora el clic.")]
    [SerializeField] private bool restartIfPlaying = true;
    [Tooltip("Velocidad de reproducción (1 = normal).")]
    [SerializeField] private float playbackSpeed = 1f;
    [Tooltip("Restaurar el estado del Animator al terminar (útil si ese objeto usa Controller en otras partes).")]
    [SerializeField] private bool restoreAnimatorState = true;

    [Header("Eventos")]
    public UnityEvent onStarted;
    public UnityEvent onCompleted;

    private PlayableGraph graph;
    private AnimationClipPlayable playable;
    private bool isPlaying;

    private void Reset()
    {
        if (button == null) button = GetComponent<Button>();
        if (targetAnimator == null) targetAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(PlayOnce);
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayOnce);

        StopAndCleanup();
    }

    public void PlayOnce()
    {
        if (clip == null || targetAnimator == null)
        {
            Debug.LogWarning("[UIPlayClipOnClick] Falta asignar Button/Animator/Clip.");
            return;
        }

        if (isPlaying && !restartIfPlaying)
            return;

        // Si estaba sonando y queremos reiniciar, limpiamos primero
        if (isPlaying && restartIfPlaying)
            StopAndCleanup();

        // Construir el graph de Playables
        graph = PlayableGraph.Create("UIPlayClipGraph");
        var output = AnimationPlayableOutput.Create(graph, "AnimOutput", targetAnimator);

        playable = AnimationClipPlayable.Create(graph, clip);
        playable.SetApplyFootIK(false);
        playable.SetApplyPlayableIK(false);
        playable.SetTime(0);
        playable.SetSpeed(Mathf.Approximately(playbackSpeed, 0f) ? 1f : playbackSpeed);
        output.SetSourcePlayable(playable);

        // Reproducir
        graph.Play();
        isPlaying = true;
        onStarted?.Invoke();

        // Finalizar al acabar la duración real del clip
        float duration = Mathf.Max(0.01f, clip.length / Mathf.Abs(playbackSpeed));
        Invoke(nameof(Finish), duration);
    }

    private void Finish()
    {
        StopAndCleanup();

        if (restoreAnimatorState && targetAnimator != null)
        {
            // Rebind para devolver al estado base/controlador
            targetAnimator.Rebind();
            targetAnimator.Update(0f);
        }

        onCompleted?.Invoke();
    }

    private void StopAndCleanup()
    {
        if (IsInvoking(nameof(Finish))) CancelInvoke(nameof(Finish));
        if (graph.IsValid())
        {
            graph.Stop();
            graph.Destroy();
        }
        isPlaying = false;
    }
}
