using System;
using UnityEngine;

namespace Movements
{
    public class CamMovementController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [SerializeField] private float radius;

        [SerializeField] private float speed;

        private float _angle = 0f;
        // Start is called before the first frame update
        private void Start()
        {
            
        }

        private void MoveSelf()
        {
            _angle += speed * Time.deltaTime;
            var position = target.position;
            float x = position.x + Mathf.Cos(_angle) * radius;
            float z = position.z + Mathf.Sin(_angle) * radius;
            transform.position = new Vector3(x * -1, transform.position.y, z);
            transform.LookAt(target);
        }

        // Update is called once per frame
        [Obsolete("Obsolete")]
        private void Update()
        {
            MoveSelf();
        }
    }
}
