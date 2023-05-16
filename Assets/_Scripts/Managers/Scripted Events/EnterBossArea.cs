using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid {
    public class EnterBossArea : MonoBehaviour
    {
        public BoxCollider2D TriggerEnterArea;
        public GameObject ActivateBoss, ActivateWalls;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                ActivateBoss.SetActive(true);
                ActivateWalls.SetActive(true);
                TriggerEnterArea.enabled = false;
            }
        }

        private void Update() {
            if (ActivateBoss == null) {
                ActivateWalls.SetActive(false);
            }
        }
    }
}
