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
        public TextMeshProUGUI text;
        public Image overlayImage;
        [Header("Settings")]
        public float fadeInDuration = 1.0f;
        public float fadeOutDuration = 1.0f;

        private void FadeIn()
        {
            void Finish()
            {
                backgroundButton.onClick.AddListener(OnClick);
            }
            
            backgroundImage.CoFade(0.5f, fadeInDuration)
                .With(text.CoFade(1.0f, fadeInDuration))
                .Then(Finish)
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
            text.SetAlpha(0.0f);
            overlayImage.SetAlpha(0.0f);
        }

        private void Start()
        {
            FadeIn();
        }
    }
}