using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using FightSystem.Runtime;

namespace UI.Runtime
{
    public class ATBBarUI : MonoBehaviour
    {
        #region Publics

        [SerializeField] private UIDocument _uiDocument;

        public static event Action<CombatUnit> OnPlayerAttack;
        
        #endregion


        #region Unity API

        private void Start()
        {
            _root = _uiDocument.rootVisualElement;
            _renderer = _root.Q<VisualElement>("Renderer");
            _actionPanel = _root.Q<VisualElement>("ActionPanel");
            _attackButton = _root.Q<Button>("AttackButton");
            
            _attackButton.clicked += OnAttackClicked;
            _root.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (_icons.Count == 0) return;
            UpdateIcons();
            CheckPlayerReady();
        }

        #endregion


        #region Main API

        public void Initialize(List<CombatUnit> units)
        {
            _root.style.display = DisplayStyle.Flex;
            foreach (VisualElement icon in _icons.Values)
                icon.RemoveFromHierarchy();
            _icons.Clear();

            
            foreach (CombatUnit unit in units)
            {
                VisualElement icon = new VisualElement();
                icon.style.width = 30;
                icon.style.height = 30;
                icon.style.position = Position.Absolute;
                icon.style.top = 0;

                
                if (unit._icon != null)
                    icon.style.backgroundImage = new StyleBackground(unit._icon);
                else if (unit._isPlayer)
                    icon.style.backgroundColor = new StyleColor(Color.green);
                else
                    icon.style.backgroundColor = new StyleColor(Color.red);

                _renderer.Add(icon);
                _icons[unit] = icon;
            }
        }
        
        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
            foreach (VisualElement icon in _icons.Values)
                icon.RemoveFromHierarchy();
            _icons.Clear();
        }

        private void UpdateIcons()
        {
            float barWidth = _renderer.resolvedStyle.width;
            
            foreach (var pair in _icons)
            {
                
                CombatUnit unit = pair.Key;
                VisualElement icon = pair.Value;

                float xPos = unit.Progress * barWidth - 15f; 
                icon.style.left = xPos;
            }
        }
        private void CheckPlayerReady()
        {
            foreach (var pair in _icons)
            {
                CombatUnit unit = pair.Key;
                if (unit._isPlayer && unit.State == CombatState.Acting)
                {
                    _currentEnemy = null;
                    float highestProgress = -1f;
                    foreach (var p in _icons)
                    {
                        if (!p.Key._isPlayer && p.Key.Progress > highestProgress)
                        {
                            highestProgress = p.Key.Progress;
                            _currentEnemy = p.Key;
                        }
                    }

                    _actionPanel.style.display = DisplayStyle.Flex;
                    return;
                }
            }
            _actionPanel.style.display = DisplayStyle.None;
        }

        private void OnAttackClicked()
        {
            if (_currentEnemy == null) return;
            OnPlayerAttack?.Invoke(_currentEnemy);
        }
        
        #endregion


        #region Private and Protected

        private VisualElement _actionPanel;
        private Button _attackButton;
        private CombatUnit _currentEnemy;
        
        private VisualElement _root;
        private VisualElement _renderer;
        private Dictionary<CombatUnit, VisualElement> _icons = new Dictionary<CombatUnit, VisualElement>();


        #endregion
    }
}