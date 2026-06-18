using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HealthSystem = Health.Runtime.Health;
using UI.Runtime;
    
namespace FightSystem.Runtime
{
    public class ATBManager : MonoBehaviour
    {
        public static ATBManager Instance { get; private set; }
        

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Update()
        {
            if (!_isCombatActive || _isActing) return;

            CombatUnit player = _units.Find(u => u._isPlayer);

            if (player != null && player.State == CombatState.Acting && _selectedAttackType != 0)
            {
                _isActing = true;
                StartCoroutine(ExecutePlayerAct(player));
            }

            if (player != null && player.State == CombatState.Casting && _selectedAttackType == 0)
                _combatSpeed = 0.01f;
            else if (_selectedAttackType == 0)
                _combatSpeed = 1f;

            foreach (CombatUnit unit in _units)
            {
                float speed = (unit._isPlayer && _selectedAttackType != 0) ? _playerCastSpeed : _combatSpeed;
                unit.IconProgress(Time.unscaledDeltaTime * speed);
                if (unit.State == CombatState.Acting && !unit._isPlayer)
                    EnemyAct(unit);
            }
        }

        private void OnEnable() => ATBBarUI.OnAttackSelected += SelectAttack;
        private void OnDisable() => ATBBarUI.OnAttackSelected -= SelectAttack;

        public void StartCombat(List<CombatUnit> units)
        {
            _units = units;
            foreach (CombatUnit unit in _units) unit.Initialize();
            _atbBarUI.Initialize(units);
            _isCombatActive = true;
        }

        public void StopCombat()
        {
            _isCombatActive = false;
            _isActing = false;
            _units.Clear();
            if (_atbBarUI != null) _atbBarUI.Hide();
        }

        public void SelectAttack(CombatUnit target, int attackType)
        {
            _currentTarget = target;
            _selectedAttackType = attackType;
            _combatSpeed = 1f;
            _playerCastSpeed = (attackType == 1) ? _normalCastSpeed : _counterCastSpeed;
        }

        private IEnumerator ExecutePlayerAct(CombatUnit player)
        {
            if (_currentTarget == null) { _isActing = false; yield break; }

            Animator animator = _playerTransform.GetComponent<Animator>();
            Vector3 startPos = _playerTransform.position;
            Vector3 targetPos = _currentTarget.transform.position + new Vector3(-0.2f, -1f, 0f);
            
            _combatSpeed = 0f;
            _playerCastSpeed = 0f;

            while (Vector3.Distance(_playerTransform.position, targetPos) > _attackStopDistance)
            {
                _playerTransform.position = Vector3.MoveTowards(
                    _playerTransform.position, targetPos, _attackMoveSpeed * Time.unscaledDeltaTime);
                yield return null;
            }

            if (animator != null)
            {
                string trigger = (_selectedAttackType == 1) ? "AttackKnife" : "AttackHammer";
                animator.SetTrigger(trigger);

                yield return new WaitForSecondsRealtime(0.5f);
            }

            float multiplier = (_selectedAttackType == 1) ? _normalAttackMultiplier : _counterAttackMultiplier;
            _currentTarget.GetComponent<HealthSystem>().TakeDamage(player._statistique.m_attackDamage * multiplier);
            _currentTarget.Interrupt();
            CheckCombatEnd();
            player.ResetTurn();

            while (Vector3.Distance(_playerTransform.position, startPos) > 0.05f)
            {
                _playerTransform.position = Vector3.MoveTowards(
                    _playerTransform.position, startPos, _attackMoveSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            _playerTransform.position = startPos;

            _combatSpeed = 1f;
            _playerCastSpeed = 1f;
            _selectedAttackType = 0;
            _currentTarget = null;
            _isActing = false;
        }

        private void CheckCombatEnd()
        {
            _units.RemoveAll(u => u == null);

            bool allEnemiesDead = !_units.Exists(u => u != null && !u._isPlayer && !u.GetComponent<HealthSystem>()._isDead);

            if (allEnemiesDead)
            {
                GameManager.Runtime.GameManager.Instance.EndFight(true);
                return;
            }

            CombatUnit player = _units.Find(u => u != null && u._isPlayer);
            if (player != null && player.GetComponent<HealthSystem>()._isDead)
                GameManager.Runtime.GameManager.Instance.EndFight(false);
        }

        private void EnemyAct(CombatUnit enemy)
        {
            CombatUnit player = _units.Find(u => u._isPlayer);
            if (player == null) return;
            player.GetComponent<HealthSystem>().TakeDamage(enemy._statistique.m_attackDamage);
            player.Interrupt();
            enemy.ResetTurn();
        }

        private float _combatSpeed = 1f;
        private float _playerCastSpeed = 1f;
        private bool _isCombatActive = false;
        private bool _isActing = false;
        private int _selectedAttackType = 0;
        private CombatUnit _currentTarget = null;
        private List<CombatUnit> _units = new List<CombatUnit>();

        [Header("Paramètres des Attaques")]
        [SerializeField] private float _normalAttackMultiplier = 1.0f;
        [SerializeField] private float _normalCastSpeed = 0.01f;
        [SerializeField] private float _counterAttackMultiplier = 1.5f;
        [SerializeField] private float _counterCastSpeed = 0.005f;
        [SerializeField] private ATBBarUI _atbBarUI;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _attackMoveSpeed = 8f;
        [SerializeField] private float _attackStopDistance = 1f;
    }
}