using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : ComponentSystem{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation trans, ref MoveDirection direction) => {
            trans.Value += direction.Direction * direction.Speed * Time.DeltaTime;
        });
    }
}