using Events;
using UnityEngine;

namespace Game.Pausing
{
    public class PausingManager : MonoBehaviour
    {
        [SerializeField] private CrossObjectEventWithDataSO restartLevel;
        [SerializeField] private string gameMaoSceneName;
        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void RestartGame()
        {
            restartLevel.TriggerEvent(this, gameMaoSceneName);
        }
        
    }
}
