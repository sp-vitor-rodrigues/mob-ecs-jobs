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
    public int SpawnChance;
    public AnimationData[] Animations;

    /*public void AddPositionData(PositionData positionData)
    {
        SlicePositionData[positionData.RenderSlice].Add(positionData);
    }

    public void RemovePositionData(PositionData positionData)
    {
        SlicePositionData[positionData.RenderSlice].Remove(positionData);
    }

    public void Clear()
    {
        for (int i = 0; i < POSITION_SLICES; i++)
        {
            SlicePositionData[i].Clear();
        }
    }*/
}

[Serializable]
public class FactionData
{
    public SlicePositionData.FactionType FactionType;
    public CharacterTypeData[] CharactersData;

    /*public FactionData(int numOfCharacters, Material material, Mesh mesh)
    {
        CharacterData = new CharacterTypeData[numOfCharacters];
        for (int i = 0; i < numOfCharacters; i++)
        {
            CharacterData[i] = new CharacterTypeData(material, mesh);
        }
    }

    public void AddPositionData(PositionData positionData, int characterType)
    {
        CharacterData[characterType].AddPositionData(positionData);
    }

    public void RemovePositionData(PositionData positionData, int characterType)
    {
        CharacterData[characterType].RemovePositionData(positionData);
    }

    public void Clear()
    {
        for (int i = 0; i < CharacterData.Length; i++)
        {
            CharacterData[i].Clear();
        }
    }*/
}

public struct RenderData
{
    public float3 position;
    public Matrix4x4 matrix;
    public Vector4 uv;
}

public struct MoveTo : IComponentData {
    public bool Move;
    public float3 Position;
    public float MoveSpeed;
}
/*
[Serializable]
public class ViewData
{
    public SlicePositionData.FactionType FactionType;
    public bool Inverted;
    public int SpawnChance;
}*/

public class GameController : MonoBehaviour
{
    const int POSITION_SLICES = 20;

    public Material Material;
    public Mesh Mesh;
//    public ViewData[] PlayerCharactersViewData;
//    public ViewData[] EnemyCharactersViewData;
    public Camera MainCamera;

    public SlicePositionData[] SlicePositionData = new SlicePositionData[POSITION_SLICES];

    static GameController _instance;

    public static GameController Instance => _instance;

    public FactionData[] FactionsData = new FactionData[2];

    float[] _slicesPositions = new float[POSITION_SLICES];

    EntityManager _entityManager;

    EntityArchetype _meleeArchetype;

    public bool ReadyToRun = false;

    int _timeBetweenSpawns = 1;
    int _amountOfUnitsPerSpawn = 50;
    int _totalUnits = 0;

