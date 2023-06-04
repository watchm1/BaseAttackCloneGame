using System;
using System.Collections;
using UnityEngine;

namespace AttackMechanic
{
    public class WeaponController : MonoBehaviour
    {
        private WeaponData _currentWeapon;
        private Vector3 _direction;
        private int _counter;
        public bool isReloading;
        public IEnumerator Shoot(Vector3 direction)
        {
            if (_counter > 0)
            {
                // todo:: setting direction and shoot
                // info direction will be where client touch on the screen point; 
                // running this function with event system;
                // use Instantie method but the bullet when touch to anything on the scene. the bullet will destroy;
                var obj = Instantiate(_currentWeapon.bullet.bulletPrefab);
                obj.transform.position = transform.position;
                obj.AddComponent<Rigidbody>().AddForce( direction * (_currentWeapon.bullet.bulletSpeed ), ForceMode.VelocityChange);
                _counter--;    
            }
            else
            {
                isReloading = true;
            }
            yield return new WaitForSeconds(_currentWeapon.shootDelay);

        }

        private IEnumerator ReloadGun()
        {
            Debug.Log("Reloading");
            yield return new WaitForSeconds(_currentWeapon.shootDelay * 2);
            isReloading = false;
            _counter = _currentWeapon.chargerCapacity;
        }
        
        public void InjectData(WeaponData data)
        {
            _currentWeapon = data;
            Debug.Log("Data injected");
            _counter = _currentWeapon.chargerCapacity;
            isReloading = false;
        }
        private void Start()
        {
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (Camera.main == null) return;
            if (!isReloading)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = 10f;
                var worldMousePoint = Camera.main.ScreenToWorldPoint(mousePos);
                var direction = (worldMousePoint - transform.position).normalized;
                StartCoroutine(Shoot(direction));
            }
            else
                StartCoroutine(ReloadGun());

        }
    }
}
