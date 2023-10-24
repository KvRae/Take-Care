using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace KvRaeScripts
{
    public class PhonePosition: MonoBehaviour
    {
    
        private Quaternion _rotation;
        private Position _position;
        private Vector3 _acceleration;

        public TextMeshProUGUI rotationText;
        public TextMeshProUGUI accelerationText;
    

        private void Start()
        {
            if (SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
            }
        }
    
        private void Update()
        {
            if (!SystemInfo.supportsGyroscope) return;
            _rotation = GyroToUnity(Input.gyro.attitude);
            _acceleration = Input.gyro.userAcceleration;
            
            rotationText.text = "Rotation: " + _rotation;
            accelerationText.text = "Acceleration: " + _acceleration;
        }
    
        private static Quaternion GyroToUnity(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }
    }
}
