using UnityEngine;

namespace DefaultNamespace
{
    public class TrailVFX : MonoBehaviour
    {
        private float timer = 0f;

        public void Init()
        {
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 0.5f)
            {
                VFXFactory.Instance.ReturnTrailObject(this.transform);
            }
        }
        
    }
}