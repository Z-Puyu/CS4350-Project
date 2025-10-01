using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sprites.UI.Test_UI {
    public class MainMenuEvents : MonoBehaviour
    {
        private UIDocument _document;
        private Button _button;
        private AudioSource _audio;

        private List<Button> _menuButtons = new List<Button>();

        private void Awake()
        {
            this._document = this.GetComponent<UIDocument>();
            this._audio = this.GetComponent<AudioSource>();
            this._button = this._document.rootVisualElement.Q<Button>("StartButton");
            this._button.RegisterCallback<ClickEvent>(this.OnPlayGameClick);
        
            this._menuButtons = this._document.rootVisualElement.Query<Button>().ToList();
            foreach (var btn in this._menuButtons)
            {
                btn.RegisterCallback<ClickEvent>(this.OnAllButtonClick);
            }
        }

        private void OnDisable()
        {
            this._button.UnregisterCallback<ClickEvent>(this.OnPlayGameClick);
            foreach (var btn in this._menuButtons)
            {
                btn.UnregisterCallback<ClickEvent>(this.OnAllButtonClick);
            }
        }

        private void OnPlayGameClick(ClickEvent evt)
        {
            Debug.Log("Pressed Start Button !");
        }

        private void OnAllButtonClick(ClickEvent evt)
        {
            this._audio.Play();
        }
    }
}
