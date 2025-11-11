using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Containers")]
    public GameObject mainContainer;
    public GameObject settingsContainer;

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitApplication()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }

    public void OpenSettings()
    {
        mainContainer.SetActive(false);
        settingsContainer.SetActive(true);
    }

    public void BackToMain()
    {
        settingsContainer.SetActive(false);
        mainContainer.SetActive(true);
    }
}