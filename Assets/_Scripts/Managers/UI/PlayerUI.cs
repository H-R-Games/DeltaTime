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
        [Header("Health")]
        public Image HealthBar;
        public TMP_Text HealthText;

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
            UpdateLevel();
            UpdateAbilityIcons();
        }

        #region Healthbar
        private void UpdateHealthbar()
        {
            HealthText.text = _playerScript.CurrentHealth.ToString("0");

            // Smooth the healthbar
            float healthPercentage = _playerScript.CurrentHealth / _playerScript.MaxStats.Health + (_playerScript.MaxStats.HealthPerLevel *_playerScript.Level);
            HealthBar.fillAmount = Mathf.Lerp(HealthBar.fillAmount, healthPercentage, Time.deltaTime * 10);
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

        [Header("Level")]
        public TMP_Text LevelText;
        public Image LevelFiller;

        public void UpdateLevel() {
            int level = _playerScript.Level;
            LevelText.text = level.ToString();

            // smooth
            float levelPercentage = _playerScript.CurrentExperience / _playerScript.ExperienceToNextLevel;
            LevelFiller.fillAmount = Mathf.Lerp(LevelFiller.fillAmount, levelPercentage, Time.deltaTime * 10);
        }
        
    }
}
