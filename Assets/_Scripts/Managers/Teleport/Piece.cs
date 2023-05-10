using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hrTeleport
{
    public class Piece : MonoBehaviour
    {
        [Header("Piece")]
        private Vector2 _initPos;
        Collider2D _collider;

        void Update()
        {
            ItemFloat();
        }

        private void ItemFloat()
        {
            if (_initPos == Vector2.zero) _initPos = transform.position;

            transform.position = new Vector2(_initPos.x, _initPos.y + Mathf.Sin(Time.time * 2f) * 0.1f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                Destroy(gameObject);
                TeleportManager._piecesActivated++;
            } 
        }
    }
}