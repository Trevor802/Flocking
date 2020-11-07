using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class LinearMoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation trans, ref LinearMovementData linear) => {
            float z = math.sin((float) Time.ElapsedTime * linear.Freq) * linear.Ampl;
            trans.Value = new float3(trans.Value.x, trans.Value.y, z);
        });
    }
}

public class CircularMovementSystem : ComponentSystem{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation trans, ref CircularMovementData circular) => {
            float x = math.sin((float) Time.ElapsedTime * circular.Freq) * circular.Radius;
            float y = math.cos((float) Time.ElapsedTime * circular.Freq) * circular.Radius;
            trans.Value = new float3(x, y , trans.Value.z);
        });
    }
}