using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Parkour.Menu
{
    public sealed class MenuPrincipalController : MonoBehaviour
    {
        public const string VolumeKey = "Parkour.Menu.Volume";
        public const string MusicKey = "Parkour.Menu.Music";
        public const string FullscreenKey = "Parkour.Menu.Fullscreen";

        [SerializeField] private string levelScene = "Assets/Scenes/SampleScene.unity";
        [SerializeField] private AudioSource music;
        [SerializeField] private AudioSource effects;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private CanvasGroup mainButtons;
        [SerializeField] private GameObject levelsPanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject creditsPanel;
        [SerializeField] private GameObject exitPanel;
        [SerializeField] private GameObject messagePanel;
        [SerializeField] private Text message;
        [SerializeField] private Slider volume;
        [SerializeField] private Toggle musicEnabled;
        [SerializeField] private Toggle fullscreen;
        [SerializeField] private Button playButton;

        private GameObject activePanel;
        private GameObject previousSelection;
        private bool loading;

        public bool IsLoading => loading;
        public string LevelScene => levelScene;

        private void Start()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            volume.SetValueWithoutNotify(PlayerPrefs.GetFloat(VolumeKey, 0.65f));
            musicEnabled.SetIsOnWithoutNotify(PlayerPrefs.GetInt(MusicKey, 1) == 1);
            fullscreen.SetIsOnWithoutNotify(PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1);
            ApplyAudio();
            if (PlayerPrefs.HasKey(FullscreenKey)) Screen.fullScreen = fullscreen.isOn;
            if (music.clip != null) music.Play();
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame || loading) return;
            if (activePanel != null) ClosePanel();
            else ShowExit();
        }

        public void ShowLevels() => ShowPanel(levelsPanel);
        public void ShowOptions() => ShowPanel(optionsPanel);
        public void ShowCredits() => ShowPanel(creditsPanel);
        public void ShowExit() => ShowPanel(exitPanel);

        private void ShowPanel(GameObject panel)
        {
            if (loading) return;
            PlayClick();
            if (activePanel == null) previousSelection = EventSystem.current.currentSelectedGameObject;
            else activePanel.SetActive(false);
            mainButtons.interactable = false;
            mainButtons.blocksRaycasts = false;
            activePanel = panel;
            panel.SetActive(true);
            var first = panel.GetComponentInChildren<Selectable>();
            EventSystem.current.SetSelectedGameObject(first != null ? first.gameObject : null);
        }

        public void ClosePanel()
        {
            if (loading) return;
            PlayClick();
            if (activePanel != null) activePanel.SetActive(false);
            activePanel = null;
            mainButtons.interactable = true;
            mainButtons.blocksRaycasts = true;
            EventSystem.current.SetSelectedGameObject(previousSelection != null ? previousSelection : playButton.gameObject);
        }

        public void SetVolume(float value)
        {
            PlayerPrefs.SetFloat(VolumeKey, Mathf.Clamp01(value));
            ApplyAudio();
        }

        public void SetMusic(bool value)
        {
            PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
            ApplyAudio();
        }

        public void SetFullscreen(bool value)
        {
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            Screen.fullScreen = value;
        }

        private void ApplyAudio()
        {
            music.volume = volume.value;
            music.mute = !musicEnabled.isOn;
            effects.volume = volume.value;
        }

        public void PlayHover()
        {
            if (hoverSound != null && effects != null) effects.PlayOneShot(hoverSound, 0.35f);
        }

        private void PlayClick()
        {
            if (clickSound != null && effects != null) effects.PlayOneShot(clickSound, 0.5f);
        }

        public void Play()
        {
            if (loading) return;
            if (!Application.CanStreamedLevelBeLoaded(levelScene))
            {
                ShowPanel(messagePanel);
                message.text = "El nivel no está incluido en esta compilación. Añade SampleScene al perfil de compilación.";
                return;
            }
            PlayClick();
            PlayerPrefs.Save();
            loading = true;
            mainButtons.interactable = false;
            mainButtons.blocksRaycasts = false;
            if (activePanel != null) activePanel.SetActive(false);
            activePanel = messagePanel;
            messagePanel.SetActive(true);
            foreach (var selectable in messagePanel.GetComponentsInChildren<Selectable>()) selectable.interactable = false;
            EventSystem.current.SetSelectedGameObject(null);
            StartCoroutine(LoadLevel());
        }

        private IEnumerator LoadLevel()
        {
            message.text = "Cargando…";
            var operation = SceneManager.LoadSceneAsync(levelScene);
            while (!operation.isDone)
            {
                message.text = "Cargando… " + Mathf.RoundToInt(Mathf.Clamp01(operation.progress / 0.9f) * 100f) + "%";
                yield return null;
            }
        }

        public void Quit()
        {
            PlayerPrefs.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationPause(bool paused)
        {
            if (paused) PlayerPrefs.Save();
        }

        private void OnDisable() => PlayerPrefs.Save();
    }
}
