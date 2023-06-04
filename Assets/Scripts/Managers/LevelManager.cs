using System;
using AttackMechanic;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private WeaponData _weaponData;
        private WeaponSpawner _weaponSpawner;

        private void Start()
        {
            _weaponSpawner = new WeaponSpawner(_weaponData);
            _weaponSpawner.SpawnWeapon(Camera.main);
        }
    }
}
