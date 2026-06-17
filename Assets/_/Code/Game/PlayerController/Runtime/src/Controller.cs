using UnityEngine;
using UnityEngine.InputSystem;
using PlayerStatistique.Runtime;

namespace PlayerController.Runtime
{
    public class Controller : MonoBehaviour
    {
        #region Publics

        [Header("Settings")]
        public InputActionReference m_moveActionReference;
        public Rigidbody2D m_rigidbody;
        
        [Header("Annimations")]
        public Animator _isMoving;

        #endregion


        #region Unity API

        private void Start()
        {
            _speed = _statistique.m_speed;
        }

        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void OnEnable()
        {
            m_moveActionReference.action.Enable();
        }

        private void OnDisable()
        {
            m_moveActionReference.action.Disable();
        }

        #endregion


        #region Main API

        private void ReadInput()
        {
            _input = m_moveActionReference.action.ReadValue<Vector2>();
            if (_input.x > 0f) transform.localScale = new Vector3(1f, 1f, 1f);
            else if (_input.x < 0f) transform.localScale = new Vector3(-1f, 1f, 1f);
            
            _isMoving.SetBool("IsMoving", _input != Vector2.zero);
        }
        
        private void Movement()
        {
            m_rigidbody.MovePosition(m_rigidbody.position + _input * _speed * Time.fixedDeltaTime);
        }

        public void SetSpeed(float speed) => _speed = speed;

        
       
        
        #endregion


        #region Private and Protected

        [SerializeField] private Statistique _statistique;

        private Vector2 _input;
        private float _speed;

        #endregion
    }
}