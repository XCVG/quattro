using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonCore;
using CommonCore.World;

/// <summary>
/// Hacky script to "billboard" a sprite by rotating it to face the player
/// </summary>
public class BillboardHack : MonoBehaviour
{

    [SerializeField]
    private bool Reverse = true;

    void Update()
    {
        var targetTransform = WorldUtils.GetPlayerObject().transform;

        Vector3 vecToTarget = targetTransform.position - transform.position;
        var flatVecToTarget = new Vector3(vecToTarget.x, 0, vecToTarget.z).normalized;
        var quatToTarget = Quaternion.LookRotation(flatVecToTarget);
        if (Reverse)
            quatToTarget *= Quaternion.AngleAxis(180, Vector3.up);
        transform.rotation = quatToTarget;
    }
}
