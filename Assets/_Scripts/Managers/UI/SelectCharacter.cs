using UnityEngine;
using UnityEngine.SceneManagement;

namespace rene_roid {
    [System.Serializable]
    public enum Characters
    {
        None,
        Namka,
        Musashi,
    }

    public class SelectCharacter : PersistentSingleton<SelectCharacter>
    {
        public void LoadScene(int sceneIndex) {
            if (_character == Characters.None) return;
            SceneManager.LoadScene(sceneIndex);
        }
        public void QuitGame() => Application.Quit();
        public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Detect when a scene is loaded
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene loaded: " + scene.name);
            Debug.Log(mode);

            if (scene.name == "Gameflow") LoadCharacter();
        }

        [SerializeField] private Characters _character;

        public Characters Character
        {
            get => _character;
            set => _character = value;
        }

        public void SetCharacter(int character)
        {
            Character = (Characters) character;
        }

        [SerializeField] private GameObject[] _characters;
        public void LoadCharacter()
        {
            switch (Character)
            {
                case Characters.Namka:
                    var n = Instantiate(_characters[0]);
                    break;
                case Characters.Musashi:
                    var m = Instantiate(_characters[1]);
                    break;
                default:
                    Debug.Log("No character selected");
                    break;
            }
        }

    }
}
