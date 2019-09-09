using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

struct ProjectileData
{
    public Translation From;
    public Translation To;
    public Entity Target;
    public int Damage;
}

[UpdateAfter(typeof(FindTargetJobSystem))]
public class AttackJobSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
    NativeQueue<ProjectileData> _projectiles;
    EntityManager _entityManager;

    EntityArchetype _arrow;

    protected override void OnCreate() {
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _projectiles = new NativeQueue<ProjectileData>(Allocator.Persistent);
        base.OnCreate();
        _entityManager = World.EntityManager;
        _arrow = _entityManager.CreateArchetype(
            typeof(SpriteSheetAnimation_Data),
            typeof(Translation),
            typeof(MoveTo),
            typeof(AttackData),
            typeof(Projectile)
        );

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _projectiles.Dispose();
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
    private struct AttackEntityJob : IJobForEachWithEntity<DoAttack, AttackData>
    {
        [NativeDisableContainerSafetyRestriction]
        public NativeQueue<ProjectileData>.ParallelWriter Queue;
        [ReadOnly] public ComponentDataFromEntity<HealthData> HealthDataGetter;
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationDataGetter;
        [ReadOnly] public float DeltaTime;

        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, ref DoAttack doAttack, ref AttackData attackData)
        {
            if (doAttack.TargetEntity != Entity.Null)
            {
                doAttack.ElapsedTime += DeltaTime;
                if (doAttack.ElapsedTime >= doAttack.AttackTime)
                {
                    doAttack.ElapsedTime = 0f;
                    if (attackData.CharacterType == CharacterTypeData.CharactersType.Ranger ||
                        attackData.CharacterType == CharacterTypeData.CharactersType.Wizard)
                    {
                        var position = TranslationDataGetter[entity];
                        var targetPosition = TranslationDataGetter[doAttack.TargetEntity];
                        ProjectileData data = new ProjectileData()
                        {
                            From = position,
                            To = targetPosition,
                            Target = doAttack.TargetEntity,
                            Damage = doAttack.Damage
                        };
                        Queue.Enqueue(data);
                    }
                    else
                    {
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
        var translationDataGetter = GetComponentDataFromEntity<Translation>(true);
        _projectiles.Clear();

        var attackEntityJob = new AttackEntityJob
        {
            Queue = _projectiles.AsParallelWriter(),
            HealthDataGetter = healthDataGetter,
            DeltaTime = Time.deltaTime,
            EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            TranslationDataGetter = translationDataGetter,
        };
        var handle = attackEntityJob.Schedule(this, inputDeps);
        //_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);
        handle.Complete();

        var attackDataGetter = GetComponentDataFromEntity<AttackData>(true);

        // Add Attacking Component to Entities that have a Closest Target
        var addComponentJob = new AddComponentJob
        {
            EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            AttackDataGetter = attackDataGetter,
        };
        handle = addComponentJob.Schedule(this, handle);
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);
        handle.Complete();

        while (_projectiles.Count > 0)
        {
            var projectileData = _projectiles.Dequeue();
            var arrow = _entityManager.CreateEntity(_arrow);
            _entityManager.SetComponentData(arrow, new Translation() { Value = projectileData.From.Value } );
            _entityManager.SetComponentData(arrow, new Projectile() { Target = projectileData.Target, Damage = projectileData.Damage } );
            _entityManager.SetComponentData(arrow, new MoveTo() { Move = true, Position = projectileData.To.Value, MoveSpeed = 1.2f, Distance = 0.01f } );
            _entityManager.SetComponentData(arrow,
                new SpriteSheetAnimation_Data
                {
                    currentFrame = 1,
                    frameCount = 8,
                    frameTimer = 0f,
                    frameTimerMax = .1f,
                    inverted = false,
                    yIndex = 14
                }
            );
            GameController.Instance.AddPositionData(arrow);
        }

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

public struct Projectile : IComponentData
{
    public Entity Target;
    public int Damage;
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