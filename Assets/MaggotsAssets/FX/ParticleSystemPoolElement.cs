using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Maggots
{
    public class ParticleSystemPoolElement : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> systems;
        [SerializeField] private float lifeTime = 1f;

        private IObjectPool<ParticleSystemPoolElement> pool;

        public void Init(IObjectPool<ParticleSystemPoolElement> pool)
        {
            this.pool = pool;
        }

        private void OnEnable()
        {
            systems.ForEach(s => s.Play());
            StartCoroutine(Finish());
        }

        private IEnumerator Finish()
        {
            yield return new WaitForSeconds(lifeTime);
            pool.Release(this);
        }
    }
}

