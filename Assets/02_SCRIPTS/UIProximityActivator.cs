using UnityEngine;
using System.Collections;

public class UIProximityActivator : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Asigna tu bot�n aqu� (o el panel contenedor del bot�n)
    public float fadeDuration = 0.5f; // Duraci�n de la animaci�n
    public float targetAlphaVisible = 1f;
    public float targetAlphaHidden = 0f;

    private Coroutine currentFade;
    private bool isPlayerInside = false;

    private void Start()
    {
        SetCanvasGroupAlpha(0f); // Inicialmente oculto
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isPlayerInside = true;
            StartFade(targetAlphaVisible);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            isPlayerInside = false;
            StartFade(targetAlphaHidden);
        }
    }

    void StartFade(float targetAlpha)
    {
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeCanvasGroup(targetAlpha));
    }

    IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Si est� oculto, desactiva interacci�n
        canvasGroup.interactable = targetAlpha > 0.9f;
        canvasGroup.blocksRaycasts = targetAlpha > 0.9f;
    }

    void SetCanvasGroupAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        canvasGroup.interactable = alpha > 0.9f;
        canvasGroup.blocksRaycasts = alpha > 0.9f;
    }
}
