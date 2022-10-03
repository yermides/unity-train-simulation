using UnityEngine;

namespace SGS
{
    public class AccelerationState : IState
    {
        private readonly Train _train;
        private readonly Transform _transform;
        
        public AccelerationState(Train train)
        {
            _train = train;
            _transform = train.transform;
        }

        public void Tick()
        {
            // Using MRUA equation
            float deltaTime = Time.deltaTime;
            
            // Calculate next speed, factoring the acceleration
            float nextSpeed = _train.Speed + (_train.Acceleration * deltaTime);
            
            // Clamp up and down so we don't exceed max speed
            _train.Speed = Mathf.Clamp(nextSpeed, 0, _train.MaxSpeed);
            
            // Finally, move with the new speed value
            _transform.position += _transform.forward * (_train.Speed * deltaTime);
        }
    }
}