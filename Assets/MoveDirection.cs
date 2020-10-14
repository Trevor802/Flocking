﻿using Unity.Entities;
using Unity.Mathematics;
[GenerateAuthoringComponent]
public struct MoveDirection : IComponentData{
    public float3 Direction;
    public float Speed;
}