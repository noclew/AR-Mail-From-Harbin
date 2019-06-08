using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class NcMenuItems : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [MenuItem("ncHelpers/reset previous version")]
    static void checkversion()
    {
        NCVersioning versioningObj = GetVersioningObject("NcVersioingObject");

        if (versioningObj != null)
        {
            versioningObj.ResetPreviousVersion();
        }
    }

    [MenuItem("ncHelpers/Set Versions")]
    static void setVersionsToVersioningObject()
    {
        print(Application.unityVersion);
        NCVersioning vs = GetVersioningObject("NcVersioingObject");

        if (vs != null)
        {
            if (vs.m_majorVersion != vs.m_majorVersion_p || vs.m_minorVersion != vs.m_minorVersion_p)
            {
                vs.m_majorVersion_p = vs.m_majorVersion;
                vs.m_minorVersion_p = vs.m_minorVersion;
                vs.m_buildNumber = 1;
            }

            vs.SetVersions();

            PlayerSettings.bundleVersion = "" + vs.m_majorVersion + "." + vs.m_minorVersion + "." + vs.m_buildNumber;
            PlayerSettings.Android.bundleVersionCode = int.Parse( vs.m_buildCode) ;
            PlayerSettings.iOS.buildNumber = vs.m_buildCode;
        }
    }

    static NCVersioning GetVersioningObject(string objectName)
    {
        string filter = objectName;
        string[] guids = AssetDatabase.FindAssets(objectName, null);
  
        if (guids.Length != 1)
        {
            string msg = "you have " + guids.Length + " versioning objects. The name must be NcVersioingObject.";
            EditorUtility.DisplayDialog("Error", msg, "Got it");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        print(path);
        NCVersioning versioningObj = (NCVersioning)AssetDatabase.LoadAssetAtPath(path, typeof(NCVersioning));
        return versioningObj;
    }

    [PostProcessBuildAttribute(0)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        NCVersioning vs = GetVersioningObject("NcVersioingObject");
        if (vs != null)
        { 
            vs.m_buildNumber += 1;  
        }

    }
}
