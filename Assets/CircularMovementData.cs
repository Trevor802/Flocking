using Unity.Mathematics;
using Unity.Entities;
[GenerateAuthoringComponent]
public struct CircularMovementData : IComponentData{
    public float Radius;
    public float Freq;
}