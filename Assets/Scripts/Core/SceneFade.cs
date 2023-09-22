using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SceneFade : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeRoutine(float TimeToFade, float Target)
        {
            while (!Mathf.Approximately(canvasGroup.alpha,Target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, Target, Time.deltaTime/TimeToFade);
                yield return null;
            }
        }

        public void InstantFadeOut()
        {
            canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float TimeToFade)
        {
            return Fade(TimeToFade, 1f);
        }

        public Coroutine FadeIn(float TimeToFade)
        {
            return Fade(TimeToFade, 0f);
        }

        public Coroutine Fade(float TimeToFade, float Target)
        {
            if (currentActiveFade != null) StopCoroutine(currentActiveFade);

            currentActiveFade = StartCoroutine(FadeRoutine(TimeToFade, Target));
            return currentActiveFade;
        }
    }
}