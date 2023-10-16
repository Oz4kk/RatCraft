using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 Abs(this Vector3 value)
    {
        return new Vector3(Math.Abs(value.x), Math.Abs(value.y), Math.Abs(value.z));
    }

    public static string GetStringOfVector3(this Vector3 value)
    {
        return $"X = {value.x}, Y = {value.y}, Z = {value.z}";
    }
}
