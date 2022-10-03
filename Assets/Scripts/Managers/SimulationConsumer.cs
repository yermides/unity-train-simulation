using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace SGS
{
    // This class is used to link key presses to functions of the Simulation Facade
    public class SimulationConsumer : MonoBehaviour
    {
        [SerializeField] private KeyCode[] keys;
        [SerializeField] private UnityEvent[] actions;
        private Dictionary<KeyCode, UnityEvent> _keysToEventsDictionary;

        private void Awake()
        {
            // Dictionaries can't be serialized, so two arrays are needed to construct one
            Assert.IsTrue(keys.Length == actions.Length);
            
            _keysToEventsDictionary = new Dictionary<KeyCode, UnityEvent>();

            for(int i = 0; i < keys.Length; i++)
            {
                _keysToEventsDictionary.Add(keys[i], actions[i]);
            }
        }

        private void Update()
        {
            foreach (var key in keys)
            {
                if (Input.GetKeyDown(key) && _keysToEventsDictionary.TryGetValue(key, out var action))
                {
                    action.Invoke();
                }
            }
        }
    }
}