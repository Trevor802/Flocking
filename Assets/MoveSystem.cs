using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : ComponentSystem{
    protected override void OnUpdate()
    {
        // Conduct moving & rotating
        Entities.ForEach((ref Translation trans, ref MoveDirection direction, ref Rotation rotation) => {
            // Calculate direction
            float3 pos = trans.Value;
            float3 coh = float3.zero;
            float3 ali = float3.zero;
            float3 avo = float3.zero;
            float3 sty = float3.zero;
            uint count = 0;
            uint avoCount = 0;
            Entities.ForEach((ref Translation t, ref MoveDirection d) => {
                float3 dist = pos - t.Value;
                float distSqr = math.mul(dist, dist);
                if (distSqr < DEF.VIEW_RADIUS_SQR && math.abs(distSqr) > DEF.EPS){
                    count++;
                    coh += t.Value;
                    ali += d.Value;
                    if (distSqr < DEF.AVO_RADIUS_SQR){
                        avoCount++;
                        avo += math.normalize(dist) * math.pow(DEF.AVO_RADIUS_SQR - distSqr, 1);
                    }
                } 
            });
            if (count > 0){
                coh /= count;
                ali /= count;
            }
            if (avoCount > 0){
                avo /= avoCount;
            }
            float rSqr = math.mul(pos, pos);
            float p = rSqr / DEF.STY_RADIUS_SQR;
            if (p > 0.9f)
                sty = -math.normalizesafe(pos) * p * p;
            // Clamp
            coh = coh.Clamp(DEF.COH_WGT, DEF.COH_WGT_SQR);
            ali = ali.Clamp(DEF.ALI_WGT, DEF.ALI_WGT_SQR);
            avo = avo.Clamp(DEF.AVO_WGT, DEF.AVO_WGT_SQR);
            sty = sty.Clamp(DEF.STY_WGT, DEF.STY_WGT_SQR);
            // Accumulate
            float3 all = direction.Value * DEF.DAMP + coh + ali + avo + sty;
            // all = all.Clamp(DEF.MAX_SPEED, DEF.MAX_SPEED_SQR);
            // Save direction for next loop
            direction.Value = math.normalize(all);
            // Look towards
            float3 fwd = direction.Value;
            float3 rt = new float3(fwd.z, 0, -fwd.x);
            float3 up = math.normalizesafe(math.cross(fwd, rt));
            rotation.Value = quaternion.LookRotation(fwd, up);
            // Execute movement
            trans.Value += direction.Value * DEF.MAX_SPEED * Time.DeltaTime;
        });
    }

    private float3 GetCohesion(float3 pos, float radiusSqr){
        float3 result = float3.zero;
        uint count = 0;
        Entities.ForEach((ref Translation trans) => {
            float3 dist = pos - trans.Value;
            float distSqr = math.mul(dist, dist);
            if (distSqr < radiusSqr && math.abs(distSqr) > DEF.EPS){
                count++;
                result += trans.Value;
            }
        });
        if (count > 0)
            result /= count;
        return result;
    }

    private float3 GetAlignment(float3 pos, float radiusSqr){
        float3 result = float3.zero;
        uint count = 0;
        Entities.ForEach((ref Translation trans, ref MoveDirection direction) => {
            float3 dist = pos - trans.Value;
            float distSqr = math.mul(dist, dist);
            if (distSqr < radiusSqr && math.abs(distSqr) > DEF.EPS){
                count++;
                result += direction.Value;
            }
        });
        if (count > 0)
            result /= count;
        return result;
    }

    private float3 GetAvoidance(float3 pos, float radiusSqr){
        float3 result = float3.zero;
        uint count = 0;
        Entities.ForEach((ref Translation trans) => {
            float3 dist = pos - trans.Value;
            float distSqr = math.mul(dist, dist);
            if (distSqr < radiusSqr && math.abs(distSqr) > DEF.EPS){
                count++;
                result += dist;
            }
        });
        if (count > 0)
            result /= count;
        return result;
    }
}

public static class MathExtension{
    public static float3 Clamp(this float3 value, float maxValue, float maxValueSqr){
        if (math.mul(value, value) > maxValueSqr){
            return math.normalize(value) * maxValue;
        }
        return value;
    }
}