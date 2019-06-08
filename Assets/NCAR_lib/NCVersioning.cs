using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "Nc Scriptable Ojbect/VersioningObject", order = 1)]
public class NCVersioning : ScriptableObject
{
    [Header("App Version Info")]
    [SerializeField]
    public int m_majorVersion;
    public int m_majorVersion_p { get; set; }

    [SerializeField]
    public int m_minorVersion;
    public int m_minorVersion_p { get; set; }

    [SerializeField]
    public int m_buildNumber;

    [SerializeField]
    public string m_buildCode;


    [Header("IDE Version Info")]
    [SerializeField]
    public string m_unityVersion;

    [SerializeField]
    public string m_vuforiaVersion;

    [SerializeField]
    public string m_ArCoreVersion;

    [SerializeField]
    public string m_ArKitVersion;

    [Header("Extra Info")]
    [SerializeField]
    public string m_note;

    public void SetVersions()
    {
        if (m_majorVersion_p == 0) m_majorVersion_p = m_majorVersion;
        if (m_minorVersion_p == 0) m_minorVersion_p = m_minorVersion;

        Debug.Log("current version: " + m_majorVersion + "." + m_minorVersion + "." + m_buildNumber);
        Debug.Log("previous version: " + m_majorVersion_p + "." + m_minorVersion_p );

        m_unityVersion = Application.unityVersion;
        m_vuforiaVersion = Vuforia.VuforiaUnity.GetVuforiaLibraryVersion();
        m_buildCode = (m_majorVersion * 1000000 + m_minorVersion * 1000 + m_buildNumber).ToString();


        Debug.Log(m_buildCode);
    }

    public void ResetPreviousVersion()
    {
        m_majorVersion_p = 0;
        m_minorVersion_p = 0;
    }

}


