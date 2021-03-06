﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;
using Random = Unity.Mathematics.Random;

public class MoveSystem : ComponentSystem{
    protected override void OnUpdate()
    {
        PointOctree<(float3, float3)> pointTree = null;
        var rand = new Random();
        rand.InitState();
        if (FlockingManager.Instance.OCTREE){
            pointTree = new PointOctree<(float3, float3)>(FlockingManager.Instance.OCT_LTH, Vector3.zero, FlockingManager.Instance.MIN_OCT_NODE);
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
            float3 rad = rand.NextFloat3Direction();
            uint count = 0;
            uint avoCount = 0;
            // Golden spiral collision detection
            foreach(var f in BoidHelper.G_S_Dirs){
                float3 d = math.mul(rotation.Value, f);
                if (FlockingManager.Instance.DEBUG){
                    Debug.DrawLine(pos.ToVector3(), pos.ToVector3() + d.ToVector3() * FlockingManager.Instance.COL_DET_RAD, Color.red);
                }
                var pWorld = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>();
                var cWorld = pWorld.PhysicsWorld.CollisionWorld;
                var raycast = new RaycastInput{
                    Start = pos,
                    End = pos + d * FlockingManager.Instance.COL_DET_RAD,
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
            if (FlockingManager.Instance.OCTREE){
                // Using octree
                var nodes = pointTree.GetNearby(pos.ToVector3(), FlockingManager.Instance.VIEW_RADIUS);
                foreach(var n in nodes){
                    float3 dist = pos - n.Item1;
                    float distSqr = math.mul(dist, dist);
                    float halfAngle = math.acos(math.mul(dir, -dist) / math.sqrt(distSqr));
                    if (distSqr < FlockingManager.Instance.VIEW_RADIUS_SQR && distSqr > FlockingManager.EPS && 
                        halfAngle < FlockingManager.Instance.VIEW_HALF_AGL_DEG * FlockingManager.D2R){
                        count++;
                        coh += n.Item1;
                        ali += n.Item2;
                        if (distSqr < FlockingManager.Instance.AVO_RADIUS_SQR){
                            avoCount++;
                            avo += math.normalize(dist) * (FlockingManager.Instance.AVO_RADIUS_SQR - distSqr);
                        }
                    }
                }
            }
            else{
                // Not using octree
                Entities.ForEach((ref Translation t, ref MoveDirection d) => {
                    float3 dist = pos - t.Value;
                    float distSqr = math.mul(dist, dist);
                    float halfAngle = math.acos(math.mul(dir, dist) / math.sqrt(distSqr));
                    if (distSqr < FlockingManager.Instance.VIEW_RADIUS_SQR && distSqr > FlockingManager.EPS && math.abs(halfAngle) < FlockingManager.Instance.VIEW_HALF_AGL_DEG){
                        count++;
                        coh += t.Value;
                        ali += d.Value;
                        if (distSqr < FlockingManager.Instance.AVO_RADIUS_SQR){
                            avoCount++;
                            avo += math.normalize(dist) * (FlockingManager.Instance.AVO_RADIUS_SQR - distSqr);
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
            float rSqr = math.mul(pos, pos);
            float p = rSqr / (FlockingManager.Instance.STY_RADIUS * FlockingManager.Instance.STY_RADIUS);
            if (p > FlockingManager.Instance.STY_SRT_RIT * FlockingManager.Instance.STY_SRT_RIT)
                sty = -math.normalizesafe(pos) * p * FlockingManager.Instance.STY_WGT;
            // Clamp
            coh = coh.Clamp(FlockingManager.Instance.COH_WGT);
            ali = ali.Clamp(FlockingManager.Instance.ALI_WGT);
            avo = avo.Clamp(FlockingManager.Instance.AVO_WGT);
            col *= FlockingManager.Instance.COL_WGT;
            // Accumulate
            float3 all = dir * FlockingManager.Instance.DAMP_WGT + coh + ali + avo + sty + col + rad * FlockingManager.Instance.RAND_WGT;
            all = all.Clamp(FlockingManager.Instance.SPD_LMT.x, FlockingManager.Instance.SPD_LMT.y);
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
    public static float3 GetUp(float3 forward){
        float3 right = new float3(forward.z, 0, -forward.x);
        float3 up = math.normalizesafe(math.cross(forward, right));
        return up;
    }
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