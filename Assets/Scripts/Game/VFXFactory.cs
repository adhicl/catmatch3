using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

    public class VFXFactory : MonoBehaviour
    {
        #region singleton

        public static VFXFactory Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        #endregion
        
        [SerializeField] GameObject smokePrefab;
        [SerializeField] GameObject trailPrefab;
        [SerializeField] GameObject splashPrefab;
        
        private List<Transform> smokeLists = new List<Transform>();
        private List<Transform> trailLists = new List<Transform>();
        private List<Transform> splashLists = new List<Transform>();

        private Vector2 hidePosition = new Vector2(-15f, 0f);

        #region smoke
        //return new smoke object
        public Transform GetSmokeObject()
        {
            Transform smoke;
            if (smokeLists.Count == 0)
            {
                smoke = Instantiate(smokePrefab, transform).GetComponent<Transform>();
            }
            else
            {
                smoke = smokeLists[0];
                smokeLists.RemoveAt(0);
            }
            smoke.GetComponent<SmokeVFX>().Init();
            smoke.GetComponent<ParticleSystem>().Play();
            return smoke;
        }
        public void ReturnSmokeObject(Transform smoke)
        {
            smoke.transform.position = hidePosition;
            smoke.GetComponent<ParticleSystem>().Stop();
            smokeLists.Add(smoke);
        }
        
        #endregion
        
        #region trail
        //return new trail object
        public Transform GetTrailObject()
        {
            Transform trail;
            if (trailLists.Count == 0)
            {
                trail = Instantiate(trailPrefab, transform).GetComponent<Transform>();
            }
            else
            {
                trail = trailLists[0];
                trailLists.RemoveAt(0);
                trail.gameObject.SetActive(true);
            }

            trail.GetComponent<TrailVFX>().Init();
            return trail;
        }
        
        public void ReturnTrailObject(Transform trail)
        {
            trail.gameObject.SetActive(false);
            trail.transform.position = hidePosition;
            trailLists.Add(trail);
        }
        
        #endregion
        
        #region splash
        //return new splash object
        public Transform GetSplashObject()
        {
            Transform splash;
            if (splashLists.Count == 0)
            {
                splash = Instantiate(splashPrefab, transform).GetComponent<Transform>();
            }
            else
            {
                splash = splashLists[0];
                splashLists.RemoveAt(0);
                splash.gameObject.SetActive(true);
            }

            splash.GetComponent<SplashVFX>().Init();
            return splash;
        }
        
        public void ReturnSplashObject(Transform splash)
        {
            splash.gameObject.SetActive(false);
            splash.transform.position = hidePosition;
            splashLists.Add(splash);
        }
        
        #endregion

    }