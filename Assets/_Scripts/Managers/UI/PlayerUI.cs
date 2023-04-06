using rene_roid_player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace rene_roid
{
    public class PlayerUI : MonoBehaviour
    {
        #region Internal
        [Header("Internal")]
        [SerializeField] private GameObject _player;

        private PlayerBase _playerScript;
        #endregion

        #region External
        [Header("External")]
        [Header("Healt")]
        public Image HealthBar;

        [Header("Abilities")]
        public Image BasicAttackFiller;
        public Image BasicAttackSprite;
        public Image SpecialAttack1Filler;
        public Image SpecialAttack1Sprite;
        public Image SpecialAttack2Filler;
        public Image SpecialAttack2Sprite;
        public Image UltimateFiller;
        public Image UltimateSprite;
        #endregion

        private void Awake()
        {
        }

        private void Start() {
            if (_player == null) _player = GameObject.FindObjectOfType<PlayerBase>().gameObject;
            _playerScript = _player.GetComponent<PlayerBase>();
        }

        private void Update() {
            UpdateHealthbar();
            UpdateAbilityIcons();
        }

        #region Healthbar
        private void UpdateHealthbar()
        {
            HealthBar.fillAmount = _playerScript.CurrentHealth / _playerScript.MaxStats.Health;
        }
        #endregion

        #region Ability Icons
        private void UpdateAbilityIcons() {
            BasicAttackFiller.fillAmount = _playerScript.BasicAttackTimer / _playerScript.BasicAttackCooldown;
            SpecialAttack1Filler.fillAmount = _playerScript.Skill1Timer / _playerScript.Skill1Cooldown;
            SpecialAttack2Filler.fillAmount = _playerScript.Skill2Timer / _playerScript.Skill2Cooldown;
            UltimateFiller.fillAmount = _playerScript.UltimateTimer / _playerScript.UltimateCooldown;
        }
        #endregion
        
    }
}
