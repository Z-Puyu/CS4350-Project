using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Containers")]
    public GameObject tutorialContainer;

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void QuitApplication()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }

    public void OpenTutorial()
    {
        this.gameObject.SetActive(false);
        tutorialContainer.SetActive(true);
    }
}