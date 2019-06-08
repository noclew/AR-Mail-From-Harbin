using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NCARHelperTools
{
    public static void addBoxColliderEncapsulatingChildren(GameObject obj)
    {
        Renderer thisRenderer = obj.gameObject.transform.GetComponent<Renderer>();
        Renderer[] allRenderers = obj.GetComponentsInChildren<Renderer>();

        if (allRenderers == null) return;

        else
        {
            Bounds newBounds = new Bounds(allRenderers[0].bounds.center, allRenderers[0].bounds.size);

            foreach (Renderer childRenderer in allRenderers)
            {
                newBounds.Encapsulate(childRenderer.bounds);
            }
            BoxCollider boxCol = obj.GetComponent(typeof(BoxCollider)) as BoxCollider;
            if (boxCol == null)
                boxCol = obj.gameObject.AddComponent<BoxCollider>();

            //below is needed because collider.center is based on the fucking local space while bounds are global.
            boxCol.center = obj.transform.InverseTransformPoint(newBounds.center);
            boxCol.size = obj.transform.InverseTransformVector(newBounds.size);
        }
    }
}
