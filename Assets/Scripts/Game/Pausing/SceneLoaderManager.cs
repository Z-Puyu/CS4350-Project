using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Pausing
{
    public class SceneLoaderManager : MonoBehaviour
    {

        public void LoadScene(Component component, object sn)
        { 
            string sceneName = (string) ((object[]) sn)[0];
            SceneManager.LoadScene(sceneName);
        }
    }
}
