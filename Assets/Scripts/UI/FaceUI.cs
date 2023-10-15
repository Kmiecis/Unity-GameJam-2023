using System.Collections;
using Common.Coroutines;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class FaceUI : MonoBehaviour
    {
        private const int NEXT_SCENE_INDEX = 2;
        
        public Image faceImage;
        public RectTransform faceTransform;
        public TextMeshProUGUI quoteText;
        public TextMeshProUGUI authorText;
        public Image fillImage;
        public TextMeshProUGUI fillText;
        
        [Header("Settings")]
        public float fadeInDuration = 1.0f;
        public float floatDuration = 1.0f;
        public float fadeOutDuration = 1.0f;
        public float fillDuration = 2.0f;
        public float faceScale = 1.2f;

        private float _filled = 0.0f;
        private bool _finishing;

        private void Run()
        {
            faceImage.CoFade(1.0f, fadeInDuration)
                .With(quoteText.CoFade(1.0f, fadeInDuration))
                .With(authorText.CoFade(1.0f, fadeInDuration))
                .Then(faceTransform.CoLocalScale(faceScale, floatDuration)
                    //faceTransform.CoAnchorMin(new Vector2(-0.05f, 0.0f), floatDuration),
                    //faceTransform.CoAnchorMax(Vector2.one, floatDuration)
                )
                .Then(() => _finishing = true)
                .Then(
                    faceImage.CoFade(0.0f, fadeOutDuration),
                    quoteText.CoFade(0.0f, fadeOutDuration),
                    authorText.CoFade(0.0f, fadeOutDuration),
                    FindObjectOfType<SoundsManager>().FadeVolume(0.0f, fadeOutDuration)
                )
                .Then(LoadNextScene)
                .Start(this);
        }

        private IEnumerator FadeText(string text, float duration)
        {
            var timer = UCoroutine.YieldTimeNormalized(duration);
            var splits = text.Split(';');

            var index = 0;
            quoteText.text = splits[index];
            
            while (timer.MoveNext())
            {
                var time = timer.Current;
                
                var current = Mathf.RoundToInt((splits.Length - 1) * (time + 0.2f));
                if (index != current)
                {
                    index = current;
                    
                    quoteText.text += splits[index];
                }
                yield return null;
            }

            quoteText.text = text;
        }

        private void LoadNextScene()
        {
            SceneManager.LoadScene(NEXT_SCENE_INDEX);
        }

        private void UpdateInput()
        {
            var dt = Time.deltaTime;
            var fill = fillDuration * dt;

            bool wasFilling = !Mathf.Approximately(_filled, 0.0f);
            
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Mouse0))
            {
                _filled = Mathf.Min(_filled + fill, 1.0f);
            }
            else
            {
                _filled = Mathf.Max(_filled - fill, 0.0f);
            }

            if (Mathf.Approximately(_filled, 1.0f))
            {
                StopAllCoroutines();

                _finishing = true;
                UCoroutine.Yield()
                    .Then(
                        faceImage.CoFade(0.0f, fadeOutDuration),
                        quoteText.CoFade(0.0f, fadeOutDuration),
                        authorText.CoFade(0.0f, fadeOutDuration),
                        fillText.CoFade(0.0f, fadeInDuration),
                        fillImage.CoFade(0.0f, fadeOutDuration),
                        FindObjectOfType<SoundsManager>().FadeVolume(0.0f, fadeOutDuration)
                    )
                    .Then(LoadNextScene)
                    .Start(this);
            }
            else if (Mathf.Approximately(_filled, 0.0f))
            {
                if (wasFilling)
                {
                    fillText.CoFade(0.0f, 0.15f).Start(this);
                }
            }
            else
            {
                if (!wasFilling)
                {
                    fillText.CoFade(1.0f, 0.15f).Start(this);
                }
            }

            fillImage.fillAmount = _filled;
        }

        private void Awake()
        {
            faceTransform.anchorMin = Vector2.zero;
            faceTransform.anchorMax = new Vector2(1.05f, 1.00f);
            faceImage.SetAlpha(0.0f);
            quoteText.SetAlpha(0.0f);
            authorText.SetAlpha(0.0f);
            fillText.SetAlpha(0.0f);
        }

        private void Start()
        {
            Run();
        }

        private void Update()
        {
            if (!_finishing)
            {
                UpdateInput();
            }
        }
    }
}