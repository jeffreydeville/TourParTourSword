using UnityEngine;
using System.Collections.Generic;
using HealthSystem = Health.Runtime.Health;
using UI.Runtime;
    
namespace FightSystem.Runtime
{
    public class ATBManager : MonoBehaviour
    {
        #region Publics

        public static ATBManager Instance { get; private set; }
        
        [SerializeField] private ATBBarUI _atbBarUI;


        #endregion


        #region Unity API

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            if (!_isCombatActive) return;

            foreach (CombatUnit unit in _units)
            {
                unit.IconProgress(Time.unscaledDeltaTime);
                
                if (unit.State == CombatState.Acting && !unit._isPlayer)
                    EnemyAct(unit);
            }
        }
        private void OnEnable()
        {
            ATBBarUI.OnPlayerAttack += PlayerAct;
        }

        private void OnDisable()
        {
            ATBBarUI.OnPlayerAttack -= PlayerAct;
        }

        #endregion


        #region Main API

        public void StartCombat(List<CombatUnit> units)
        {
            _units = units;

            foreach (CombatUnit unit in _units)
                unit.Initialize();
            
            _atbBarUI.Initialize(units);
            _isCombatActive = true;
        }

        public void PlayerAct(CombatUnit target)
        {
            CombatUnit player = _units.Find(u => u._isPlayer);
            if (player == null) return;

            if (player.State != CombatState.Acting) return;

            target.GetComponent<HealthSystem>().TakeDamage(player._statistique.m_attackDamage);

            target.Interrupt();

            CheckCombatEnd();

            player.ResetTurn();
        }

        public void StopCombat()
        {
            _isCombatActive = false;
            _units.Clear();
            _atbBarUI.Hide();
        }

        private void EnemyAct(CombatUnit enemy)
        {
            CombatUnit player = _units.Find(u => u._isPlayer);
            if (player == null) return;

            player.GetComponent<HealthSystem>().TakeDamage(enemy._statistique.m_attackDamage);

            player.Interrupt();

            enemy.ResetTurn();
        }

        private void CheckCombatEnd()
        {
            bool allEnemiesDead = !_units.Exists(u => !u._isPlayer && u.GetComponent<HealthSystem>().enabled);

            if (allEnemiesDead)
                GameManager.Runtime.GameManager.Instance.EndFight(true);

            CombatUnit player = _units.Find(u => u._isPlayer);
            if (player == null) return;

            bool playerDead = !player.GetComponent<HealthSystem>().enabled;
            if (playerDead)
                GameManager.Runtime.GameManager.Instance.EndFight(false);
        }
        
        #endregion


        #region Private and Protected

        private bool _isCombatActive = false;
        private List<CombatUnit> _units = new List<CombatUnit>();


        #endregion
    }
}