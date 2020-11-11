using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlockingSettings", menuName = "ScriptableObjects/FlockingSettings", order = 1)]
public class FlockingSetting : ScriptableObject
{
    [Header("Agent Settings")]
    [Tooltip("Agent view radius")] public float VIEW_RADIUS = 10f;
    [Tooltip("Agent view half angle")] public float VIEW_HALF_AGL = 30f;
    [Tooltip("Agent avoidance radius")] public float AVO_RADIUS = 2f;
    [Tooltip("Agent stay radius")] public float STY_RADIUS = 20f;
    [Tooltip("Agent collision detection radius")] public float COL_DET_RAD = 2f;
    [MinMaxSlider(0, 10)] public Vector2 SPD_LMT;
    [Header("Flocking Weights")]
    [Tooltip("Agent cohesion weight")] public float COH_WGT = 3f;
    [Tooltip("Agent alignment weight")] public float ALI_WGT = 2f;
    [Tooltip("Agent avoidance weight")] public float AVO_WGT = 2f;
    [Tooltip("Agent stay weight")] public float STY_WGT = 4f;
    [Tooltip("Agent collision avoidance weight")] public float COL_WGT = 10f;
    [Tooltip("Agent damping weight")] public float DAMP_WGT = 2f;
}
