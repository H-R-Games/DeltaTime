using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;
using rene_roid;

namespace rene_roid_enemy
{
    public class ProyectilContreoller : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 3f;
        [SerializeField] private LayerMask _playerLayer;
        CircleCollider2D _collider;
        float _damage = 1f;
        Vector2 _direction;

        void Start()
        {
            _collider = GetComponent<CircleCollider2D>();
            Destroy(gameObject, _lifeTime);
        }

        void FixedUpdate()
        {
            transform.Translate(new Vector2(_direction.x, 0) * _speed * Time.deltaTime);
            // hacemos que el hijo rote para darle un efecto de rotacion de movimiento
            transform.GetChild(0).Rotate(new Vector3(0, 0, 1) * 1000 * Time.deltaTime);

            var player = Physics2D.OverlapCircle(transform.position, _collider.radius, _playerLayer);

            if (player != null)
            {
                player.GetComponent<PlayerBase>().TakeDamage(_damage);
                Destroy(gameObject);
            }
        }

        public void SetValues(float damage, Vector2 direction)
        {
            _damage = damage;
            _direction = direction;
        }
    }
}