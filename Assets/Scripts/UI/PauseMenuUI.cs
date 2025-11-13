using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Containers")]
    public GameObject tutorialContainer;
    
    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    
    public void OpenTutorial()
    {
        this.gameObject.SetActive(false);
        tutorialContainer.SetActive(true);
    }
}
