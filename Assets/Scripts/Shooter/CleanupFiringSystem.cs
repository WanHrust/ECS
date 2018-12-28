using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class CleanupFiringSystem : JobComponentSystem
{
    private struct CleanupFiringJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray Entities;
        public EntityCommandBuffer.Concurrent ThisEntityCommandBuffer;
        public float CurrentTime;
        public ComponentDataArray<FiringComponent> Firings;
        public void Execute(int index)
        {
            if (CurrentTime - Firings[index].FiredAt < 0.5f) return;
            ThisEntityCommandBuffer.RemoveComponent<FiringComponent>(index, Entities[index]);
        }

    }
    private struct Data
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<FiringComponent> Firings;
    }

    [Inject] private Data _data;
    [Inject] private CleanupFiringBarrier _barrier;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new CleanupFiringJob
        {
            Entities = _data.Entities,
            ThisEntityCommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
            CurrentTime = Time.time,
            Firings = _data.Firings
        }.Schedule(_data.Length, 64, inputDeps);
    }
}

public class CleanupFiringBarrier : BarrierSystem
{

}
