using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LoadingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tLoadingText;
        
        private void OnEnable()
        {
            StartCoroutine(AnimateLoadingText());
        }

        private void OnDisable()
        {
            StopCoroutine(AnimateLoadingText());
        }

        private IEnumerator AnimateLoadingText()
        {
            int dotCount = 0;
            string baseText = "Loading"; 
            while (true)
            {
                Debug.Log("Loading "+dotCount);
                // Update the text with the current number of dots
                tLoadingText.text = baseText + new string('.', dotCount);

                // Increment the dot count and reset if it exceeds 3
                dotCount = (dotCount + 1) % 4;

                // Wait for the specified animation speed
                yield return new WaitForSeconds(0.5f);
            }
        }
        
    }
}