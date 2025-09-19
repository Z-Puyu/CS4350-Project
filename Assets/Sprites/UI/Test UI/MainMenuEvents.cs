using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _button;
    private AudioSource _audio;

    private List<Button> _menuButtons = new List<Button>();

    private void Awake()
    {
        _document = GetComponent<UIDocument>();
        _audio = GetComponent<AudioSource>();
        _button = _document.rootVisualElement.Q<Button>("StartButton");
        _button.RegisterCallback<ClickEvent>(OnPlayGameClick);
        
        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        foreach (var btn in _menuButtons)
        {
            btn.RegisterCallback<ClickEvent>(OnAllButtonClick);
        }
    }

    private void OnDisable()
    {
        _button.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        foreach (var btn in _menuButtons)
        {
            btn.UnregisterCallback<ClickEvent>(OnAllButtonClick);
        }
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("Pressed Start Button !");
    }

    private void OnAllButtonClick(ClickEvent evt)
    {
        _audio.Play();
    }
}
