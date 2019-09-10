using System;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct PositionData : System.IEquatable<PositionData>
{
    public Entity Entity;
    public float3 Position;
    public Matrix4x4 Matrix;
    public Vector4 UV;
    public int RenderSlice;
    public int SheetYIndex;

    public bool Equals(PositionData other)
    {
        return Entity.Equals(other.Entity) &&
               Position.Equals(other.Position) &&
               Matrix.Equals(other.Matrix) &&
               UV.Equals(other.UV) &&
               RenderSlice == other.RenderSlice &&
               SheetYIndex == other.SheetYIndex;
    }

    public override bool Equals(object obj)
    {
        return obj is PositionData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Entity.GetHashCode();
            hashCode = (hashCode * 397) ^ Position.GetHashCode();
            hashCode = (hashCode * 397) ^ Matrix.GetHashCode();
            hashCode = (hashCode * 397) ^ UV.GetHashCode();
            hashCode = (hashCode * 397) ^ RenderSlice;
            hashCode = (hashCode * 397) ^ SheetYIndex;
            return hashCode;
        }
    }
}

[Serializable]
public class AnimationData
{
    public enum AnimationType
    {
        Walk,
        Attack,
        Idle,
        Cast
    }

    public AnimationType AnimType;
    public int YIndex;
    public int Frames;
    public int PixelsPerFrame;
    public bool Inverted;
}

public class SlicePositionData
{
    public enum FactionType
    {
        Defenders = 0,
        Attackers = 1,
        Total
    }

    public NativeList<PositionData> PositionsData;
    public bool DataChanged;

    public SlicePositionData()
    {
        PositionsData = new NativeList<PositionData>(Allocator.Persistent);
    }

    public void Add(PositionData positionData)
    {
        PositionsData.Add(positionData);
        DataChanged = true;
    }

    public void Remove(PositionData positionData)
    {
        PositionsData.Remove(positionData);
        DataChanged = true;
    }

    public void Clear()
    {
        PositionsData.Dispose();
    }
}

[Serializable]
public class CharacterTypeData
{
    const int POSITION_SLICES = 20;

    public enum CharactersType
    {
        Ogre,
        Orc,
        Knight,
        Ranger,
        Wizard
    }

    public CharactersType CharacterType;
    public float AttackDistance = 0.15f;
    public int Damage = 10;
    public float AttackTime = 1f;
    public float MoveSpeed = 0.5f;
    public int SpawnChance;
    public int Health = 100;
    public AnimationData[] Animations;
}

[Serializable]
public class FactionData
{
    public SlicePositionData.FactionType FactionType;
    public CharacterTypeData[] CharactersData;
}

public struct RenderData
{
    public float3 position;
    public Matrix4x4 matrix;
    public Vector4 uv;
}

public struct HealthData : IComponentData
{
    public int MaxHealth;
    public int CurrentHealth;
    public bool IsDead
    {
        get { return CurrentHealth <= 0; }
    }
}

public struct MoveTo : IComponentData {
    public bool Move;
    public float3 Position;
    public float MoveSpeed;
    public float Distance;
}

public struct Orc : IComponentData
{
}

public struct Ogre : IComponentData
{
}

public struct Knight : IComponentData
{
}

public struct Ranger : IComponentData
{
}

public struct Wizard : IComponentData
{
}

public class GameController : MonoBehaviour
{
    const int POSITION_SLICES = 20;

    public Material Material;
    public Mesh Mesh;
    public Camera MainCamera;

    public SlicePositionData[] SlicePositionData = new SlicePositionData[POSITION_SLICES];

    public ChangeUnitNumber ChangeUnitNumber;

    static GameController _instance;

    public static GameController Instance => _instance;
    public bool UseQuadrantSystem;

    public FactionData[] FactionsData = new FactionData[2];

    float[] _slicesPositions = new float[POSITION_SLICES];

    EntityManager _entityManager;

    EntityArchetype _meleeArchetype;
    EntityArchetype _rangedArchetype;
    EntityArchetype _aoeArchetype;

    public bool ReadyToRun = false;

    float _timeBetweenSpawns = 0.2f;
    int _amountOfUnitsPerSpawn = 10;
    int _totalUnits = 0;

