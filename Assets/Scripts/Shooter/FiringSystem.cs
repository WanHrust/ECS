using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace Shooter { 
public class FiringSystem : JobComponentSystem
{
    private ComponentGroup _componentGroup;

    [Inject] private FiringBarrier _barrier;

    protected override void OnCreateManager()
    {
        _componentGroup = GetComponentGroup(ComponentType.Create<FiringComponent>(),
            ComponentType.Create<Position>(),
            ComponentType.Create<Rotation>());
        _componentGroup.SetFilterChanged(ComponentType.Create<FiringComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new FiringJob
        {
            ThisEntityCommandBuffer = _barrier.CreateCommandBuffer().ToConcurrent(),
            Positions = _componentGroup.GetComponentDataArray<Position>(),
            Rotations = _componentGroup.GetComponentDataArray<Rotation>()
        }.Schedule(_componentGroup.CalculateLength(), 64, inputDeps);
    }

    private struct FiringJob : IJobParallelFor
    {
        public EntityCommandBuffer.Concurrent ThisEntityCommandBuffer;
        public ComponentDataArray<Position> Positions;
        public ComponentDataArray<Rotation> Rotations;

        public void Execute(int index)
        {
            ThisEntityCommandBuffer.CreateEntity(index);
            ThisEntityCommandBuffer.AddSharedComponent(index,Bootstrap.BulletRenderer);
            ThisEntityCommandBuffer.AddComponent(index, new LocalToWorld());
            ThisEntityCommandBuffer.AddComponent(index, new MoveSpeed { speed = 6 });
            ThisEntityCommandBuffer.AddComponent(index, Positions[index]);
            ThisEntityCommandBuffer.AddComponent(index, Rotations[index]);
        }
    }
    private class FiringBarrier : BarrierSystem { }

}
}