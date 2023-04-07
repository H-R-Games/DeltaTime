using UnityEngine;
using rene_roid_player;

namespace rene_roid {    
    public class Death : MonoBehaviour
    {
        [SerializeField] private GameObject _deathCanvas;
        void Start()
        {
            
        }
        
        void Update()
        {
            
        }

        public void OnDeath() {
            _deathCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
