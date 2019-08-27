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

    void Start()
    {
        _instance = this;

        var entityManager = World.Active.EntityManager;
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(SpriteSheetAnimation_Data)
        );

        var storedNumberOfEnemies = PlayerPrefs.GetInt("NumberOfEnemies");

        NativeArray<Entity> entityArray = new NativeArray<Entity>(storedNumberOfEnemies, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        foreach (Entity entity in entityArray) {
            entityManager.SetComponentData(entity,
                new Translation {
                    Value = new float3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-2.5f, 2.5f), 0)
                }
            );
            entityManager.SetComponentData(entity,
                new SpriteSheetAnimation_Data {
                    currentFrame = UnityEngine.Random.Range(0, 7),
                    frameCount = 7,
                    frameTimer = UnityEngine.Random.Range(0f, 1f),
                    frameTimerMax = .1f
                }
            );
        }

        entityArray.Dispose();
    }
}
