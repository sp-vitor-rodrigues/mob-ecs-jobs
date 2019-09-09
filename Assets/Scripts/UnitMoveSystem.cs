using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// Unit go to Move Position
public class UnitMoveSystem : JobComponentSystem
{
    NativeQueue<Entity> _completedMovement;

    protected override void OnCreate()
    {
        base.OnCreate();
        _completedMovement = new NativeQueue<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _completedMovement.Dispose();
    }

    [BurstCompile]
    private struct Job : IJobForEachWithEntity<MoveTo, Translation>
    {

        public NativeQueue<Entity>.ParallelWriter Queue;
        public float deltaTime;

        public void Execute(Entity entity, int index, [ReadOnly] ref MoveTo moveTo, ref Translation translation)
        {
            if (moveTo.Move)
            {
                if (math.distance(translation.Value, moveTo.Position) > moveTo.Distance)
                {
                    // Move to position
                    float3 moveDir = math.normalize(moveTo.Position - translation.Value);
                    translation.Value += moveDir * moveTo.MoveSpeed * deltaTime;
                }
                else
                {
                    Queue.Enqueue(entity);
                    moveTo.Move = false;
                    // Already there
                }
            }
        }

    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Job job = new Job
        {
            Queue = _completedMovement.AsParallelWriter(),
            deltaTime = Time.deltaTime,
        };
        return job.Schedule(this, inputDeps);

        while (_completedMovement.Count > 0)
        {
            GameController.Instance.EntityReachedTarget(_completedMovement.Dequeue());
        }
    }

}