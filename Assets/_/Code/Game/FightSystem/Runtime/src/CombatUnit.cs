using UnityEngine;
using PlayerStatistique.Runtime;

namespace FightSystem.Runtime
{
   
    public enum CombatState 
    { 
        Waiting,  
        Casting,  
        Acting    
    }

    public class CombatUnit : MonoBehaviour
    {
        #region Publics

        public string _unitName;
        public bool _isPlayer = false;
        public Statistique _statistique;
        public Sprite _icon;

      
        public float Progress { get; private set; } = 0f;                                   
                                                                                 //permet un abbonement par tout le monde a linverse de => ( public float Progress = 0f; Variable normale - tout le monde peut lire ET modifier)
        public CombatState State { get; private set; } = CombatState.Waiting;

        #endregion


        #region Main API

        public void Initialize()
        {
            Progress = 0f;
            State = CombatState.Waiting;
        }

        public void IconProgress(float deltaTime)
        {
            if (State == CombatState.Acting) return;

            Progress += deltaTime * _statistique.m_attackSpeed * 0.1f;
            Progress = Mathf.Clamp01(Progress);

            if (Progress >= 0.75f && State == CombatState.Waiting)
                State = CombatState.Casting;

            if (Progress >= 1f && State == CombatState.Casting)
                State = CombatState.Acting;
        }

        public void Interrupt()
        {
            if (State == CombatState.Waiting) return;
            Progress = 0.4f; 
            State = CombatState.Waiting;
        }

        public void ResetTurn()
        {
            Progress = 0f;
            State = CombatState.Waiting;
        }

        #endregion
    }
}