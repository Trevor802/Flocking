using UnityEngine;
public class DEF : MonoBehaviour{ 
    public static DEF Instance {get; private set;} = null;
    private void Awake() {
        Instance = this;
        SelectFlockingSetting(1);
    }
    [Header("Flocking Settings")]
    public bool UseSetting;
    public FlockingSetting Chaotic;
    public FlockingSetting Neutral;
    public FlockingSetting Lawful;
    public void SelectFlockingSetting(int id){
        FlockingSetting setting = null;
        switch(id){
            case 0:
                setting = Chaotic;
                break;
            case 1:
                setting = Neutral;
                break;
            case 2:
                setting = Lawful;
                break;
            default:
                setting = Neutral;
                break;
        }
        m_setting = setting;
    }
    private FlockingSetting m_setting;
    public const float EPS = 0.000001f;
    public const float D2R = Mathf.PI / 180f;
    public float VIEW_RADIUS_SQR => VIEW_RADIUS * VIEW_RADIUS;
    public float AVO_RADIUS_SQR => AVO_RADIUS * AVO_RADIUS;
    public float VIEW_RADIUS => UseSetting ? m_setting.VIEW_RADIUS : ViewRadius;
    public float VIEW_HALF_AGL_DEG => UseSetting ? m_setting.VIEW_HALF_AGL : ViewHalfAngle;
    public float AVO_RADIUS => UseSetting ? m_setting.AVO_RADIUS : AvoidanceRadius;
    public float STY_RADIUS => UseSetting ? m_setting.STY_RADIUS : StayRadius;
    public float COL_DET_RAD => UseSetting ? m_setting.COL_DET_RAD : CollisionDetectionRadius;
    public Vector2 SPD_LMT => SpeedLimit;
    public float COH_WGT => UseSetting ? m_setting.COH_WGT : CohesionWeight;
    public float ALI_WGT => UseSetting ? m_setting.ALI_WGT : AlignmentWeight;
    public float AVO_WGT => UseSetting ? m_setting.AVO_WGT : AvoidanceRadius;
    public float STY_WGT => UseSetting ? m_setting.STY_WGT : StayWeight;
    public float COL_WGT => UseSetting ? m_setting.COL_WGT : CollisionWeight;
    public float DAMP_WGT => DampingWeight;

    public float STY_SRT_RIT => UseSetting ? m_setting.StayStartRatio : StayStartRatio;

    [Header("Agent Settings")]
    [Tooltip("Agent view radius")] public float ViewRadius = 10f;
    [Tooltip("Agent view half angle")] public float ViewHalfAngle = 30f;
    [Tooltip("Agent avoidance radius")] public float AvoidanceRadius = 5f;
    [Tooltip("Agent stay radius")] public float StayRadius = 50f;
    [Tooltip("Agent stay start raito")] [Range(0.1f, 1f)] public float StayStartRatio = 0.9f;
    [Tooltip("Agent collision detection radius")] public float CollisionDetectionRadius = 2f;
    [Header("Flocking Weights")]
    [Tooltip("Agent cohesion weight")] [Range(0.1f, 5f)] public float CohesionWeight = 3f;
    [Tooltip("Agent alignment weight")] [Range(0.1f, 5f)] public float AlignmentWeight = 2f;
    [Tooltip("Agent avoidance weight")] [Range(0.1f, 5f)] public float AvoidanceWeight = 2f;
    [Tooltip("Agent stay weight")] [Range(0.1f, 5f)] public float StayWeight = 4f;
    [Tooltip("Agent collision avoidance weight")] [Range(1f, 10f)] public float CollisionWeight = 10f;
    [Header("Octree Settings")]
    [Tooltip("Octree size")] public float OCT_LTH = 10f;
    [Tooltip("Minimum octree node size")] public float MIN_OCT_NODE = 1f;
    [Header("Debug tool")]
    [Tooltip("If draw debug line")] public bool DEBUG = false;
    [Tooltip("if use octree to calculate agents nearby")] public bool OCTREE = false;
    [Header("Other Settings")]
    [MinMaxSlider(3, 20)] public Vector2 SpeedLimit;
    [Range(0f, 0.5f)] public float RandomWeight = 0.5f;
    [Tooltip("Agent damping weight")] [Range(0.1f, 10f)] public float DampingWeight = 2f;
}