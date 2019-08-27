using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Mesh Mesh;
    public Material Material;

    static GameController _instance;

    public static GameController Instance => _instance;

    EntityManager _entityManager;
    NativeArray<Entity> _entityArray;

    void Start()
    {
        _instance = this;

        _entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = _entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data)
        );

        var storedNumberOfEnemies = PlayerPrefs.GetInt("NumberOfEnemies");

        _entityArray = new NativeArray<Entity>(storedNumberOfEnemies, Allocator.Persistent);
        _entityManager.CreateEntity(entityArchetype, _entityArray);

        foreach (Entity entity in _entityArray) {
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
        }

        //_entityArray.Dispose();
    }

    void OnDestroy()
    {
        DestroyEntities();
    }

    void DestroyEntities()
    {
        _entityManager.DestroyEntity(_entityArray);
        _entityArray.Dispose();
    }
}