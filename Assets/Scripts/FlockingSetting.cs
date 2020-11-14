using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlockingSettings", menuName = "ScriptableObjects/FlockingSettings", order = 1)]
public class FlockingSetting : ScriptableObject
{
    [Header("Agent Settings")]
    [Tooltip("Agent view radius")] public float VIEW_RADIUS = 10f;
    [Tooltip("Agent view half angle")] public float VIEW_HALF_AGL = 30f;
    [Tooltip("Agent avoidance radius")] [Range(2f, 10f)] public float AVO_RADIUS = 2f;
    [Tooltip("Agent stay radius")] public float STY_RADIUS = 20f;
    [Tooltip("Agent stay start raito")] [Range(0.1f, 1f)] public float StayStartRatio = 0.9f;
    [Tooltip("Agent random moving weight")] [Range(0f, 1f)] public float RandomWeight = 0.5f;
    [Tooltip("Agent collision detection radius")] public float COL_DET_RAD = 2f;
    [Header("Flocking Weights")]
    [Tooltip("Agent cohesion weight")] [Range(0.1f, 5f)] public float COH_WGT = 3f;
    [Tooltip("Agent alignment weight")] [Range(0.1f, 5f)] public float ALI_WGT = 2f;
    [Tooltip("Agent avoidance weight")] [Range(0.1f, 5f)] public float AVO_WGT = 2f;
    [Tooltip("Agent stay weight")] [Range(0.1f, 5f)] public float STY_WGT = 4f;
    [Tooltip("Agent collision avoidance weight")] [Range(1f, 10f)] public float COL_WGT = 10f;
}
