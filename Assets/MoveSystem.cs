using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : ComponentSystem{
    protected override void OnUpdate()
    {
        var averagePosition = new float3();
        // TODO Merge foreachs
        // Calcuate avPosition
        Entities.ForEach((ref Translation trans) => {
            averagePosition += trans.Value;
        });
        // Conduct moving & rotating
        Entities.ForEach((ref Translation trans, ref MoveDirection direction, ref Rotation rotation) => {
            direction.Direction = math.normalizesafe(averagePosition - trans.Value);
            float3 axisX = direction.Direction;
            float3 axisY = new float3(axisX.y, axisX.z, axisX.x);
            float3 axisZ = axisX * axisY;
            rotation.Value = quaternion.LookRotation(axisY, axisX);
            trans.Value += direction.Direction * direction.Speed * Time.DeltaTime;
        });
    }
}