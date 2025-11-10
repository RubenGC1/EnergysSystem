using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class WelcomeBanner : MonoBehaviour
{
    public float fadeIn = 0.3f;
    public float hold = 2.0f;
    public float fadeOut = 0.4f;

    CanvasGroup cg;

    IEnumerator Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        gameObject.SetActive(true);

        yield return FadeTo(1f, fadeIn);
        yield return new WaitForSecondsRealtime(hold);
        yield return FadeTo(0f, fadeOut);

        gameObject.SetActive(false); // ocultar al terminar
    }

    IEnumerator FadeTo(float target, float dur)
    {
        if (dur <= 0f) { cg.alpha = target; yield break; }
        float start = cg.alpha, t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        cg.alpha = target;
    }
}
