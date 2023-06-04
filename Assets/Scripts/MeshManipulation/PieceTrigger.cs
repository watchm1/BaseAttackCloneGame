using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MeshManipulation
{
    public class PieceTrigger : MonoBehaviour
    {
        [FormerlySerializedAs("LifeTime")] public float lifeTime;
        IEnumerator DestroyGameObject()
        {
            yield return new WaitForSeconds(lifeTime);
            Destroy(gameObject);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Ground")) return;
            Debug.Log("Triggered");
            StartCoroutine(DestroyGameObject());
        }
    }
}