    void Start()
    {
        InitializeSlices();
        _instance = this;

        _entityManager = World.Active.EntityManager;

        /*FactionsData[0] = new FactionData(PlayerCharactersViewData.Length, PlayerCharactersViewData[0].Material, PlayerCharactersViewData[0].Mesh);
        FactionsData[1] = new FactionData(EnemyCharactersViewData.Length, EnemyCharactersViewData[0].Material, EnemyCharactersViewData[0].Mesh);*/

        _meleeArchetype = _entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data),
            typeof(MoveTo)
        );

        /*StartCoroutine(CreateDefenders(PlayerCharactersViewData));
        StartCoroutine(CreateAttackers(EnemyCharactersViewData));*/

        ReadyToRun = true;
    }

    /*IEnumerator CreateDefenders(ViewData[] viewsData)
    {
        var storedNumberOfDefenders = PlayerPrefs.GetInt("NumberOfDefenders", 1);

        while (_totalUnits < storedNumberOfDefenders)
        {
            var unitsToSpawn = (storedNumberOfDefenders - _totalUnits) < _amountOfUnitsPerSpawn
                ? storedNumberOfDefenders - _totalUnits
                : _amountOfUnitsPerSpawn;

            var index = GetRandomView(viewsData);
            CreateMeleeDefenders(unitsToSpawn, viewsData[index], index);
            _totalUnits += unitsToSpawn;

            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
    }*/

    /*IEnumerator CreateAttackers(ViewData[] viewsData)
    {
        var storedNumberOfEnemies = PlayerPrefs.GetInt("NumberOfAttackers", 1);

        while (_totalUnits < storedNumberOfEnemies)
        {
            var unitsToSpawn = (storedNumberOfEnemies - _totalUnits) < _amountOfUnitsPerSpawn
                ? storedNumberOfEnemies - _totalUnits
                : _amountOfUnitsPerSpawn;

            var index = GetRandomView(viewsData);
            CreateMeleeAttackers(unitsToSpawn, viewsData[index], index);
            _totalUnits += unitsToSpawn;

            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
    }*/

    /*int GetRandomView(ViewData[] viewsData)
    {
        var chance = UnityEngine.Random.Range(1, 100);

        var accumulated = 0;

        for (var i = 0; i < viewsData.Length; i++)
        {
            var viewData = viewsData[i];
            accumulated += viewData.SpawnChance;
            if (chance < accumulated)
            {
                return i;
            }
        }

        return -1;
    }*/

    void InitializeSlices()
    {
        float3 cameraPosition = MainCamera.transform.position;
        float cameraSliceSize = MainCamera.orthographicSize * 2f / POSITION_SLICES;

        _slicesPositions[0] = cameraPosition.y + MainCamera.orthographicSize;
        for (int i = 1; i < 20; i++)
        {
            _slicesPositions[i] = _slicesPositions[0] - cameraSliceSize * i;
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

    /*void AddPositionData(Entity entity, SlicePositionData.FactionType factionType, int characterType)
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
            FactionsData[(int)factionType].AddPositionData(positionData, characterType);
        }
    }*/

    /*void CreateMeleeAttackers(int amount, ViewData viewData, int index)
    {
        var entityArray = new NativeArray<Entity>(amount, Allocator.Temp);

        _entityManager.CreateEntity(_meleeArchetype, entityArray);

        foreach (Entity entity in entityArray)
        {
            var position = new float3(UnityEngine.Random.Range(10f, 30f), UnityEngine.Random.Range(-5f, 5f), 0);
            var targetPosition = new float3(-5.2f, position.y, 0);
            _entityManager.SetComponentData(entity,
                new Translation
                {
                    Value = position
                }
            );
            _entityManager.SetComponentData(entity,
                new SpriteSheetAnimation_Data
                {
                    currentFrame = UnityEngine.Random.Range(0, viewData.Frames),
                    frameCount = viewData.Frames,
                    frameTimer = UnityEngine.Random.Range(0f, 1f),
                    frameTimerMax = .1f,
                    inverted = viewData.Inverted,
                }
            );
            _entityManager.SetComponentData(entity, new MoveTo
            {
                Move = true, Position = targetPosition, MoveSpeed = 0.5f
            });
            AddPositionData(entity, viewData.FactionType, index);
        }
        entityArray.Dispose();
    }

    void CreateMeleeDefenders(int amount, ViewData viewData, int index)
    {
        var entityArray = new NativeArray<Entity>(amount, Allocator.Temp);

        _entityManager.CreateEntity(_meleeArchetype, entityArray);

        foreach (Entity entity in entityArray)
        {
            var position = new float3(-10f, UnityEngine.Random.Range(-5f, 5f), 0);
            var targetPosition = new float3(-5.2f, position.y, 0);
            _entityManager.SetComponentData(entity,
                new Translation
                {
                    Value = position
                }
            );
            _entityManager.SetComponentData(entity,
                new SpriteSheetAnimation_Data
                {
                    currentFrame = UnityEngine.Random.Range(0, viewData.Frames),
                    frameCount = viewData.Frames,
                    frameTimer = UnityEngine.Random.Range(0f, 1f),
                    frameTimerMax = .1f,
                    inverted = viewData.Inverted,
                }
            );
            _entityManager.SetComponentData(entity, new MoveTo
            {
                Move = true, Position = targetPosition, MoveSpeed = 0.5f
            });
            AddPositionData(entity, viewData.FactionType, index);
        }
        entityArray.Dispose();
    }*/
    
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

        /*for (int i = 0; i < FactionsData.Length; i++)
        {
            FactionsData[i].Clear();
        }*/
    }
}