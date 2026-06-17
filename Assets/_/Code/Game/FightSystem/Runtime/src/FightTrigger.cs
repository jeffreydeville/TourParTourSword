using UnityEngine;
using System;
using System.Collections.Generic;

namespace FightSystem.Runtime
{
    public class FightTrigger : MonoBehaviour
    {
        public static event Action<List<GameObject>> OnFightTriggered;

        [SerializeField] private float _allyDetectionRadius = 3f;
        [SerializeField] private LayerMask _enemyLayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_hasTriggered) return;
            _hasTriggered = true;

            List<GameObject> enemies = new List<GameObject>();
            enemies.Add(gameObject);

            Collider2D[] OtherEnnemy = Physics2D.OverlapCircleAll(transform.position, _allyDetectionRadius, _enemyLayer);
            foreach (Collider2D col in OtherEnnemy)
            {
                if (col.gameObject == gameObject) continue;
                enemies.Add(col.gameObject);
        
                FightTrigger allyTrigger = col.gameObject.GetComponent<FightTrigger>();
                if (allyTrigger != null) allyTrigger._hasTriggered = true;
            }

            OnFightTriggered?.Invoke(enemies);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _allyDetectionRadius);
        }
        
        
        private bool _hasTriggered = false;
    }
}