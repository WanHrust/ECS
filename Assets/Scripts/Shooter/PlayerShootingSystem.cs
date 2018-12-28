using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class PlayerShootingSystem : JobComponentSystem
{
    private struct PlayerShootingJob : IJobParallelFor
    {
        [ReadOnly] public EntityArray ThisEntityArray;
        public EntityCommandBuffer.Concurrent ThisEntityCommandBuffer;

        public float CurrentTime;

        public void Execute(int index)
        {
            ThisEntityCommandBuffer.AddComponent<FiringComponent>(index, ThisEntityArray[index], new FiringComponent { FiredAt = CurrentTime});
        }
    }

    private struct Data
    {
        public readonly int Length;
        public EntityArray Entities;
        public ComponentDataArray<Weapon> Weapons;
        public SubtractiveComponent<FiringComponent> Firings;
    }

    [Inject] private Data _data;
    [Inject] private PlayerShootingBarrier _barrier;

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (Input.GetButton("Fire1"))
        {
            return new PlayerShootingJob
            {
                ThisEntityArray = _data.Entities,
                ThisEntityCommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
                CurrentTime = Time.time
            }.Schedule(_data.Length, 64, inputDeps);
        }

        return base.OnUpdate(inputDeps);
    }
}

public class PlayerShootingBarrier : BarrierSystem
{

}