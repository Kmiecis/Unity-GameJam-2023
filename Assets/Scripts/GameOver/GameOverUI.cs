using Common.Coroutines;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class GameOverUI : MonoBehaviour
    {
        public Image backgroundImage;
        public Button backgroundButton;
        public TextMeshProUGUI upperTtext;
        public TextMeshProUGUI lowerText;
        public TextMeshProUGUI tipText;
        public Image overlayImage;
        [Header("Settings")]
        public float fadeInDuration = 1.0f;
        public float fadeOutDuration = 1.0f;
        public float delayDuration = 1.0f;

        private void FadeIn()
        {
            void Finish()
            {
                backgroundButton.onClick.AddListener(OnClick);
            }
            
            backgroundImage.CoFade(0.5f, fadeInDuration)
                .With(upperTtext.CoFade(1.0f, fadeInDuration))
                .With(lowerText.CoFade(1.0f, fadeInDuration))
                .Then(Finish)
                .WaitRealtime(delayDuration)
                .Then(tipText.CoFade(1.0f, fadeInDuration))
                .Start(this);
        }

        private void FadeOut()
        {
            overlayImage.CoFade(1.0f, fadeOutDuration)
                .With(FindObjectOfType<SoundsManager>().FadeVolume(0.0f, fadeOutDuration))
                .Then(LoadSameScene)
                .Start(this);
        }
        
        private void OnClick()
        {
            FadeOut();
        }

        private void LoadSameScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void Awake()
        {
            backgroundImage.SetAlpha(0.0f);
            upperTtext.SetAlpha(0.0f);
            lowerText.SetAlpha(0.0f);
            tipText.SetAlpha(0.0f);
            overlayImage.SetAlpha(0.0f);
        }

        private void Start()
        {
            FadeIn();
        }
    }
}