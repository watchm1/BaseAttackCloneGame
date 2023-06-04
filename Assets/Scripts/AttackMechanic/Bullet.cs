using System;
using JetBrains.Annotations;
using MeshManipulation;
using UnityEngine;

namespace AttackMechanic
{
    public class Bullet : MonoBehaviour
    {
        public bool canDestructObjects;
        [CanBeNull] public ParticleSystem particleEffect;
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out MeshDestroy meshDestroyer))
            {
                if(canDestructObjects)
                    meshDestroyer.DestroyMesh();
            }

            if (other.gameObject.CompareTag($"Enemy"))
            {
                // todo:: access enemy and decrease healing and configure animations
            }

            DestroySelf();
        }

        private void DestroySelf()
        {
            // todo:: make particle effect
            if (particleEffect != null) particleEffect.Play();
            Destroy(gameObject);
        }
    }
}
