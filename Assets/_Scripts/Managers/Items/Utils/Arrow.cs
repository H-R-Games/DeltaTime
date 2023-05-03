using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_enemy;


public class Arrow : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _lifeTime = 5f;
    public Vector3 targetPosition;
    public Vector3 direction;
    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, _lifeTime);
        if (direction.x > 0) this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        else this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
    }

    void Update()
    {
        _rb.velocity = direction * _speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si la flecha colisiona con un enemigo, le hacemos daño
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyBase>().TakeDamage(_damage);
        }
    }
}
