using UnityEngine;
public class DEF : MonoBehaviour{ 
    public static DEF Instance {get; private set;} = null;
    private void Awake() {
        Instance = this;
    }
    public float VIEW_RADIUS = 10f;
    public float VIEW_RADIUS_SQR => VIEW_RADIUS * VIEW_RADIUS;
    public float VIEW_HALF_AGL = 30f;
    public float AVO_RADIUS = 2f;
    public float AVO_RADIUS_SQR => AVO_RADIUS * AVO_RADIUS;
    public float STY_RADIUS = 20f;
    public const float EPS = 0.000001f;
    [MinMaxSlider(0, 10)]
    public Vector2 SPD_LMT;
    public float COH_WGT = 3f;
    public float ALI_WGT = 2f;
    public float AVO_WGT = 2f;
    public float STY_WGT = 4f;
    public float COL_WGT = 10f;
    public float DAMP_WGT = 2f;
    public float COL_DET_RAD = 2f;
    public float OCT_LTH = 10f;
    public float MIN_OCT_NODE = 1f;
    public bool DEBUG = false;
    public bool OCTREE = false;
}