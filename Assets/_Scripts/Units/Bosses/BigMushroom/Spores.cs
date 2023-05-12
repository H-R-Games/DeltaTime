using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;
using rene_roid_enemy;

namespace rene_roid_enemy
{
    public class Spores : EnemyBase
    {
        [Header("Spores Settings")]
        [SerializeField][Range(0, 100f)] private float _sporesDistance = 1f;
        [SerializeField][Range(0, 100f)] private float _sporesLifeTime = 1f;
        float _timeCurrenLife = 0f;
        float t = 1f;

        void Update()
        {
            _timeCurrenLife += Time.deltaTime;
            if (_timeCurrenLife >= _sporesLifeTime) Destroy(gameObject);

            if (t >= 2f)
            {
                if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _sporesDistance) _targetPlayer.GetComponent<PlayerBase>().TakeDamage(_damage / 2);
                t = 0f;
            }
            t += Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _sporesDistance);
        }
    }
}