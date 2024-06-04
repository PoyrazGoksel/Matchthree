using UnityEngine;

namespace Extensions.Unity
{
    public class ETransform
    {
        public Vector3 position => _transform.position;
        public Quaternion rotation => _transform.rotation;
        public Vector3 eulerAngles => _transform.eulerAngles;
        public Vector3 localPosition => _transform.localPosition;
        public Quaternion localRotation => _transform.localRotation;
        public Vector3 localEulerAngles => _transform.localEulerAngles;
        
        private readonly Transform _transform;
        
        public ETransform(Transform transform)
        {
            _transform = transform;
        }

        public Transform GetTransIns()
        {
            return _transform;
        }
        
        public void Update(Transform transform)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}