using UnityEngine;
using StateTransitionCondition = System.Func<bool>;

namespace SGS
{
    // This class is more of a mediator that sets up both trains
    public class SimulationFacade : MonoBehaviour
    {
        [SerializeField] private Train trainA;
        [SerializeField] private Train trainB;
        [SerializeField, Min(0.0f), Tooltip("In Meters")] private float safetyStoppingThreshold;
        [SerializeField] private bool startSimulationOnAwake;
        private float _initialDistanceBetweenTrains; // Meters
        private Vector3 _initialPositionA, _initialPositionB; // Meters
        
        private void Awake()
        {
            // Remember positions to be able to replay simulation
            _initialPositionA = trainA.transform.position;
            _initialPositionB = trainB.transform.position;
            _initialDistanceBetweenTrains = DistanceBetweenTrains();
        }

        private void Start()
        {
            if (startSimulationOnAwake)
            {
                StartSimulation();
            }
        }

        public void StartSimulation()
        {
            ConfigureTrains();
        }

        public void RestartSimulation()
        {
            ResetTrainConfigurations();
            StartSimulation();
        }

        public void PauseSimulation()
        {
            trainA.enabled = false;
            trainB.enabled = false;
        }

        public void ResumeSimulation()
        {
            trainA.enabled = true;
            trainB.enabled = true;
        }

        private void ConfigureTrains()
        {
            {
                // Set up train A
                StateMachine machine = trainA.States;
                
                IState idle = new IdleState();
                IState accelerate = new AccelerationState(trainA);
                IState moving = new MaxSpeedMovementState(trainA);
                IState decelerate = new DecelerationState(trainA);
                
                machine.AddTransition(accelerate, moving, ReachedMaxVelocityA());
                machine.AddTransition(accelerate, decelerate, ReachedStoppingThreshold());
                machine.AddTransition(moving, decelerate, ReachedStoppingThreshold());
                machine.AddTransition(decelerate, idle, ReachedZeroVelocityA());
                
                machine.SetState(accelerate);
            }
            
            {
                // Set up train B
                StateMachine machine = trainB.States;
                
                IState idle = new IdleState();
                IState accelerate = new AccelerationState(trainB);
                IState moving = new MaxSpeedMovementState(trainB);
                IState decelerate = new DecelerationState(trainB);
                
                machine.AddTransition(accelerate, moving, ReachedMaxVelocityB());
                machine.AddTransition(accelerate, decelerate, ReachedStoppingThreshold());
                machine.AddTransition(moving, decelerate, ReachedStoppingThreshold());
                machine.AddTransition(decelerate, idle, ReachedZeroVelocityB());
                
                machine.SetState(accelerate);
            }

            // Conditions

            StateTransitionCondition ReachedMaxVelocityA() => () => trainA.Speed >= trainA.MaxSpeed;
            StateTransitionCondition ReachedZeroVelocityA() => () => trainA.Speed <= 0.0f;
            StateTransitionCondition ReachedMaxVelocityB() => () => trainB.Speed >= trainB.MaxSpeed;
            StateTransitionCondition ReachedZeroVelocityB() => () => trainB.Speed <= 0.0f;
            StateTransitionCondition ReachedStoppingThreshold() => () => DistanceBetweenTrains() - (safetyStoppingThreshold + Train.Width) <=
                                                                         GetDecelerationDistance(trainA) + GetDecelerationDistance(trainB);
        }
        
        private void ResetTrainConfigurations()
        {
            trainA.States.Clear();
            trainB.States.Clear();
            
            trainA.transform.position = _initialPositionA;
            trainB.transform.position = _initialPositionB;

            trainA.Speed = 0;
            trainB.Speed = 0;
            
            trainA.enabled = true;
            trainB.enabled = true;
        }

        private float DistanceBetweenTrains()
        {
            return trainB.transform.position.x - trainA.transform.position.x;
        }

        // From zero velocity, the maximum distance it can travel until reaching it's max velocity
        private float GetAccelerationMaxTraveledDistance(Train train)
        {
            // Get t from the equation: V = Vo + a * t
            float timeTaken = train.MaxSpeed / train.Acceleration;
            
            // Get P from equation: P = Po + Vo * t + 1/2 * a * t * t
            float traveledDistance = 0.5f * train.Acceleration * Mathf.Pow(timeTaken, 2);
            
            return traveledDistance;
        }

        private float GetDecelerationMaxTraveledDistance(Train train)
        {
            // Get t from the equation: V = Vo + a * t (it assumes deceleration is a negative number)
            float timeTaken = Mathf.Abs(train.MaxSpeed / train.Deceleration);
            
            // Get P from equation: P = Po + Vo * t + 1/2 * a * t * t
            float traveledDistance = (train.MaxSpeed * timeTaken) + (0.5f * train.Deceleration * Mathf.Pow(timeTaken, 2));
            
            return traveledDistance;
        }
        
        // Get distance that the train would travel with it's current speed
        private float GetDecelerationDistance(Train train)
        {
            // Get t from the equation: V = Vo + a * t (it assumes deceleration is a negative number)
            float timeTaken = Mathf.Abs(train.Speed / train.Deceleration);
            
            // Get P from equation: P = Po + Vo * t + 1/2 * a * t * t
            float traveledDistance = (train.Speed * timeTaken) + (0.5f * train.Deceleration * Mathf.Pow(timeTaken, 2));
            
            return traveledDistance;
        }
    }
}

