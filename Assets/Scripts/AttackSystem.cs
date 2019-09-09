using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(FindTargetJobSystem))]
public class AttackJobSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate() {
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        base.OnCreate();
    }

    [ExcludeComponent(typeof(DoAttack))]
    [BurstCompile]
    // Add Attacking Component to Entities that have HasTarget
    private struct AddComponentJob : IJobForEachWithEntity<Translation, HasTarget>
    {
        [ReadOnly] public ComponentDataFromEntity<AttackData> AttackDataGetter;

        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, ref Translation translation, ref HasTarget target)
        {
            if (target.targetEntity != Entity.Null)
            {
                var attackData = AttackDataGetter[entity];

                EntityCommandBuffer.AddComponent(index, entity, new DoAttack
                {
                    TargetEntity = target.targetEntity,
                    AttackTime = attackData.Time,
                    Damage = attackData.Damage,
                    ElapsedTime = float.MaxValue
                });
            }
        }
    }

    [ExcludeComponent(typeof(IsDead))]
    [BurstCompile]
    private struct AttackEntityJob : IJobForEachWithEntity<DoAttack>
    {
        [ReadOnly] public ComponentDataFromEntity<HealthData> HealthDataGetter;
        [ReadOnly] public float DeltaTime;

        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, ref DoAttack doAttack)
        {
            if (doAttack.TargetEntity != Entity.Null)
            {
                doAttack.ElapsedTime += DeltaTime;
                if (doAttack.ElapsedTime >= doAttack.AttackTime)
                {
                    doAttack.ElapsedTime = 0f;
                    var healthData = HealthDataGetter[doAttack.TargetEntity];
                    if (!healthData.IsDead)
                    {
                        healthData.CurrentHealth -= doAttack.Damage;
//                        Debug.Log("Caused " + doAttack.Damage + " to " + doAttack.TargetEntity);
                    }

                    if (!healthData.IsDead)
                    {
                        EntityCommandBuffer.RemoveComponent(index, entity, typeof(DoAttack));
                        EntityCommandBuffer.RemoveComponent(index, entity, typeof(HasTarget));
                        EntityCommandBuffer.AddComponent(index, doAttack.TargetEntity, typeof(IsDead));
                    }
                }
            }
        }
    }

    /*[BurstCompile]
    private struct RemoveDeadJob : IJobForEachWithEntity<IsDead>
    {
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, ref IsDead dead)
        {
            // TODO: Particles? Animation?
            EntityCommandBuffer.RemoveComponent(index, entity, typeof(IsDead));
            EntityCommandBuffer.DestroyEntity(index, entity);
            Debug.Log("Removed dead " + entity);
        }
    }*/

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var healthDataGetter = GetComponentDataFromEntity<HealthData>(true);

        var attackEntityJob = new AttackEntityJob
        {
            HealthDataGetter = healthDataGetter,
            DeltaTime = Time.deltaTime,
            EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };
        var handle = attackEntityJob.Schedule(this, inputDeps);
        handle.Complete();
        //_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);

        var attackDataGetter = GetComponentDataFromEntity<AttackData>(true);

        // Add Attacking Component to Entities that have a Closest Target
        var addComponentJob = new AddComponentJob
        {
            EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            AttackDataGetter = attackDataGetter,
        };
        handle = addComponentJob.Schedule(this, handle);
        handle.Complete();
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);


        /*var removeDeadJob = new RemoveDeadJob()
        {
            EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
        };
        handle = removeDeadJob.Schedule(this, handle);*/


        /*var entitiesToRemove = unitQuery.ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < entitiesToRemove.Length; i++)
        {
            var entity = entitiesToRemove[i];
            Debug.Log("Removed dead " + entity);
            _entityManager.DestroyEntity(entity);
        }
        entitiesToRemove.Dispose();*/
        return handle;
    }
}

[UpdateAfter(typeof(AttackJobSystem))]
public class RemoveDeadSystem : ComponentSystem
{
    EntityManager _entityManager;

    protected override void OnCreate()
    {
        base.OnCreate();
        _entityManager = World.Active.EntityManager;
    }

    protected override void OnUpdate()
    {
        var unitQuery = GetEntityQuery(typeof(IsDead));

        var entitiesToRemove = unitQuery.ToEntityArray(Allocator.TempJob);
        for (int i = 0; i < entitiesToRemove.Length; i++)
        {
            var entity = entitiesToRemove[i];
            //Debug.Log("Removed dead " + entity);
            //_entityManager.DestroyEntity(entity);
            GameController.Instance.RemovePositionData(entity);
        }
        entitiesToRemove.Dispose();
    }
}