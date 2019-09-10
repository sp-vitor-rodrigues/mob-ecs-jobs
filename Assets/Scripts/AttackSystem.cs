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

struct MeleeData
{
    public Translation From;
    public Translation To;
    public Entity Entity;
    public Entity Target;
    public int Damage;
}

[UpdateAfter(typeof(FindTargetJobSystem))]
public class AttackJobSystem : JobComponentSystem
{
    EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
    NativeQueue<ProjectileData> _projectiles;
    NativeQueue<MeleeData> _melees;
    EntityManager _entityManager;

    EntityArchetype _arrow;

    protected override void OnCreate() {
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _projectiles = new NativeQueue<ProjectileData>(Allocator.Persistent);
        _melees = new NativeQueue<MeleeData>(Allocator.Persistent);
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
        _melees.Dispose();
    }

    [ExcludeComponent(typeof(DoAttack))]
    //[BurstCompile]
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
        [NativeDisableContainerSafetyRestriction]
        public NativeQueue<ProjectileData>.ParallelWriter QueueProjectiles;
        public NativeQueue<MeleeData>.ParallelWriter QueueMelees;
        [ReadOnly] public ComponentDataFromEntity<HealthData> HealthDataGetter;
        [ReadOnly] public ComponentDataFromEntity<Translation> TranslationDataGetter;
        [ReadOnly] public ComponentDataFromEntity<AttackData> AttackDataGetter;
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
                    var attackData = AttackDataGetter[entity];

                    var position = TranslationDataGetter[entity];
                    var targetPosition = TranslationDataGetter[doAttack.TargetEntity];
                    if (attackData.CharacterType == CharacterTypeData.CharactersType.Ranger ||
                        attackData.CharacterType == CharacterTypeData.CharactersType.Wizard)
                    {
                        ProjectileData data = new ProjectileData()
                        {
                            From = position,
                            To = targetPosition,
                            Target = doAttack.TargetEntity,
                            Damage = doAttack.Damage
                        };
                        QueueProjectiles.Enqueue(data);
                    }
                    else
                    {
                        MeleeData data = new MeleeData()
                        {
                            From = position,
                            To = targetPosition,
                            Target = doAttack.TargetEntity,
                            Entity = entity,
                            Damage = doAttack.Damage
                        };
                        QueueMelees.Enqueue(data);
                    }
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var healthDataGetter = GetComponentDataFromEntity<HealthData>(true);
        var translationDataGetter = GetComponentDataFromEntity<Translation>(true);
        var attackDataGetter = GetComponentDataFromEntity<AttackData>(true);
        _projectiles.Clear();

        var attackEntityJob = new AttackEntityJob
        {
            QueueProjectiles = _projectiles.AsParallelWriter(),
            QueueMelees = _melees.AsParallelWriter(),
            HealthDataGetter = healthDataGetter,
            DeltaTime = Time.deltaTime,
            EntityCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            TranslationDataGetter = translationDataGetter,
            AttackDataGetter = attackDataGetter,
        };
        var handle = attackEntityJob.Schedule(this, inputDeps);
        //_endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);
        handle.Complete();


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
            _entityManager.SetComponentData(arrow, new MoveTo() { Move = true, Position = projectileData.To.Value, MoveSpeed = 2.0f, Distance = 0.01f } );
            _entityManager.SetComponentData(arrow,
                new SpriteSheetAnimation_Data
                {
                    currentFrame = 1,
                    frameCount = 8,
                    frameTimer = 0f,
                    frameTimerMax = .1f,
                    inverted = false,
                    yIndex = 14,
                    OneLiner = false
                }
            );
            //GameController.Instance.AddPositionData(arrow);
        }

        while (_melees.Count > 0)
        {
            var meleeData = _melees.Dequeue();
            var health = _entityManager.GetComponentData<HealthData>(meleeData.Target);

            if (!health.IsDead)
            {
                health.CurrentHealth -= meleeData.Damage;
            }
            _entityManager.SetComponentData(meleeData.Target, health);

            if (health.IsDead)
            {
                _entityManager.RemoveComponent(meleeData.Entity, typeof(DoAttack));
                _entityManager.RemoveComponent(meleeData.Entity, typeof(HasTarget));
                _entityManager.AddComponent(meleeData.Target, typeof(IsDead));
            }

        }
        var totalDefenders = 0;
        var totalAttackers = 0;
        EntityQuery unitQuery = GetEntityQuery(typeof(Orc), ComponentType.Exclude<IsDead>());
        totalAttackers += unitQuery.CalculateEntityCount();
        unitQuery = GetEntityQuery(typeof(Ogre), ComponentType.Exclude<IsDead>());
        totalAttackers += unitQuery.CalculateEntityCount();

        unitQuery = GetEntityQuery(typeof(Knight), ComponentType.Exclude<IsDead>());
        totalDefenders += unitQuery.CalculateEntityCount();
        unitQuery = GetEntityQuery(typeof(Ranger), ComponentType.Exclude<IsDead>());
        totalDefenders += unitQuery.CalculateEntityCount();
        unitQuery = GetEntityQuery(typeof(Wizard), ComponentType.Exclude<IsDead>());
        totalDefenders += unitQuery.CalculateEntityCount();

        GameController.Instance.ChangeUnitNumber.SetNumberOfAttackers(totalAttackers);
        GameController.Instance.ChangeUnitNumber.SetNumberOfDefenders(totalDefenders);

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