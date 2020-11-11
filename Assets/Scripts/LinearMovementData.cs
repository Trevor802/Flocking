using Unity.Mathematics;
using Unity.Entities;
[GenerateAuthoringComponent]
public struct LinearMovementData : IComponentData{
    public float Freq;
    public float Ampl;
}