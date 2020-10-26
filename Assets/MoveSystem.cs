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
            float3 dir = direction.Value;
            float3 coh = float3.zero;
            float3 ali = float3.zero;
            float3 avo = float3.zero;
            float3 sty = float3.zero;
            uint count = 0;
            uint avoCount = 0;
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
            // Accumulate
            float3 all = direction.Value * DEF.Instance.DAMP + coh + ali + avo + sty;
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
}