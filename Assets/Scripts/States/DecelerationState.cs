using UnityEngine;

namespace SGS
{
    public class DecelerationState : IState
    {
        private readonly Train _train;
        private readonly Transform _transform;
        
        public DecelerationState(Train train)
        {
            _train = train;
            _transform = train.transform;
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            
            // Calculate next speed, factoring the deceleration
            float nextSpeed = _train.Speed + (_train.Deceleration * deltaTime);
            
            // Clamp up and down so we don't exceed max speed
            _train.Speed = Mathf.Clamp(nextSpeed, 0, _train.MaxSpeed);
            
            // Finally, move with the new speed value
            _transform.position += _transform.forward * (_train.Speed * deltaTime);
        }
    }
}