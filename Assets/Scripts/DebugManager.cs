using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    static bool debugManagerEnabled = true;

    public static void Log(string log)
    {
#if UNITY_EDITOR
        if (debugManagerEnabled)
        {
            Debug.Log(log);
        }
#endif
    }
}
