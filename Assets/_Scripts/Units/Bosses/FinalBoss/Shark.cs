using System.Collections;
using UnityEngine;

namespace rene_roid_enemy {    
    public class Shark : MonoBehaviour
    {
        private bool _onGoing = false;
        public Transform _target;
        public FinalBoss _boss;

        void Update()
        {
            if (_boss.ZaWarudoActive == true && _onGoing == false)
            {
                _onGoing = true;
                StartCoroutine(SharkAttack());
            }
        }

        IEnumerator SharkAttack() {
            yield return new WaitForSeconds(1f);
            var rx = Random.Range(-1f, 1f);
            var ry = Random.Range(-1f, 1f);
            var bosspos = _boss.transform.position;
            bosspos.x += rx;
            bosspos.y += ry;
            var dirboss = (bosspos - transform.position).normalized;

            var t = 0f;
            var dur = 3f;
            while (t < 1) {
                t += Time.deltaTime / dur;
                if (Vector2.Distance(bosspos, transform.position) > 1f) {
                    transform.position = Vector2.Lerp(transform.position, bosspos, t);
                    print("moving to boss");
                } else {
                    t = 1f;
                }
                yield return null;
            }
            print("reached boss");

            var targetPos = _target.position;
            var speed = 1f;

            t = 0f;
            dur = 5f;
            while (t < 1) {
                t += Time.deltaTime / dur;
                transform.position += (targetPos - transform.position).normalized * Time.deltaTime * speed;
                //transform.LookAt(_target);
                print("moving to player");
                yield return null;
            }
            print("reached player");

            t = 0f;
            dur = 3f;
            speed = 30f;
            targetPos = new Vector2(_target.position.x, _target.position.y);
            var dir = (targetPos - transform.position).normalized;
            while (t < 1) {
                t += Time.deltaTime / dur;
                transform.position += dir * Time.deltaTime * speed;
                print("attacking to player");
                yield return null;
            }
            print("attacked player");
        }
    }
}
