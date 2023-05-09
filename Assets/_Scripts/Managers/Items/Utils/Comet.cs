using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_enemy;

public class Comet : MonoBehaviour
{
    [Header("Comet Settings")]
    [SerializeField] private float _speed = 70f;
    [SerializeField] private float _lifeTime = 2f;
    public Vector3 targetPosition;
    public Vector3 direction;
    public float damage;
    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, _lifeTime);
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
            other.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }
}
