using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransfomDeepChildExtension
{
    public static Transform FindDeepChild(this Transform transform, string name)
    {
        Transform result;

        result = transform.Find(name);
        if (result != null)
        {
            return result;
        }

        foreach (Transform child in transform)
        {
            result = child.FindDeepChild(name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}