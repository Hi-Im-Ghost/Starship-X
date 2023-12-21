using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TargetInfo 
{
    public static bool isTargetInRange(Vector3 position, Vector3 rayDirection, out RaycastHit hitInfo, float range)
    {
        return Physics.Raycast(position,rayDirection,out hitInfo,range);
    }

}
