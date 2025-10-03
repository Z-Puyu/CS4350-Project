using UnityEngine;
using UnityEngine.UIElements;

namespace Sprites.UI.Test_UI
{
    public class MainMenuEvents : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset mainMenuAsset;
        [SerializeField] private VisualTreeAsset guidePanelAsset;
        [SerializeField] private AudioSource buttonAudio;

        private UIDocument _uiDocument;
        private VisualElement _root;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            // Show main menu by default
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            _root.Clear();
            var mainMenu = mainMenuAsset.CloneTree();
            _root.Add(mainMenu);

            var startButton = mainMenu.Q<Button>("StartButton");
            var guideButton = mainMenu.Q<Button>("GuideButton");
            var quitButton  = mainMenu.Q<Button>("QuitButton");

            startButton.clicked += () => Debug.Log("Start Game");
            guideButton.clicked += ShowGuidePanel;
            quitButton.clicked  += () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            };

            // optional: play sound for all buttons
            foreach (var btn in mainMenu.Query<Button>().ToList())
                btn.clicked += () => buttonAudio?.Play();
        }

        private void ShowGuidePanel()
        {
            _root.Clear();
            var guidePanel = guidePanelAsset.CloneTree();
            _root.Add(guidePanel);

            var closeButton = guidePanel.Q<Button>("CloseGuideButton");
            closeButton.clicked += ShowMainMenu;

            // optional: play sound for all buttons
            foreach (var btn in guidePanel.Query<Button>().ToList())
                btn.clicked += () => buttonAudio?.Play();
        }
    }
}