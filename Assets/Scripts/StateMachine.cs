using System;
using System.Collections.Generic;

namespace SGS
{
    public class StateTransition
    {
        private IState _to;
        private Func<bool> _condition;

        public bool IsConditionMet
        {
            get
            {
                if (_condition != null)
                {
                    return _condition();
                }

                return false;
            }
        }

        public IState To => _to;

        public StateTransition(IState to, Func<bool> condition)
        {
            _to = to;
            _condition = condition;
        }
    }

    public class StateMachine
    {
        private static readonly List<StateTransition> EmptyTransitions = new List<StateTransition>(0);
        private readonly Dictionary<Type, List<StateTransition>> _transitions = new Dictionary<Type, List<StateTransition>>();
        private List<StateTransition> _currentTransitions = new List<StateTransition>();
        private readonly List<StateTransition> _anyTransitions = new List<StateTransition>();
        private IState _currentState;
        
        public void Tick()
        {
            var transition = GetTransition();

            if (transition != null)
            {
                SetState(transition.To);
            }

            _currentState?.Tick();
        }

        public void SetState(IState state)
        {
            if (state == _currentState)
            {
                return;
            }

            // _currentState?.OnExit();
            _currentState = state;
      
            _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
            
            if (_currentTransitions == null)
            {
                _currentTransitions = EmptyTransitions;
            }
            
            // _currentState.OnEnter();
        }

        public void AddTransition(IState from, IState to, Func<bool> predicate)
        {
            if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
            {
                transitions = new List<StateTransition>();
                _transitions[from.GetType()] = transitions;
            }
      
            transitions.Add(new StateTransition(to, predicate));
        }

        public void AddAnyTransition(IState state, Func<bool> predicate)
        {
            _anyTransitions.Add(new StateTransition(state, predicate));
        }
        
        private StateTransition GetTransition()
        {
            foreach (var transition in _anyTransitions)
            {
                if (transition.IsConditionMet)
                {
                    return transition;
                }
            }

            foreach (var transition in _currentTransitions)
            {
                if (transition.IsConditionMet)
                {
                    return transition;
                }
            }

            return null;
        }

        public void Clear()
        {
            _transitions.Clear();
            _currentTransitions.Clear();
            _anyTransitions.Clear();
            _currentState = null;
        }
    }
}