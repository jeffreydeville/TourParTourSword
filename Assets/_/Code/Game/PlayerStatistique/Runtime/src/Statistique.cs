using UnityEngine;

namespace PlayerStatistique.Runtime
{
    [CreateAssetMenu(fileName = "Statistique", menuName = "Game/Statistique")]
    public class Statistique : ScriptableObject
    {
        [Header("Hp")]
        public float m_MaxHp = 10f;
        public float m_defaultMaxHp = 10f;

        [Header("Attack")]
        public float m_attackDamage = 10f;
        public float m_defaultAttackDamage = 10f;
        public float m_attackSpeed = 10f;
        public float m_defaultAttackSpeed = 10f;
        public float m_attackrange = 10f;
        public float m_defaultAttackRange = 10f;

        [Header("Projectile")]
        public float m_projectileSpeed = 10f;
        public float m_defaultProjectileSpeed = 10f;
        public float m_projectileLifeTime = 5f;
        public float m_defaultProjectileLifeTime = 5f;

        [Header("Speed")]
        public float m_speed = 1f;
        public float m_defaultSpeed = 1f;

        [Header("Dash")]
        public bool m_dash = true;
        public bool m_defaultDash = true;
        public float m_dashForce = 10f;
        public float m_defaultDashForce = 10f;
        public float m_dashDuration = 0.2f;
        public float m_defaultDashDuration = 0.2f;
        public float m_dashCooldown = 1f;
        public float m_defaultDashCooldown = 1f;

        public void ResetToDefault()
        {
            m_MaxHp = m_defaultMaxHp;
            m_attackDamage = m_defaultAttackDamage;
            m_attackSpeed = m_defaultAttackSpeed;
            m_attackrange = m_defaultAttackRange;
            m_projectileSpeed = m_defaultProjectileSpeed;
            m_projectileLifeTime = m_defaultProjectileLifeTime;
            m_speed = m_defaultSpeed;
            m_dash = m_defaultDash;
            m_dashForce = m_defaultDashForce;
            m_dashDuration = m_defaultDashDuration;
            m_dashCooldown = m_defaultDashCooldown;
        }
    }
}