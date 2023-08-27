using UnityEngine;
using UnityEngine.Pool;

namespace Maggots
{
    public class FXObjectsPool : MonoBehaviour
    {
        public static FXObjectsPool Instance;

        [SerializeField] private ParticleSystemPoolElement explosionFXPrefab;
        [SerializeField] int explosionStartSizePool = 20;

        private ObjectPool<ParticleSystemPoolElement> explosionParticles;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayFXParticle(FXType type, Vector2 position, Vector2 scale)
        {
            switch (type)
            {
                case FXType.Explosion:
                    var particle = explosionParticles.Get();
                    particle.gameObject.transform.position = position;
                    particle.gameObject.transform.localScale = scale;
                    break;
            }
        }

        public enum FXType
        {
            Explosion
        }

        private void Start()
        {
            explosionParticles = new ObjectPool<ParticleSystemPoolElement>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, explosionStartSizePool, 40);
        }

        private ParticleSystemPoolElement CreatePooledItem()
        {
            ParticleSystemPoolElement poolElement = Instantiate(explosionFXPrefab);
            poolElement.transform.parent = transform;
            poolElement.Init(explosionParticles);
            return poolElement;
        }
        private void OnReturnedToPool(ParticleSystemPoolElement system)
        {
            system.gameObject.SetActive(false);
        }

        private void OnTakeFromPool(ParticleSystemPoolElement system)
        {
            system.gameObject.SetActive(true);
        }

        private void OnDestroyPoolObject(ParticleSystemPoolElement system)
        {
            Destroy(system.gameObject);
        }
    }
}

