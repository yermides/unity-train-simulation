using System;
using UnityEngine;

namespace SGS
{
    public class Train : MonoBehaviour
    {
        [SerializeField, Tooltip("Meters per second")] 
        private float maxSpeed;
        
        [SerializeField, Tooltip("Meters per second square")] 
        private float accelerationRate;
        
        [SerializeField,Tooltip("Meters per second square, must be a negative value")] 
        private float decelerationRate;

        private float _currentSpeed; // speed in meters per second
        private StateMachine _stateMachine;
        // private IState _state;

        public float Speed
        {
            get => _currentSpeed;
            set => _currentSpeed = value;
        }

        public float MaxSpeed => maxSpeed;
        public float Acceleration => accelerationRate;
        public float Deceleration => decelerationRate;
        public StateMachine States => _stateMachine;

        public static float Width => 14.0f;

        private void Awake()
        {
            _stateMachine = new StateMachine();
        }

        private void Update()
        {
            _stateMachine.Tick();
        }
    }
}
