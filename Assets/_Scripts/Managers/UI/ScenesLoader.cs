using UnityEngine;
using UnityEngine.SceneManagement;

namespace rene_roid {
    public class ScenesLoader : MonoBehaviour
    {
        public void LoadScene(int sceneIndex) => SceneManager.LoadScene(sceneIndex);
        public void QuitGame() => Application.Quit();
        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
