using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Chobitech.Unity;

public class FadeController : MonoBehaviour
{
    public UnityEvent<float> onFadeRateChanged;

    private Coroutine fadeCoroutine;

    public bool IsFading => fadeCoroutine != null;

    public void StopFading()
    {
        if (IsFading)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private IEnumerator InnerFadeProcess(bool isFadeIn, float durationSec, UnityAction onFinished)
    {
        if (onFadeRateChanged == null)
        {
            yield break;
        }

        float startRate, endRate;

        if (isFadeIn)
        {
            startRate = 0f;
            endRate = 1f;
        }
        else
        {
            startRate = 1f;
            endRate = 0f;
        }

        if (durationSec <= 0f)
        {
            onFadeRateChanged?.Invoke(endRate);
            onFinished?.Invoke();
            yield break;
        }

        onFadeRateChanged.Invoke(startRate);

        var ct = 0f;
        while (ct < durationSec)
        {
            onFadeRateChanged.Invoke(Mathf.Lerp(startRate, endRate, ct / durationSec));
            ct += Time.deltaTime;
            yield return null;
        }

        onFadeRateChanged.Invoke(endRate);

        onFinished?.Invoke();
    }

    public IEnumerator FadeInOnlyProcess(float durationSec, UnityAction onFinished = null)
    {
        yield return InnerFadeProcess(true, durationSec, onFinished);
    }
    public IEnumerator FadeOutOnlyProcess(float durationSec, UnityAction onFinished = null)
    {
        yield return InnerFadeProcess(false, durationSec, onFinished);
    }

    private void FadeInOrOutOnly(bool isFadeIn, float durationSec, bool restartIfFading = false, UnityAction onFinished = null)
    {
        if (onFadeRateChanged == null || (!restartIfFading && IsFading))
        {
            return;
        }

        StopFading();

        fadeCoroutine = StartCoroutine(InnerFadeProcess(isFadeIn, durationSec, () =>
        {
            fadeCoroutine = null;
            onFinished?.Invoke();
        }));
    }

    public void StartFadeInOnly(float durationSec, bool restartIfFading = false, UnityAction onFinished = null) => FadeInOrOutOnly(true, durationSec, restartIfFading, onFinished);

    public void StartFadeOutOnly(float durationSec, bool restartIfFading = false, UnityAction onFinished = null) => FadeInOrOutOnly(false, durationSec, restartIfFading, onFinished);


    public IEnumerator FadeInAndOutProcess(float fadeInDurationSec, float fadeOutDurationSec, IEnumerator inFadingProcess = null, UnityAction onFinished = null)
    {
        if (onFadeRateChanged == null)
        {
            yield break;
        }

        yield return InnerFadeProcess(true, fadeInDurationSec, null);
        yield return inFadingProcess;
        yield return InnerFadeProcess(false, fadeOutDurationSec, null);

        fadeCoroutine = null;
        onFinished?.Invoke();
    }


    public void StartFadeInAndOut(float fadeInDurationSec, float fadeOutDurationSec, bool restartIfFading = false, IEnumerator inFadingProcess = null, UnityAction onFinished = null)
    {
        if (onFadeRateChanged == null || (!restartIfFading && IsFading))
        {
            return;
        }

        StopFading();

        fadeCoroutine = StartCoroutine(FadeInAndOutProcess(fadeInDurationSec, fadeOutDurationSec, inFadingProcess, onFinished));
    }

    public void StartFadeInAndOut(
        float fadeInDurationSec,
        float fadeOutDurationSec,
        bool restartIfFading = false,
        UnityAction inFadingAction = null,
        UnityAction onFinished = null) => StartFadeInAndOut(fadeInDurationSec, fadeOutDurationSec, restartIfFading, inFadingAction?.ToIEnumerator(), onFinished);



}
