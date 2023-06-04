using UnityEngine;
using UnityEngine.Serialization;

namespace AttackMechanic
{
    [System.Serializable]
    public struct BulletData
    {
        [FormerlySerializedAs("BulletPrefab")] public GameObject bulletPrefab;
        [FormerlySerializedAs("BulletDamage")] public float bulletDamage;
        [FormerlySerializedAs("BulletSpeed")] public float bulletSpeed;
        [FormerlySerializedAs("CanDestructObjects")] public bool canDestructObjects;
        [FormerlySerializedAs("ParticleEffect")] public ParticleSystem particleEffect;
    }

    [System.Serializable]
    public struct WeaponData
    {
        [FormerlySerializedAs("WeaponId")] public int weaponId;
        [FormerlySerializedAs("WeaponName")] public string weaponName;
        [FormerlySerializedAs("Bullet")] public BulletData bullet;
        [FormerlySerializedAs("ChargerCapacity")] public int chargerCapacity;
        public float shootDelay;
    }
}