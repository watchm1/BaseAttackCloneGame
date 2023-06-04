using UnityEngine;

namespace AttackMechanic
{
    public class WeaponSpawner
    {
        private WeaponData _weaponData;

        public WeaponSpawner(WeaponData data)
        {
            this._weaponData = data;
        }

        public void SpawnWeapon(Camera main)
        {
            var weapon = new GameObject(this._weaponData.weaponName);
            var weaponController = weapon.AddComponent<WeaponController>();
            weaponController.InjectData(_weaponData);
            if (Camera.main != null)
            {
                var transform = main.transform;
                weapon.transform.parent = transform;
                weapon.transform.position = transform.position;
            }
            Debug.Log("WeaponController has been created Successfully");
        }
    }
}
