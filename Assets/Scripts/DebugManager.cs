using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    static bool debugManagerEnabled = false;

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
