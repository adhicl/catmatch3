using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class SmokeVFX : MonoBehaviour
    {
        private float timer = 0f;

        public void Init()
        {
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 0.2f)
            {
                VFXFactory.Instance.ReturnSmokeObject(this.transform);
            }
        }
    }
}