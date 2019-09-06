/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
//using CodeMonkey.Utils;


public struct QuadrantEntity : IComponentData {
    public TypeEnum typeEnum;

    public enum TypeEnum {
        Defender,
        Attacker
    }
}

public struct QuadrantData {
    public Entity entity;
    public float3 position;
    public QuadrantEntity quadrantEntity;
}

public class QuadrantSystem : ComponentSystem {

    public static NativeMultiHashMap<int, QuadrantData> quadrantMultiHashMap;

    public const int quadrantYMultiplier = 1000;
    private const int quadrantCellSize = 5;

    public static int GetPositionHashMapKey(float3 position) {
        return (int) (math.floor(position.x / quadrantCellSize) + (quadrantYMultiplier * math.floor(position.y / quadrantCellSize)));
    }

    private static void DebugDrawQuadrant(float3 position) {
        Vector3 lowerLeft = new Vector3(math.floor(position.x / quadrantCellSize) * quadrantCellSize, math.floor(position.y / quadrantCellSize) * quadrantCellSize);
        Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+1, +0) * quadrantCellSize);
        Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+0, +1) * quadrantCellSize);
        Debug.DrawLine(lowerLeft + new Vector3(+1, +0) * quadrantCellSize, lowerLeft + new Vector3(+1, +1) * quadrantCellSize);
        Debug.DrawLine(lowerLeft + new Vector3(+0, +1) * quadrantCellSize, lowerLeft + new Vector3(+1, +1) * quadrantCellSize);
        //Debug.Log(GetPositionHashMapKey(position) + " " + position);
    }

    private static int GetEntityCountInHashMap(NativeMultiHashMap<int, QuadrantData> quadrantMultiHashMap, int hashMapKey) {
        QuadrantData quadrantData;
        NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;
        int count = 0;
        if (quadrantMultiHashMap.TryGetFirstValue(hashMapKey, out quadrantData, out nativeMultiHashMapIterator)) {
            do {
                count++;
            } while (quadrantMultiHashMap.TryGetNextValue(out quadrantData, ref nativeMultiHashMapIterator));
        }
        return count;
    }

    [BurstCompile]
    private struct SetQuadrantDataHashMapJob : IJobForEachWithEntity<Translation, QuadrantEntity> {

        public NativeMultiHashMap<int, QuadrantData>.Concurrent quadrantMultiHashMap;

        public void Execute(Entity entity, int index, ref Translation translation, ref QuadrantEntity quadrantEntity) {
            int hashMapKey = GetPositionHashMapKey(translation.Value);
            quadrantMultiHashMap.Add(hashMapKey, new QuadrantData {
                entity = entity,
                position = translation.Value,
                quadrantEntity = quadrantEntity
            });
        }

    }

    protected override void OnCreate() {
        quadrantMultiHashMap = new NativeMultiHashMap<int, QuadrantData>(0, Allocator.Persistent);
        base.OnCreate();
    }

    protected override void OnDestroy() {
        quadrantMultiHashMap.Dispose();
        base.OnDestroy();
    }

    protected override void OnUpdate() {
        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(QuadrantEntity));

        quadrantMultiHashMap.Clear();
        if (entityQuery.CalculateEntityCount() > quadrantMultiHashMap.Capacity) {
            quadrantMultiHashMap.Capacity = entityQuery.CalculateEntityCount();
        }

        SetQuadrantDataHashMapJob setQuadrantDataHashMapJob = new SetQuadrantDataHashMapJob {
            quadrantMultiHashMap = quadrantMultiHashMap.ToConcurrent(),
        };
        JobHandle jobHandle = JobForEachExtensions.Schedule(setQuadrantDataHashMapJob, entityQuery);
        jobHandle.Complete();

        //var position = GameController.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        //DebugDrawQuadrant(position);
        //Debug.Log(GetEntityCountInHashMap(quadrantMultiHashMap, GetPositionHashMapKey(position)));
    }

}

public struct Unit : IComponentData { }
public struct Target : IComponentData { }

public struct HasTarget : IComponentData {
    public Entity targetEntity;
}

[DisableAutoCreation]
public class HasTargetDebug : ComponentSystem {

    protected override void OnUpdate() {
        Entities.ForEach((Entity entity, ref Translation translation, ref HasTarget hasTarget) => {
            if (World.Active.EntityManager.Exists(hasTarget.targetEntity)) {
                Translation targetTranslation = World.Active.EntityManager.GetComponentData<Translation>(hasTarget.targetEntity);
                Debug.DrawLine(translation.Value, targetTranslation.Value);
            }
        });
    }
}