    void Start()
    {
        InitializeSlices();
        _instance = this;

        _entityManager = World.Active.EntityManager;

        _meleeArchetype = _entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data),
            typeof(MoveTo),
            typeof(QuadrantEntity),
            typeof(MeleeUnit),
            typeof(Unit),
            typeof(HealthData),
            typeof(AttackData)
        );
        _rangedArchetype = _entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data),
            typeof(MoveTo),
            typeof(QuadrantEntity),
            typeof(RangedUnit),
            typeof(Unit),
            typeof(HealthData),
            typeof(AttackData)
        );
        _aoeArchetype = _entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data),
            typeof(MoveTo),
            typeof(QuadrantEntity),
            typeof(AOEUnit),
            typeof(Unit),
            typeof(HealthData),
            typeof(AttackData)
        );

        CreateUnits(FactionsData);

        ReadyToRun = true;
    }

    void CreateUnits(FactionData[] factionsData)
    {
        for (int i = 0; i < factionsData.Length; i++)
        {
            StartCoroutine(CreateUnits(factionsData[i]));
        }
    }

    IEnumerator CreateUnits(FactionData factionData)
    {
        var storedNumberOfUnits = PlayerPrefs.GetInt(factionData.FactionType == global::SlicePositionData.FactionType.Defenders ? "NumberOfDefenders" : "NumberOfAttackers", 1);

        var totalUnits = 0;
        while (totalUnits < storedNumberOfUnits)
        {
            var unitsToSpawn = (storedNumberOfUnits - totalUnits) < _amountOfUnitsPerSpawn
                ? storedNumberOfUnits - totalUnits
                : _amountOfUnitsPerSpawn;

            var index = GetRandomUnit(factionData.CharactersData);
            CreateUnits(unitsToSpawn, factionData.FactionType, factionData.CharactersData[index]);

            totalUnits += unitsToSpawn;

            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
    }

    int GetRandomUnit(CharacterTypeData[] charsData)
    {
        var chance = UnityEngine.Random.Range(1, 100);

        var accumulated = 0;

        for (var i = 0; i < charsData.Length; i++)
        {
            var charData = charsData[i];
            accumulated += charData.SpawnChance;
            if (chance < accumulated)
            {
                return i;
            }
        }

        return -1;
    }

    void CreateUnits(int unitsToSpawn, SlicePositionData.FactionType faction, CharacterTypeData charData)
    {
        var entityArray = new NativeArray<Entity>(unitsToSpawn, Allocator.Temp);

        var animationData = charData.Animations[(int) AnimationData.AnimationType.Walk];

        var movePosition = 0f;
        switch (charData.CharacterType)
        {
            case CharacterTypeData.CharactersType.Knight:
                _entityManager.CreateEntity(_meleeArchetype, entityArray);
                movePosition = -5.15f;
                break;
            case CharacterTypeData.CharactersType.Orc:
            case CharacterTypeData.CharactersType.Ogre:
                _entityManager.CreateEntity(_meleeArchetype, entityArray);
                movePosition = -5.05f;
                break;
            case CharacterTypeData.CharactersType.Ranger:
                _entityManager.CreateEntity(_rangedArchetype, entityArray);
                movePosition = -6f;
                break;
            case CharacterTypeData.CharactersType.Wizard:
                _entityManager.CreateEntity(_aoeArchetype, entityArray);
                movePosition = -7f;
                break;
        }

        foreach (Entity entity in entityArray)
        {
            switch (charData.CharacterType)
            {
                case CharacterTypeData.CharactersType.Knight:
                    _entityManager.AddComponent(entity, typeof(Knight));
                    break;
                case CharacterTypeData.CharactersType.Orc:
                    _entityManager.AddComponent(entity, typeof(Orc));
                    break;
                case CharacterTypeData.CharactersType.Ogre:
                    _entityManager.AddComponent(entity, typeof(Ogre));
                    break;
                case CharacterTypeData.CharactersType.Ranger:
                    _entityManager.AddComponent(entity, typeof(Ranger));
                    break;
                case CharacterTypeData.CharactersType.Wizard:
                    _entityManager.AddComponent(entity, typeof(Wizard));
                    break;
            }
            
            var position = new float3(
                faction == global::SlicePositionData.FactionType.Attackers ? UnityEngine.Random.Range(10f, 30f) : -10f,
                UnityEngine.Random.Range(-5f, 5f), 0);
            var targetPosition = new float3(movePosition, position.y, 0);
            _entityManager.SetComponentData(entity,
                new Translation
                {
                    Value = position
                }
            );
            _entityManager.SetComponentData(entity,
                new SpriteSheetAnimation_Data
                {
                    currentFrame = UnityEngine.Random.Range(0, animationData.Frames),
                    frameCount = animationData.Frames,
                    frameTimer = UnityEngine.Random.Range(0f, 1f),
                    frameTimerMax = .1f,
                    inverted = animationData.Inverted,
                    yIndex = animationData.YIndex
                }
            );
            _entityManager.SetComponentData(entity, new MoveTo
            {
                Move = true, Position = targetPosition, MoveSpeed = charData.MoveSpeed, Distance = 0.15f
            });
            _entityManager.SetComponentData(entity, new QuadrantEntity
            {
                typeEnum = faction == global::SlicePositionData.FactionType.Defenders ? QuadrantEntity.TypeEnum.Defender : QuadrantEntity.TypeEnum.Attacker
            });
            _entityManager.SetComponentData(entity, new AttackData
            {
                Range = charData.AttackDistance,
                Damage = charData.Damage,
                Time = charData.AttackTime,
                CharacterType = charData.CharacterType
            });
            _entityManager.SetComponentData(entity, new HealthData
            {
                MaxHealth = charData.Health,
                CurrentHealth = charData.Health
            });
            AddPositionData(entity);
        }
        entityArray.Dispose();
    }

    void InitializeSlices()
    {
        float3 cameraPosition = MainCamera.transform.position;
        float cameraSliceSize = MainCamera.orthographicSize * 2f / POSITION_SLICES;

        _slicesPositions[0] = cameraPosition.y + MainCamera.orthographicSize;
        SlicePositionData[0] = new SlicePositionData();
        for (int i = 1; i < POSITION_SLICES; i++)
        {
            _slicesPositions[i] = _slicesPositions[0] - cameraSliceSize * i;
            SlicePositionData[i] = new SlicePositionData();
        }
    }

    int GetSliceIndex(float positionY)
    {
        for (int i = POSITION_SLICES - 1; i > -1; i--)
        {
            if (positionY < _slicesPositions[i])
            {
                return i;
            }
        }

        return -1;
    }

    public void AddPositionData(Entity entity)
    {
        var translation = _entityManager.GetComponentData<Translation>(entity);
        var index = GetSliceIndex(translation.Value.y);
        if (index < 0)
        {
            Debug.LogError("Proper slice index was not found!");
        }
        else
        {
            var positionData = new PositionData()
            {
                Entity = entity,
                Position = translation.Value,
                RenderSlice = index
            };
            SlicePositionData[index].Add(positionData);
        }
    }

    public void RemovePositionData(Entity entity)
    {
        var translation = _entityManager.GetComponentData<Translation>(entity);
        var index = GetSliceIndex(translation.Value.y);
        if (index < 0)
        {
            Debug.LogError("Proper slice index was not found!");
        }
        else
        {
            for (int i = 0; i < SlicePositionData[index].PositionsData.Length; i++)
            {
                if (SlicePositionData[index].PositionsData[i].Entity == entity)
                {
                    SlicePositionData[index].PositionsData.RemoveAt(i);
                }


            }
        }
    }

    public void EntityReachedTarget(Entity entity)
    {
        if (_entityManager.HasComponent(entity, typeof(Projectile)))
        {
            _entityManager.RemoveComponent(entity, typeof(MoveTo));

            var projectile = _entityManager.GetComponentData<Projectile>(entity);
            var health = _entityManager.GetComponentData<HealthData>(projectile.Target);

            if (!health.IsDead)
            {
                health.CurrentHealth -= projectile.Damage;
            }
            _entityManager.SetComponentData(projectile.Target, health);

            if (health.IsDead)
            {
                _entityManager.RemoveComponent(entity, typeof(DoAttack));
                _entityManager.RemoveComponent(entity, typeof(HasTarget));
                _entityManager.AddComponent(projectile.Target, typeof(IsDead));
            }

        }
    }

    void OnDestroy()
    {
        DestroyEntities();
    }

    void DestroyEntities()
    {
        if (World.Active != null)
        {
            _entityManager?.DestroyEntity(_entityManager.GetAllEntities());
        }

        for (int i = 0; i < POSITION_SLICES; i++)
        {
            SlicePositionData[i].Clear();
        }
        /*for (int i = 0; i < FactionsData.Length; i++)
        {
            FactionsData[i].Clear();
        }*/
    }
}