#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColliderToFit : MonoBehaviour
{
    [MenuItem("My Tools/Collider/print bounds")]
    static void tellMeBoudns()
    {
        Collider m_Collider;
        Vector3 m_Center;
        Vector3 m_Size, m_Min, m_Max;
        //Fetch the Collider from the GameObject
        m_Collider = Selection.gameObjects[0].GetComponent<Collider>();
        //Fetch the center of the Collider volume
        m_Center = m_Collider.bounds.center;
        //Fetch the size of the Collider volume
        m_Size = m_Collider.bounds.size;
        //Fetch the minimum and maximum bounds of the Collider volume
        m_Min = m_Collider.bounds.min;
        m_Max = m_Collider.bounds.max;
        //Output this data into the console
        Debug.Log("Collider Center : " + m_Center);
        Debug.Log("Collider Size : " + m_Size);
        Debug.Log("Collider bound Minimum : " + m_Min);
        Debug.Log("Collider bound Maximum : " + m_Max);
    }



    [MenuItem("My Tools/Collider/Fit to Children test")]
    static void addBoxCollider()
    {
        GameObject obj = Selection.gameObjects[0];
        Renderer thisRenderer = obj.gameObject.transform.GetComponent<Renderer>();
        Renderer[] allRenderers = obj.GetComponentsInChildren<Renderer>();

        if (allRenderers == null) return;

        else
        {
            Bounds newBounds = new Bounds( allRenderers[0].bounds.center, allRenderers[0].bounds.size);
            
            foreach(Renderer childRenderer in allRenderers)
            {
                newBounds.Encapsulate(childRenderer.bounds);
            }
            BoxCollider boxCol = obj.GetComponent(typeof(BoxCollider)) as BoxCollider;
            if (boxCol == null)
                boxCol = obj.gameObject.AddComponent<BoxCollider>();

            //below is needed because collider.center is based on the fucking local space while bounds are global.
            boxCol.center = obj.transform.InverseTransformPoint (newBounds.center);
            boxCol.size = obj.transform.InverseTransformVector( newBounds.size );
  
        }

        //BoxCollider boxCol = obj.GetComponent(typeof(BoxCollider)) as BoxCollider;
        //if (boxCol == null)
        //    boxCol = obj.gameObject.AddComponent<BoxCollider>();

        //Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        //Renderer thisRenderer = obj.gameObject.transform.GetComponent<Renderer>();
        //if (thisRenderer != null)
        //    bounds.Encapsulate(thisRenderer.bounds);

        //bounds.center = boxCol.bounds.center;
        //bounds.size = boxCol.bounds.size;

        //Component[] allDescendants = selected.gameObject.GetComponentsInChildren<Transform>();
        //foreach (Transform desc in allDescendants)
        //{
        //    Renderer childRenderer = desc.GetComponent<Renderer>();
        //    if (childRenderer != null)
        //    {
        //        //Bounds childbounds = new Bounds(Vector3.zero, Vector3.zero);
        //        //childbounds.size = (childRenderer.bounds)
        //        Bounds cBounds = childRenderer.bounds;
        //        bounds.Encapsulate(childRenderer.bounds);
        //        print("center: " + cBounds.center);
        //        print("size: "+ cBounds.size);
        //    }

        //}
        //boxCol.size = selected.TransformVector( bounds.size);

    }

    static void printBounds(Bounds b)
    {
        print(">> center: " + b.center);
        print(">> size: "+ b.size);
    }

    [MenuItem("My Tools/Collider/Fit to Children")]
    static void FitToChildren()
    {
        //foreach (GameObject rootGameObject in Selection.gameObjects)
        //{
        //    if (!(rootGameObject.GetComponent<Collider>() is BoxCollider))
        //        continue;

        //    bool hasBounds = false;
        //    Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        //    for (int i = 0; i < rootGameObject.transform.childCount; ++i)
        //    {
        //        Renderer childRenderer = rootGameObject.transform.GetChild(i).GetComponent<Renderer>();
        //        if (childRenderer != null)
        //        {
        //            if (hasBounds)
        //            {
        //                bounds.Encapsulate(childRenderer.bounds);
        //            }
        //            else
        //            {
        //                bounds = childRenderer.bounds;
        //                hasBounds = true;
        //            }
        //        }
        //    }

        //    BoxCollider collider = (BoxCollider)rootGameObject.GetComponent<Collider>();
        //    collider.center = bounds.center - rootGameObject.transform.position;
        //    collider.size = bounds.size;
        //}


        Transform selected = Selection.gameObjects[0].transform;
        BoxCollider collider = (BoxCollider)selected.GetComponent<Collider>();

        Debug.Log(collider.center); //bounds center is global; collider center is local

        Vector3[] bb;
        bb = getBounds(selected);

        bb[0] = selected.InverseTransformPoint(bb[0]);
        bb[1] = selected.InverseTransformPoint(bb[1]);

        collider.center = (bb[0] + bb[1]) / 2;
        collider.size = bb[0] - bb[1];



    }

    static Vector3[] getBounds(Transform trans)
    {
        //Bounds bound = trans.GetComponent<Renderer>().bounds;

        if (trans.childCount == 0)
        {
            //Debug.Log(trans.name);
            if (trans.GetComponent<Renderer>() == null)
            {
                return null;
            }
            else
            {
                Bounds bound = trans.GetComponent<Renderer>().bounds;
                Vector3[] res = { bound.min, bound.max }; //return global
                return res;
            }

        }


        else
        {
            Vector3 newMin = Vector3.one * Mathf.Infinity;
            Vector3 newMax = Vector3.one * -Mathf.Infinity;

            foreach (Transform childTrans in trans)
            {
                //if (childTrans.GetComponent<Renderer>() == null) continue;
                Vector3[] childMinMax = getBounds(childTrans);

                Vector3 curMin;
                Vector3 curMax;

                if (childMinMax == null && trans.GetComponent<Renderer>() != null)
                {
                    newMin = (trans.GetComponent<Renderer>().bounds.min);
                    newMax = (trans.GetComponent<Renderer>().bounds.max);
                    continue;
                }

                else if (childMinMax == null && trans.GetComponent<Renderer>() == null)
                {
                    continue;
                }

                else if (childMinMax != null && trans.GetComponent<Renderer>() == null)
                {
                    curMin = newMin;
                    curMax = newMax;
                }


                else
                {
                    curMin = (trans.GetComponent<Renderer>().bounds.min);
                    curMax = (trans.GetComponent<Renderer>().bounds.max);
                }

                newMin.x = curMin.x < childMinMax[0].x ? curMin.x : childMinMax[0].x;
                newMin.y = curMin.y < childMinMax[0].y ? curMin.y : childMinMax[0].y;
                newMin.z = curMin.z < childMinMax[0].z ? curMin.z : childMinMax[0].z;

                newMax.x = curMax.x > childMinMax[1].x ? curMax.x : childMinMax[1].x;
                newMax.y = curMax.y > childMinMax[1].y ? curMax.y : childMinMax[1].y;
                newMax.z = curMax.z > childMinMax[1].z ? curMax.z : childMinMax[1].z;


            }
            return new Vector3[2] { newMin, newMax };
        }
    }

}
#endif