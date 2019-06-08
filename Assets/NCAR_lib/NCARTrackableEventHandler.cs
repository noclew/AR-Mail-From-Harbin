/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class NCARTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{
    #region PROTECTED_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;
    protected TrackableBehaviour.Status m_PreviousStatus;
    protected TrackableBehaviour.Status m_NewStatus;

    #endregion // PROTECTED_MEMBER_VARIABLES

    #region Noclew_MEMBER_VARIABLES

    private ImageTargetBehaviour mImgTarget;
    public bool isBeingTracked { get; set;}
    Vector3 ptTopLeft;
    Vector3 ptTopRight;
    Vector3 ptBottomLeft;
    Vector3 ptBottomRight;
    float targetHeight;
    float targetWidth;


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
        get { return isBeingTracked; }
    }
    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS

    void Awake()
    {
        mImgTarget = GetComponent<ImageTargetBehaviour>();
        isBeingTracked = false;
    }

    protected virtual void Start()
    {
        //------------------------------ncCode
        targetHeight = mImgTarget.GetSize()[1];
        targetWidth = mImgTarget.GetSize()[0];

        ptTopRight = new Vector3(0.5f * (targetWidth / targetHeight), 0, 0.5f);
        ptTopLeft = new Vector3(-0.5f * (targetWidth / targetHeight), 0, 0.5f);
        ptBottomRight = new Vector3(0.5f * (targetWidth / targetHeight), 0, -0.5f);
        ptBottomLeft = new Vector3(-0.5f * (targetWidth / targetHeight), 0, -0.5f);
        //------------------------------

        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
    }

    protected virtual void OnDestroy()
    {
        if (mTrackableBehaviour)
            mTrackableBehaviour.UnregisterTrackableEventHandler(this);
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        m_PreviousStatus = previousStatus;
        m_NewStatus = newStatus;

        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            OnTrackingFound();
            isBeingTracked = true;
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NO_POSE)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
            isBeingTracked = false;
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
            isBeingTracked = false;
        }
    }

    #endregion // PUBLIC_METHODS

    #region PROTECTED_METHODS

    protected virtual void OnTrackingFound()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            component.enabled = true;

        // Enable canvas':
        foreach (var component in canvasComponents)
            component.enabled = true;
    }


    protected virtual void OnTrackingLost()
    {
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
    }

    #endregion // PROTECTED_METHODS
}
