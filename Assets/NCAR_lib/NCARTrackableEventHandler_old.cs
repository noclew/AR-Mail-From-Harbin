/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class NCARTrackableEventHandler_old : MonoBehaviour,
                                                ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES

        private TrackableBehaviour mTrackableBehaviour;

        private ImageTargetBehaviour mImgTarget;
        bool isTracked = false;
        Vector3 ptTopLeft;
        Vector3 ptTopRight;
        Vector3 ptBottomLeft;
        Vector3 ptBottomRight;
        float targetHeight;
        float targetWidth;

        #endregion // PRIVATE_MEMBER_VARIABLES

        public Vector3 wTopLeft
        {
            get { return this.transform.TransformPoint(ptTopLeft); }
        }
        public Vector3 wTopRight
        {
            get { return this.transform.TransformPoint(ptTopRight); }
        }
        public Vector3 wBottomLeft
        {
            get { return this.transform.TransformPoint(ptBottomLeft); }
        }
        public Vector3 wBottomRight
        {
            get { return this.transform.TransformPoint(ptBottomRight); }
        }
        public float TargetWidth
        {
            get { return targetWidth; }
        }
        public float TargetHeight
        {
            get { return targetHeight; }
        }

        public bool IsTracked
        {
            get { return isTracked; }
        }


        #region UNTIY_MONOBEHAVIOUR_METHODS
        void Awake()
        {
            mImgTarget = GetComponent<ImageTargetBehaviour>();
        }

        void Start()
        {
            targetHeight = mImgTarget.GetSize()[1];
            targetWidth = mImgTarget.GetSize()[0];

            ptTopRight = new Vector3(0.5f * (targetWidth / targetHeight), 0, 0.5f);
            ptTopLeft = new Vector3(-0.5f * (targetWidth / targetHeight), 0, 0.5f);
            ptBottomRight = new Vector3(0.5f * (targetWidth / targetHeight), 0, -0.5f);
            ptBottomLeft = new Vector3(-0.5f * (targetWidth / targetHeight), 0, -0.5f);

            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS



        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
                isTracked = true;
            }
            else
            {
                OnTrackingLost();
                isTracked = false;
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        }


        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }

            //Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
        }

        #endregion // PRIVATE_METHODS
    }
}
