using Events;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
    [Header("Main Menu Object")]
    public GameObject mainMenu;
    
    [Header("Pause Menu Object")]
    public GameObject pauseMenu;
    
    [Header("Individual Tutorial Pages")]
    public GameObject overviewPage;
    public GameObject basicPage;
    public GameObject combatPage;
    public GameObject farmingPage;
    public GameObject quickEquipPage;
    
    private void OnEnable()
    {
        ShowOnly(overviewPage);
    }
    
    public void ShowOnly(GameObject target)
    {
        // Hide everything first
        overviewPage.SetActive(false);
        basicPage.SetActive(false);
        combatPage.SetActive(false);
        farmingPage.SetActive(false);
        quickEquipPage.SetActive(false);

        // Then show the one requested
        target.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        this.gameObject.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void BackToMainMenu()
    {
        this.gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
