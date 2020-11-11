using UnityEngine;
public class DEF : MonoBehaviour{ 
    public static DEF Instance {get; private set;} = null;
    private void Awake() {
        Instance = this;
    }
    [Header("Flocking Settings")]
    public bool UseSetting;
    [SerializeField]
    private FlockingSetting m_setting;
    public const float EPS = 0.000001f;
    public float VIEW_RADIUS_SQR => VIEW_RADIUS * VIEW_RADIUS;
    public float AVO_RADIUS_SQR => AVO_RADIUS * AVO_RADIUS;
    public float VIEW_RADIUS => UseSetting ? m_setting.VIEW_RADIUS : ViewRadius;
    public float VIEW_HALF_AGL => UseSetting ? m_setting.VIEW_HALF_AGL : ViewHalfAngle;
    public float AVO_RADIUS => UseSetting ? m_setting.AVO_RADIUS : AvoidanceRadius;
    public float STY_RADIUS => UseSetting ? m_setting.STY_RADIUS : StayRadius;
    public float COL_DET_RAD => UseSetting ? m_setting.COL_DET_RAD : CollisionDetectionRadius;
    public Vector2 SPD_LMT => UseSetting ? m_setting.SPD_LMT : SpeedLimit;
    public float COH_WGT => UseSetting ? m_setting.COH_WGT : CohesionWeight;
    public float ALI_WGT => UseSetting ? m_setting.ALI_WGT : AlignmentWeight;
    public float AVO_WGT => UseSetting ? m_setting.AVO_WGT : AvoidanceRadius;
    public float STY_WGT => UseSetting ? m_setting.STY_WGT : StayWeight;
    public float COL_WGT => UseSetting ? m_setting.COL_WGT : CollisionWeight;
    public float DAMP_WGT => UseSetting ? m_setting.DAMP_WGT : DampingWeight;
    [Header("Agent Settings")]
    [Tooltip("Agent view radius")] public float ViewRadius = 10f;
    [Tooltip("Agent view half angle")] public float ViewHalfAngle = 30f;
    [Tooltip("Agent avoidance radius")] public float AvoidanceRadius = 5f;
    [Tooltip("Agent stay radius")] public float StayRadius = 50f;
    [Tooltip("Agent collision detection radius")] public float CollisionDetectionRadius = 2f;
    [MinMaxSlider(0, 10)] public Vector2 SpeedLimit;
    [Header("Flocking Weights")]
    [Tooltip("Agent cohesion weight")] public float CohesionWeight = 3f;
    [Tooltip("Agent alignment weight")] public float AlignmentWeight = 2f;
    [Tooltip("Agent avoidance weight")] public float AvoidanceWeight = 2f;
    [Tooltip("Agent stay weight")] public float StayWeight = 4f;
    [Tooltip("Agent collision avoidance weight")] public float CollisionWeight = 10f;
    [Tooltip("Agent damping weight")] public float DampingWeight = 2f;
    [Header("Octree Settings")]
    [Tooltip("Octree size")] public float OCT_LTH = 10f;
    [Tooltip("Minimum octree node size")] public float MIN_OCT_NODE = 1f;
    [Header("Debug tool")]
    [Tooltip("If draw debug line")] public bool DEBUG = false;
    [Tooltip("if use octree to calculate agents nearby")] public bool OCTREE = false;
}