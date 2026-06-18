using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using FightSystem.Runtime;
using HealthSystem = Health.Runtime.Health;

namespace UI.Runtime
{
    public class ATBBarUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _uiDocument;
        
        public static event Action<CombatUnit, int> OnAttackSelected;
        
        private VisualElement _targetPanel, _actionPanel, _root, _renderer, _statsPanel;
        private Button _attackButton, _counter;
        private int _selectedAttackType = 0;
        private Dictionary<CombatUnit, VisualElement> _icons = new Dictionary<CombatUnit, VisualElement>();
        private Dictionary<CombatUnit, Label> _hpLabels = new Dictionary<CombatUnit, Label>();
        private Dictionary<CombatUnit, VisualElement> _statRows = new Dictionary<CombatUnit, VisualElement>();

        private void Start()
        {
            _root = _uiDocument.rootVisualElement;
            _renderer = _root.Q<VisualElement>("Renderer");
            _actionPanel = _root.Q<VisualElement>("ActionPanel");
            _targetPanel = _root.Q<VisualElement>("TargetPanel");
            _statsPanel = _root.Q<VisualElement>("StatsPanel");
            _attackButton = _root.Q<Button>("AttackButton");
            _counter = _root.Q<Button>("Counter");
            
            _attackButton.clicked += () => OnAttackClicked(1);
            _counter.clicked += () => OnAttackClicked(2);
            
            _root.style.display = DisplayStyle.None;
            _actionPanel.style.display = DisplayStyle.None;
            _targetPanel.style.display = DisplayStyle.None;
            _statsPanel.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (_icons.Count == 0) return;
            UpdateIcons();
            UpdateStatsPanel();
            CheckPlayerReady();
        }

        public void Initialize(List<CombatUnit> units)
        {
            
            _root.style.display = DisplayStyle.Flex;
            foreach (VisualElement icon in _icons.Values) icon.RemoveFromHierarchy();
            _icons.Clear();
            _hpLabels.Clear();
            _statRows.Clear();
            _statsPanel.Clear();
            _statsPanel.style.display = DisplayStyle.Flex;

            foreach (CombatUnit unit in units)
            {
                VisualElement container = new VisualElement();
                container.style.position = Position.Absolute;
                container.style.alignItems = Align.Center;

                VisualElement icon = new VisualElement();
                icon.style.width = 30;
                icon.style.height = 30;

                if (unit._icon != null) icon.style.backgroundImage = new StyleBackground(unit._icon);
                else icon.style.backgroundColor = new StyleColor(unit._isPlayer ? Color.green : Color.red);

                container.Add(icon);
                _renderer.Add(container);
                _icons[unit] = container;
                
                if (unit._isPlayer) continue;

                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.alignItems = Align.Center;
                row.style.marginBottom = 8;

                VisualElement statIcon = new VisualElement();
                statIcon.style.width = 30;
                statIcon.style.height = 30;
                statIcon.style.marginRight = 8;
                if (unit._icon != null) statIcon.style.backgroundImage = new StyleBackground(unit._icon);
                else statIcon.style.backgroundColor = new StyleColor(unit._isPlayer ? Color.green : Color.red);

                VisualElement info = new VisualElement();
                info.style.flexDirection = FlexDirection.Column;

                Label nameLabel = new Label(unit._unitName);
                nameLabel.style.fontSize = 12;
                nameLabel.style.color = new StyleColor(Color.white);

                Label hpLabel = new Label();
                hpLabel.style.fontSize = 11;
                hpLabel.style.color = new StyleColor(Color.green);

                info.Add(nameLabel);
                info.Add(hpLabel);
                row.Add(statIcon);
                row.Add(info);
                _statsPanel.Add(row);

                _hpLabels[unit] = hpLabel;
                _statRows[unit] = row;
            }
        }

        private void UpdateIcons()
        {
            float barWidth = _renderer.resolvedStyle.width;
            List<CombatUnit> deadUnits = new List<CombatUnit>();

            foreach (var pair in _icons)
            {
                if (pair.Key == null) { pair.Value.RemoveFromHierarchy(); deadUnits.Add(pair.Key); continue; }
                pair.Value.style.left = pair.Key.Progress * barWidth - 15f;
            }
            foreach (var dead in deadUnits) 
            { 
                _icons.Remove(dead); 
                _hpLabels.Remove(dead);
                if (_statRows.TryGetValue(dead, out VisualElement row))
                {
                    row.RemoveFromHierarchy();
                    _statRows.Remove(dead);
                }
            }
        }

        private void UpdateStatsPanel()
        {
            foreach (var pair in _hpLabels)
            {
                if (pair.Key == null || pair.Key._isPlayer) continue;
                HealthSystem hp = pair.Key.GetComponent<HealthSystem>();
                if (hp != null)
                    pair.Value.text = $"{(int)hp.CurrentHP}/{(int)pair.Key._statistique.m_MaxHp}";
            }
        }

        private void CheckPlayerReady()
        {
            bool playerReady = false;
            foreach (var pair in _icons)
                if (pair.Key != null && pair.Key._isPlayer && pair.Key.State == CombatState.Casting) playerReady = true;

            if (playerReady && _selectedAttackType == 0)
                _actionPanel.style.display = DisplayStyle.Flex;
            else if (!playerReady)
            {
                _actionPanel.style.display = DisplayStyle.None;
                _targetPanel.style.display = DisplayStyle.None;
                _selectedAttackType = 0;
            }
        }

        private void OnAttackClicked(int attackType)
        {
            _selectedAttackType = attackType;
            _actionPanel.style.display = DisplayStyle.None;
            ShowTargetPanel();
        }

        private void ShowTargetPanel()
        {
            _targetPanel.Clear();
            _targetPanel.style.display = DisplayStyle.Flex;

            foreach (var pair in _icons)
            {
                if (pair.Key == null || pair.Key._isPlayer) continue;

                CombatUnit enemy = pair.Key;
                Button btn = new Button();
                btn.text = enemy._unitName;
                btn.style.width = 130;
                btn.style.height = 40;
                btn.style.marginBottom = 5;
                btn.clicked += () => OnTargetSelected(enemy);
                _targetPanel.Add(btn);
            }
        }

        private void OnTargetSelected(CombatUnit target)
        {
            _targetPanel.style.display = DisplayStyle.None;
            OnAttackSelected?.Invoke(target, _selectedAttackType);
            _selectedAttackType = 0;
        }

        public void Hide() 
        { 
            _root.style.display = DisplayStyle.None;
            _icons.Clear();
            _hpLabels.Clear();
            _statRows.Clear();
            _selectedAttackType = 0;
            _statsPanel.style.display = DisplayStyle.None;
        }
    }
}