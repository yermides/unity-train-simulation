using System;
using UnityEngine;

namespace SGS
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform objectToFollow;
        [SerializeField] private Vector3 _offsetToTarget;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            // _offsetToTarget = objectToFollow.position - _transform.position;
        }

        private void LateUpdate()
        {
            _transform.position = objectToFollow.position - _offsetToTarget;
        }
    }
}