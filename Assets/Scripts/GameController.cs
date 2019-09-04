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

    public bool Equals(PositionData other)
    {
        return Entity.Equals(other.Entity) && Position.Equals(other.Position) && Matrix.Equals(other.Matrix) && UV.Equals(other.UV) && RenderSlice == other.RenderSlice;
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
            return hashCode;
        }
    }
}

public class SlicePositionData
{
    public NativeList<PositionData> PositionsData = new NativeList<PositionData>(Allocator.Persistent);
    public bool DataChanged;

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

public struct RenderData
{
    public float3 position;
    public Matrix4x4 matrix;
    public Vector4 uv;
}

public class GameController : MonoBehaviour
{
    private const int POSITION_SLICES = 20;

    public Mesh Mesh;
    public Material Material;

    static GameController _instance;

    public static GameController Instance => _instance;

    public SlicePositionData[] EntitiesPositionData = new SlicePositionData[POSITION_SLICES];

    float[] _slicesPositions = new float[POSITION_SLICES];

    EntityManager _entityManager;
    
    public bool ReadyToRun = false;

    void Start()
    {
        InitializeSlices();
        _instance = this;

        _entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = _entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data)
        );

        var storedNumberOfEnemies = PlayerPrefs.GetInt("NumberOfEnemies", 1);

        var entityArray = new NativeArray<Entity>(storedNumberOfEnemies, Allocator.Temp);
        _entityManager.CreateEntity(entityArchetype, entityArray);

        foreach (Entity entity in entityArray) {
            _entityManager.SetComponentData(entity,
                new Translation {
                    Value = new float3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-2.5f, 2.5f), 0)
                }
            );
            _entityManager.SetComponentData(entity,
                new SpriteSheetAnimation_Data {
                    currentFrame = UnityEngine.Random.Range(0, 7),
                    frameCount = 7,
                    frameTimer = UnityEngine.Random.Range(0f, 1f),
                    frameTimerMax = .1f
                }
            );
            AddPositionData(entity);
        }

        ReadyToRun = true;
    }

    void InitializeSlices()
    {
        Camera camera = Camera.main;
        float3 cameraPosition = camera.transform.position;
        float cameraSliceSize = camera.orthographicSize * 2f / POSITION_SLICES;

        _slicesPositions[0] = cameraPosition.y + camera.orthographicSize;
        for (int i = 1; i < 20; i++)
        {
            _slicesPositions[i] = _slicesPositions[0] - cameraSliceSize * i;
        }

        for (int i = 0; i < POSITION_SLICES; i++)
        {
            EntitiesPositionData[i] = new SlicePositionData();
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

    void AddPositionData(Entity entity)
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
            EntitiesPositionData[index].Add(positionData);
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
        for (int i = POSITION_SLICES - 1; i > -1; i--)
        {
            EntitiesPositionData[i].Clear();
        }
    }
}