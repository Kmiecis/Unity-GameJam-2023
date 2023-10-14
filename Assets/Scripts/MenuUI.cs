using Common.Coroutines;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class MenuUI : MonoBehaviour
    {
        private const int NEXT_SCENE_INDEX = 1;

        public Image ueBackground;
        public Image introBackground;
        public TextMeshProUGUI startText;
        public Button startButton;
        public Sound music;

        [Header("Settings")]
        public float introFadeInDuration = 1.0f;
        public float betweenDelay = 2.0f;
        public float startTextFadeInDuration = 1.0f;
        public float startTextBlinkDuration = 2.0f;
        public float fadeOutDuration = 1.0f;

        private SoundsManager _soundsManager;
        private bool _ready;

        private void FadeIn()
        {
            UCoroutine.Yield()
                .Then(ueBackground.CoFade(1.0f, introFadeInDuration))
                .WaitRealtime(betweenDelay)
                .Then(ueBackground.CoFade(0.0f, introFadeInDuration))
                .Then(() => _soundsManager.PlaySound(music))
                .Then(introBackground.CoFade(1.0f, introFadeInDuration))
                .WaitRealtime(betweenDelay)
                .Then(startText.CoFade(1.0f, startTextFadeInDuration))
                .Then(Finish)
                .Start(this);
        }

        private void Blink()
        {
            startText.transform.CoLocalScale(1.1f, startTextBlinkDuration)
                .Then(startText.transform.CoLocalScale(1.0f, startTextBlinkDuration))
                .Then(Blink)
                .Start(this);
        }

        private void FadeOut()
        {
            UCoroutine.Yield()
                .Then(
                    introBackground.CoFade(0.0f, fadeOutDuration),
                    startText.CoFade(0.0f, fadeOutDuration),
                    _soundsManager.FadeVolume(0.0f, fadeOutDuration)
                )
                .Then(LoadNextScene)
                .Start(this);
        }

        private void Finish()
        {
            startButton.onClick.AddListener(OnClickStart);
            
            Blink();
            
            _ready = true;
        }

        private void OnClickStart()
        {
            FadeOut();
        }

        private void LoadNextScene()
        {
            SceneManager.LoadScene(NEXT_SCENE_INDEX);
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickStart();
            }
        }
        
        private void Awake()
        {
            _soundsManager = FindObjectOfType<SoundsManager>();
            
            ueBackground.SetAlpha(0.0f);
            introBackground.SetAlpha(0.0f);
            startText.SetAlpha(0.0f);
        }

        private void Start()
        {
            FadeIn();
        }

        private void Update()
        {
            if (_ready)
            {
                HandleInput();
            }
        }
    }
}