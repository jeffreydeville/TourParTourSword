using UnityEngine;
using System.Collections.Generic;
using PlayerController.Runtime;
using FightSystem.Runtime;

namespace GameManager.Runtime
{
    public class GameManager : MonoBehaviour
    {
        #region Publics

        public static GameManager Instance { get; private set; }

        #endregion


        #region Unity API

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            FightTrigger.OnFightTriggered += StartFight;
        }

        private void OnDisable()
        {
            FightTrigger.OnFightTriggered -= StartFight;
        }

        #endregion


        #region Main API

        private void StartFight(List<GameObject> enemies)
        {
            if (_isInFight) return;
            _isInFight = true;
            _currentEnemies = enemies;
            _playerController.enabled = false;
            _playerController.GetComponent<Animator>().SetBool("IsMoving", false);

            List<CombatUnit> units = new List<CombatUnit>();

            CombatUnit playerUnit = _playerController.GetComponent<CombatUnit>();
            if (playerUnit != null) units.Add(playerUnit);

            foreach (GameObject enemy in enemies)
            {
                CombatUnit unit = enemy.GetComponent<CombatUnit>();
                if (unit != null) units.Add(unit);
            }

            
            ATBManager.Instance.StartCombat(units);
        }

        public void EndFight(bool playerWon)
        {
            _isInFight = false;
            _playerController.enabled = true;

            if (playerWon)
            {
                foreach (GameObject enemy in _currentEnemies)
                    Destroy(enemy);
            }

            _currentEnemies.Clear();
            ATBManager.Instance.StopCombat();
        }

        #endregion


        #region Private and Protected

        [SerializeField] private Controller _playerController;

        private bool _isInFight = false;
        private List<GameObject> _currentEnemies = new List<GameObject>();

        #endregion
    }
}