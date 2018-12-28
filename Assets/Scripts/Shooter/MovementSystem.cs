using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Shooter { 
public class MovementSystem : JobComponentSystem
{
    [BurstCompile]
    struct MovementJob : IJobProcessComponentData<Position, Rotation, MoveSpeed>
    {
        public float deltaTime;

        public void Execute(ref Position position, [ReadOnly] ref Rotation rotation, [ReadOnly] ref MoveSpeed speed)
        {
            float3 value = position.Value;

            value += deltaTime * speed.speed * math.forward(rotation.Value);

            position.Value = value;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MovementJob moveJob = new MovementJob
        {
            deltaTime = Time.deltaTime
        };

        JobHandle moveHandle = moveJob.Schedule(this, inputDeps);

        return moveHandle;
    }
}

}