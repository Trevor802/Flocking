using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

public class MoveSystem : ComponentSystem{
    protected override void OnUpdate()
    {
        PointOctree<(float3, float3)> pointTree = null;
        if (DEF.Instance.OCTREE){
            pointTree = new PointOctree<(float3, float3)>(DEF.Instance.OCT_LTH, Vector3.zero, DEF.Instance.MIN_OCT_NODE);
            Entities.ForEach((ref Translation trans, ref MoveDirection dir) => {
            pointTree.Add((trans.Value, dir.Value), trans.Value.ToVector3());
            EntitySpawner.Instance.Tree = pointTree;
            });
        }
        // Conduct moving & rotating
        Entities.ForEach((ref Translation trans, ref MoveDirection direction, ref Rotation rotation) => {
            // Calculate direction
            float3 pos = trans.Value;
            float3 dir = direction.Value;
            float3 coh = float3.zero;
            float3 ali = float3.zero;
            float3 avo = float3.zero;
            float3 sty = float3.zero;
            float3 col = float3.zero;
            uint count = 0;
            uint avoCount = 0;
            // Golden spiral collision detection
            foreach(var f in BoidHelper.G_S_Dirs){
                float3 d = math.mul(rotation.Value, f);
                if (DEF.Instance.DEBUG){
                    Debug.DrawLine(pos.ToVector3(), pos.ToVector3() + d.ToVector3() * DEF.Instance.COL_DET_RAD, Color.red);
                }
                var pWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
                var cWorld = pWorld.PhysicsWorld.CollisionWorld;
                var raycast = new RaycastInput{
                    Start = pos,
                    End = pos + d * DEF.Instance.COL_DET_RAD,
                    Filter = new CollisionFilter{
                        BelongsTo = ~0u,
                        CollidesWith = 1 << 8, // Only detect layer 8
                        GroupIndex = 0,
                    }
                };
                RaycastHit hit;
                if (cWorld.CastRay(raycast, out hit)){ // Detected collision in this direction
                    // var e = pWorld.PhysicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                }
                else{ // Found a way out
                    col = d;
                    break; // early quit, that's indispensable
                }
            }
            if (DEF.Instance.OCTREE){
                var nodes = pointTree.GetNearby(pos.ToVector3(), DEF.Instance.VIEW_RADIUS);
                foreach(var n in nodes){
                    float3 dist = pos - n.Item1;
                    float distSqr = math.mul(dist, dist);
                    float halfAngle = math.acos(math.mul(dir, dist) / math.sqrt(distSqr));
                    if (distSqr < DEF.Instance.VIEW_RADIUS_SQR && distSqr > DEF.EPS && math.abs(halfAngle) < DEF.Instance.VIEW_HALF_AGL){
                        count++;
                        coh += n.Item1;
                        ali += n.Item2;
                        if (distSqr < DEF.Instance.AVO_RADIUS_SQR){
                            avoCount++;
                            avo += math.normalize(dist) * (DEF.Instance.AVO_RADIUS_SQR - distSqr);
                        }
                    }
                }
            }
            else{
            // In terms of a single agent
                Entities.ForEach((ref Translation t, ref MoveDirection d) => {
                    float3 dist = pos - t.Value;
                    float distSqr = math.mul(dist, dist);
                    float halfAngle = math.acos(math.mul(dir, dist) / math.sqrt(distSqr));
                    if (distSqr < DEF.Instance.VIEW_RADIUS_SQR && distSqr > DEF.EPS && math.abs(halfAngle) < DEF.Instance.VIEW_HALF_AGL){
                        count++;
                        coh += t.Value;
                        ali += d.Value;
                        if (distSqr < DEF.Instance.AVO_RADIUS_SQR){
                            avoCount++;
                            avo += math.normalize(dist) * (DEF.Instance.AVO_RADIUS_SQR - distSqr);
                        }
                    }
                });
            }
            if (count > 0){
                coh /= count;
                ali /= count;
            }
            if (avoCount > 0){
                avo /= avoCount;
            }
            coh -= trans.Value;
            float rSqr = math.mul(pos, pos);
            float p = rSqr / (DEF.Instance.STY_RADIUS * DEF.Instance.STY_RADIUS);
            if (p > 0.9f)
                sty = -math.normalizesafe(pos) * p;
            // Clamp
            coh = coh.Clamp(DEF.Instance.COH_WGT);
            ali = ali.Clamp(DEF.Instance.ALI_WGT);
            avo = avo.Clamp(DEF.Instance.AVO_WGT);
            sty = sty.Clamp(DEF.Instance.STY_WGT);
            col = col.Clamp(DEF.Instance.COL_WGT);
            // Accumulate
            float3 all = direction.Value * DEF.Instance.DAMP_WGT + coh + ali + avo + sty + col;
            all = all.Clamp(DEF.Instance.SPD_LMT.x, DEF.Instance.SPD_LMT.y);
            // Save direction for next loop
            direction.Value = math.normalize(all);
            // Look towards
            float3 fwd = direction.Value;
            float3 rt = new float3(fwd.z, 0, -fwd.x);
            float3 up = math.normalizesafe(math.cross(fwd, rt));
            rotation.Value = quaternion.LookRotation(fwd, up);
            // Execute movement
            trans.Value += all * Time.DeltaTime;
        });
    }
}

public static class MathExtension{
    public static float3 Clamp(this float3 value, float maxValue){
        if (math.mul(value, value) > maxValue * maxValue){
            return math.normalize(value) * maxValue;
        }
        return value;
    }
    public static float3 Clamp(this float3 value, float minValue, float maxValue){
        var length = math.mul(value, value);
        if (length < minValue * minValue){
            return math.normalize(value) * minValue;
        }
        if (length > maxValue * maxValue){
            return math.normalize(value) * maxValue;
        }
        return value;
    }

    public static Vector3 ToVector3(this float3 value){
        return new Vector3(value.x, value.y, value.z);
    }

}

public static class BoidHelper{
    private const int NUM_DIR = 20;
    public static readonly float3[] G_S_Dirs;
    static BoidHelper(){
        G_S_Dirs = new float3[NUM_DIR];
        float goldenRatio = (1 + math.sqrt(5)) / 2;
        float _1 = 1f / NUM_DIR * 2f;
        float angleInc = math.PI * 2 * goldenRatio;
        for(int i = 0; i < NUM_DIR; i++){
            float pitch = math.acos(1 - i * _1);
            float roll = angleInc * i;
            float x = math.sin(pitch) * math.cos(roll);
            float y = math.sin(pitch) * math.sin(roll);
            float z = math.cos(pitch);
            G_S_Dirs[i] = new float3(x, y, z);
        }
    }
}