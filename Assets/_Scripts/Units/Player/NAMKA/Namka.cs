using rene_roid;
using rene_roid_enemy;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace rene_roid_player
{
    public class Namka : PlayerBase
    {
        #region Internal
        private CapsuleCollider2D _collider;
        private SpriteRenderer _renderer;

        
        #endregion

        public override void Start()
        {
            base.Start();
            _renderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<CapsuleCollider2D>();
        }

        public override void Update()
        {
            base.Update();
            
            Skill2Cancel();
        }

        #region Skills
        [Header("Skills")]
        [Header("Basic Attack")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _basicAttackPercentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoBasic = 1f;

        public override void BasicAttack()
        {
            LastSkillProcCoefficient = _procCoBasic;
            base.BasicAttack();

            if (!_skill2Active) StartCoroutine(RemoveControl(.15f));

            if (IsGrounded() && !_skill2Active) _rb.velocity = new Vector2(0, _rb.velocity.y);

            RaycastHit2D hit = Physics2D.Raycast(_collider.bounds.center, _renderer.flipX ? Vector2.left : Vector2.right, _collider.bounds.extents.x + 100f, _enemyLayer);
            if (hit.collider != null)
            {
                var enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(DealDamage(_basicAttackPercentage, _procCoBasic));
            }
        }


        [Header("Skill 1")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _skill1Percentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoSkill1 = 1f;

        [SerializeField] private BoxCollider2D _skill1Rad;
        [SerializeField] private GameObject _skill1Slash;
        [SerializeField] private Vector2 _skill1SpawnOffset;

        public override void Skill1()
        {
           if (!IsGrounded()) return;
           LastSkillProcCoefficient = _procCoSkill1;
            base.Skill1();

            // StartCoroutine(RemoveControl(.6f));

            _rb.velocity = new Vector2(0, 0);

            StartCoroutine(DealDamage(_skill1Percentage, _procCoSkill1, 0.30f));
        }

        private IEnumerator DealDamage(float percentage, float procCo, float delay)
        {
            yield return new WaitForSeconds(delay);

            // Flip _skill1Rad to the correct direction
            if (_renderer.flipX) _skill1Rad.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            else _skill1Rad.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Detect if _enemyLayer is inside _skill2Rad
            var enemies = Physics2D.OverlapBoxAll(_skill1Rad.bounds.center, _skill1Rad.bounds.size, 0, _enemyLayer);
            foreach (var enemy in enemies)
            {
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase != null) enemyBase.TakeDamage(DealDamage(percentage, procCo));
            }

            // Wait until current animation reached 0.3 seconds
            yield return new WaitForSeconds(0.2f);

            // Spawn _skill1Slash
            var slash = Instantiate(_skill1Slash, transform.position + (Vector3)(_renderer.flipX ? new Vector2(-_skill1SpawnOffset.x, _skill1SpawnOffset.y) : _skill1SpawnOffset), Quaternion.identity);
            var slashScript = slash.GetComponent<NamkaSlash>();
            slashScript.SetFlipX(_renderer.flipX);
            slashScript.SetSlashInfo(this, percentage, procCo, MaxStats.MovementSpeed * 1.5f);
        }

        public void Skill1Slash()
        {
            // Spawn _skill1Slash
            var slash = Instantiate(_skill1Slash, transform.position + (Vector3)_skill1SpawnOffset, Quaternion.identity);
            slash.GetComponent<NamkaSlash>().SetSlashInfo(this, _skill1Percentage / 3, _procCoSkill1, MaxStats.MovementSpeed * 2);
            slash.GetComponent<NamkaSlash>().SetFlipX(_renderer.flipX);
        }


        [Header("Skill 2")]
        [Tooltip("The clone duration before it disappears")]
        [SerializeField] private float _skill2Duration = 10f;
        [SerializeField] private GameObject _skill2Clone;
        [SerializeField] private float _healPercentage = 0.5f;

        private float _skill2TimerClone = 0f;
        private bool _skill2Active = false;

        private float _startHealth = 0f;

        private Vector2 _initClonePos;
        

        public override void Skill2()
        {
            base.Skill2();
            _skill2Active = true;
            _startHealth = CurrentHealth;

            StartCoroutine(Skill2Clone());
        }

        private IEnumerator Skill2Clone()
        {
            // Construct new gameobject with the same position and rotation and the same sprite
            GameObject initClone = new GameObject();
            initClone.transform.position = transform.position;
            initClone.transform.rotation = transform.rotation;
            initClone.AddComponent<SpriteRenderer>().sprite = _renderer.sprite;

            // Instantiate the initClone
            var clon = Instantiate(initClone, transform.position, transform.rotation);

            _initClonePos = initClone.transform.position;

            // Spawn _skill2Clone every x seconds
            while (_skill2TimerClone < _skill2Duration)
            {
                _skill2TimerClone += 0.1f;
                yield return new WaitForSeconds(0.1f);

                var clone = Instantiate(_skill2Clone, transform.position, Quaternion.identity);
                clone.GetComponent<CloneFade>().SetFadeTime(1);
                clone.GetComponent<CloneFade>().SetSprite(_renderer.sprite, _renderer.flipX);
                clone.GetComponent<CloneFade>().Fade();
            }

            // Heal player
            HealAmmount(_startHealth * _healPercentage);

            // Reset variables
            _skill2TimerClone = 0f;
            _skill2Active = false;

            // Destroy initClone
            Destroy(initClone);

            // Destroy clon
            Destroy(clon);
        }

        // If player presses skill 2 before the clone is done, teleport player to the clone
        private void Skill2Cancel()
        {
            if (!_skill2Active) return;
            
            print("Skill 2 Pressed");

            if (_frameInput.SpecialAttack2Down)
            {
                if (_skill2TimerClone > 0.1f)
                {
                    _skill2TimerClone = _skill2Duration;
                    transform.position = _initClonePos;
                    print("Skill 2 Cancelled");
                }
            }
        }


        [Header("Ultimate")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _ultimatePercentage = 5;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoUltimate = 2f;

        [SerializeField] private GameObject _ultimateRad;
        [SerializeField] private float _ultimateChargeTime = 2f;

        private float _ultimateChargeTimer = 0f;

        [SerializeField] private GameObject _nuke;

        public override void Ultimate()
        {
            if (!IsGrounded()) return;
            LastSkillProcCoefficient = _procCoUltimate;

            base.Ultimate();

            var nuk = Instantiate(_nuke, transform.position, Quaternion.identity, transform);
            nuk.GetComponent<NukeNamka>().Damage = _ultimatePercentage;
            nuk.GetComponent<NukeNamka>().parent = this;

            //StartCoroutine(RemoveControl(_ultimateChargeTime + 1.5f));

            //_rb.velocity = new Vector2(0, 0);

            //StartCoroutine(DealDamageUltimate(_ultimatePercentage, _procCoUltimate, 2));
        }

        private IEnumerator DealDamageUltimate(float percentage, float procCo, float delay)
        {
            var cam = Helpers.Camera;
            var camScript = cam.GetComponent<CameraFollow>();
            var camX = cam.orthographicSize * cam.aspect;

            // Flip the camX if the player is facing left
            if (_renderer.flipX) camX *= -1;

            // Flip _ultimateRad to the correct direction
            if (_renderer.flipX) _ultimateRad.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            else _ultimateRad.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            var boxRad = _ultimateRad.GetComponent<BoxCollider2D>();

            if (_renderer.flipX) _ultimateRad.transform.position = new Vector3(-_ultimateRad.transform.position.x, _ultimateRad.transform.position.y, _ultimateRad.transform.position.z);
            else _ultimateRad.transform.position = new Vector3(_ultimateRad.transform.position.x, _ultimateRad.transform.position.y, _ultimateRad.transform.position.z);

            camScript.MoveCameraToPosition(new Vector2(transform.position.x + camX, cam.transform.position.y), delay);
            yield return new WaitForSeconds(delay + 0.5f);
            _ultimateRad.SetActive(true);

            // Detect if _enemyLayer is inside _ultimateRad
            var enemies = Physics2D.OverlapBoxAll(boxRad.bounds.center, boxRad.bounds.size, 0, _enemyLayer);

            // Deal damage to all enemies
            foreach (var enemy in enemies)
            {
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase != null) enemyBase.TakeDamage(DealDamage(percentage, procCo));
            }

            yield return new WaitForSeconds(1f);
            _ultimateRad.SetActive(false);
            camScript.ReturnToPlayer();
        }

        #endregion

        #region Functions
        private IEnumerator RemoveControl(float t)
        {
            if (IsGrounded())
            {
                this.TakeAwayControl(false);
                yield return new WaitForSeconds(t);
                ReturnControl();
            }
        }
        #endregion
    }
}
