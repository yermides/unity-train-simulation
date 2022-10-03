using UnityEngine;

namespace SGS
{
    public class MaxSpeedMovementState : IState
    {
        private readonly Train _train;
        private readonly Transform _transform;
        
        public MaxSpeedMovementState(Train train)
        {
            _train = train;
            _transform = train.transform;
        }

        public void Tick()
        {
            // Use MRU equation
            _transform.position += _transform.forward * (_train.Speed * Time.deltaTime);
        }
    }
}