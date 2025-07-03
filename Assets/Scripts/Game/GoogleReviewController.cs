using System;
using System.Collections;
using Google.Play.Review;
using UnityEngine;

namespace Game
{
    public class GoogleReviewController : MonoBehaviour
    {
        #region singleton

        public static GoogleReviewController Instance { get; set; }

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
        
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;

        public void Init()
        {
            _reviewManager = new ReviewManager();
        }

        public IEnumerator RequestReview()
        {
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();
            
            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using launchFlowOperation.Error.ToString().
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
        }
        
        
    }
}