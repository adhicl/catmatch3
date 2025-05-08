using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SplashVFX : MonoBehaviour
    {
        private float timer = 0f;
        private ParticleSystem _particleSystem;

        public void Init()
        {
            timer = 0f;
            if (!_particleSystem)
            {
                _particleSystem = this.GetComponent<ParticleSystem>();
            }
            _particleSystem.Play();
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= .5f)
            {
                VFXFactory.Instance.ReturnSplashObject(this.transform);
            }
        }
    }
}