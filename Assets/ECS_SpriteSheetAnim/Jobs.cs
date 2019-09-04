using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
    
public struct PositionComparer : IComparer<RenderData> {
    public int Compare(RenderData a, RenderData b) {
        /*if (a.entity == Entity.Null || b.entity == Entity.Null)
        {
            Debug.Log("NULL ENTITY DETECTED");
        }*/

        if (a.position.y < b.position.y)
            return 1;
        else
            return -1;
    }
}

[BurstCompile]
public struct FilterJob : IJobParallelForFilter
{
    [ReadOnly]
    public NativeArray<Entity> nativeArray;

    public float xMin;
    public float xMax;
    public float yBottom; // Bottom cull position

    public float yTop_1; // Top most cull position

    public float MinY;
    public float MaxY;

    [ReadOnly]
    public ComponentDataFromEntity<Translation> translationGetter;

    public bool Execute(int index)
    {
        Translation translation = translationGetter[nativeArray[index]];

        //if (translation.Value.x > xMin && translation.Value.x < xMax && translation.Value.y < yTop_1 && translation.Value.y > yBottom)
        {
            return translation.Value.y > MinY && translation.Value.y <= MaxY;
        }
        /*else
        {
            return false;
        }*/
    }
}


[BurstCompile]
public struct CullAndSortNativeCounterJob : IJobForEachWithEntity<Translation, SpriteSheetAnimation_Data> {

    public float xMin;
    public float xMax;
    public float yBottom; // Bottom cull position

    public float yTop_1; // Top most cull position
    public float yTop_2;
    public float yTop_3;
    public float yTop_4;
    public float yTop_5;
    public float yTop_6;
    public float yTop_7;
    public float yTop_8;
    public float yTop_9;
    public float yTop_10;
    public float yTop_11;
    public float yTop_12;
    public float yTop_13;
    public float yTop_14;
    public float yTop_15;
    public float yTop_16;
    public float yTop_17;
    public float yTop_18;
    public float yTop_19;
    public float yTop_20;

    [NativeDisableContainerSafetyRestriction] public NativeArray<int> indexers;

    public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SpriteSheetAnimation_Data spriteSheetAnimationData) {
        float positionX = translation.Value.x;
        float positionY = translation.Value.y;
        if (positionX > xMin && positionX < xMax && positionY < yTop_1 && positionY > yBottom) {
            if (positionY < yTop_20)
            {
                indexers[19]++;
            }
            else if (positionY < yTop_19)
            {
                indexers[18]++;
            }
            else if (positionY < yTop_18)
            {
                indexers[17]++;
            }
            else if (positionY < yTop_17)
            {
                indexers[16]++;
            }
            else if (positionY < yTop_16)
            {
                indexers[15]++;
            }
            else if (positionY < yTop_15)
            {
                indexers[14]++;
            }
            else if (positionY < yTop_14)
            {
                indexers[13]++;
            }
            else if (positionY < yTop_13)
            {
                indexers[12]++;
            }
            else if (positionY < yTop_12)
            {
                indexers[11]++;
            }
            else if (positionY < yTop_11)
            {
                indexers[10]++;
            }
            else if (positionY < yTop_10)
            {
                indexers[9]++;
            }
            else if (positionY < yTop_9)
            {
                indexers[8]++;
            }
            else if (positionY < yTop_8)
            {
                indexers[7]++;
            }
            else if (positionY < yTop_7)
            {
                indexers[6]++;
            }
            else if (positionY < yTop_6)
            {
                indexers[5]++;
            }
            else if (positionY < yTop_5)
            {
                indexers[4]++;
            }
            else if (positionY < yTop_4)
            {
                indexers[3]++;
            }
            else if (positionY < yTop_3)
            {
                indexers[2]++;
            }
            else if (positionY < yTop_2)
            {
                indexers[1]++;
            }
            else
            {
                indexers[0]++;
            }
        }
    }
}

[BurstCompile]
public struct CullAndSortNativeQueueJob : IJobForEachWithEntity<Translation, SpriteSheetAnimation_Data> {

    public float xMin;
    public float xMax;
    public float yBottom; // Bottom cull position

    public float yTop_1; // Top most cull position
    public float yTop_2;
    public float yTop_3;
    public float yTop_4;
    public float yTop_5;
    public float yTop_6;
    public float yTop_7;
    public float yTop_8;
    public float yTop_9;
    public float yTop_10;
    public float yTop_11;
    public float yTop_12;
    public float yTop_13;
    public float yTop_14;
    public float yTop_15;
    public float yTop_16;
    public float yTop_17;
    public float yTop_18;
    public float yTop_19;
    public float yTop_20;

    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_1;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_2;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_3;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_4;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_5;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_6;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_7;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_8;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_9;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_10;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_11;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_12;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_13;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_14;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_15;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_16;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_17;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_18;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_19;
    [NativeDisableContainerSafetyRestriction] public NativeArray<RenderData> nativeArray_20;

    [NativeDisableContainerSafetyRestriction] public NativeArray<int> indexers;

    public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SpriteSheetAnimation_Data spriteSheetAnimationData) {
        float positionX = translation.Value.x;
        float positionY = translation.Value.y;
        if (positionX > xMin && positionX < xMax && positionY < yTop_1 && positionY > yBottom) {
            // Valid position
            RenderData entityPosition = new RenderData {
                position = translation.Value,
                uv = spriteSheetAnimationData.uv,
                matrix = spriteSheetAnimationData.matrix
            };
            /*if (entity == Entity.Null)
            {
                Debug.Log("NULL ENTITY DETECTED");
            }*/

            //nativeQueue_1.Enqueue(entityPosition); return;

            if (positionY < yTop_20)
            {
                nativeArray_20[indexers[19]] = entityPosition;
                indexers[19]++;
            }
            else if (positionY < yTop_19)
            {
                nativeArray_19[indexers[18]] = entityPosition;
                indexers[18]++;
            }
            else if (positionY < yTop_18)
            {
                nativeArray_18[indexers[17]] = entityPosition;
                indexers[17]++;
            }
            else if (positionY < yTop_17)
            {
                nativeArray_17[indexers[16]] = entityPosition;
                indexers[16]++;
            }
            else if (positionY < yTop_16)
            {
                nativeArray_16[indexers[15]] = entityPosition;
                indexers[15]++;
            }
            else if (positionY < yTop_15)
            {
                nativeArray_15[indexers[14]] = entityPosition;
                indexers[14]++;
            }
            else if (positionY < yTop_14)
            {
                nativeArray_14[indexers[13]] = entityPosition;
                indexers[13]++;
            }
            else if (positionY < yTop_13)
            {
                nativeArray_13[indexers[12]] = entityPosition;
                indexers[12]++;
            }
            else if (positionY < yTop_12)
            {
                nativeArray_12[indexers[11]] = entityPosition;
                indexers[11]++;
            }
            else if (positionY < yTop_11)
            {
                nativeArray_11[indexers[10]] = entityPosition;
                indexers[10]++;
            }
            else if (positionY < yTop_10)
            {
                nativeArray_10[indexers[9]] = entityPosition;
                indexers[9]++;
            }
            else if (positionY < yTop_9)
            {
                nativeArray_9[indexers[8]] = entityPosition;
                indexers[8]++;
            }
            else if (positionY < yTop_8)
            {
                nativeArray_8[indexers[7]] = entityPosition;
                indexers[7]++;
            }
            else if (positionY < yTop_7)
            {
                nativeArray_7[indexers[6]] = entityPosition;
                indexers[6]++;
            }
            else if (positionY < yTop_6)
            {
                nativeArray_6[indexers[5]] = entityPosition;
                indexers[5]++;
            }
            else if (positionY < yTop_5)
            {
                nativeArray_5[indexers[4]] = entityPosition;
                indexers[4]++;
            }
            else if (positionY < yTop_4)
            {
                nativeArray_4[indexers[3]] = entityPosition;
                indexers[3]++;
            }
            else if (positionY < yTop_3)
            {
                nativeArray_3[indexers[2]] = entityPosition;
                indexers[2]++;
            }
            else if (positionY < yTop_2)
            {
                nativeArray_2[indexers[1]] = entityPosition;
                indexers[1]++;
            }
            else
            {
                nativeArray_1[indexers[0]] = entityPosition;
                indexers[0]++;
            }
        }
    }
}

[BurstCompile]
public struct SortByPositionJob : IJob {

    public PositionComparer comparer;
    public NativeArray<RenderData> sortArray;

    public void Execute() {
        sortArray.Sort(comparer);
    }
}

[BurstCompile]
public struct FillArraysParallelJob : IJobParallelFor {

    [ReadOnly] public NativeArray<RenderData> nativeArray;
    [NativeDisableContainerSafetyRestriction] public NativeArray<Matrix4x4> matrixArray;
    [NativeDisableContainerSafetyRestriction] public NativeArray<Vector4> uvArray;
    public int startingIndex;

    public void Execute(int index) {
        RenderData entityPositionWithIndex = nativeArray[index];
        //if (entityPositionWithIndex.entity != Entity.Null)
        {
            matrixArray[startingIndex + index] = entityPositionWithIndex.matrix;
            uvArray[startingIndex + index] = entityPositionWithIndex.uv;
        }
    }
}


// NEW STUFF

[BurstCompile]
public struct NewSortByPositionJob : IJob {

    public NewPositionComparer comparer;
    public NativeList<PositionData> sortArray;

    public void Execute() {
        sortArray.Sort(comparer);
    }
}

public struct NewPositionComparer : IComparer<PositionData> {
    public int Compare(PositionData a, PositionData b) {
        if (a.Position.y < b.Position.y)
            return 1;
        else
            return -1;
    }
}


[BurstCompile]
public struct FillArraysParallelJobNew : IJobParallelFor {

    [ReadOnly] public NativeList<PositionData> positionsData;
    [NativeDisableContainerSafetyRestriction] public NativeArray<Matrix4x4> matrixArray;
    [NativeDisableContainerSafetyRestriction] public NativeArray<Vector4> uvArray;
    public int startingIndex;

    [ReadOnly]
    public ComponentDataFromEntity<SpriteSheetAnimation_Data> animationDataGetter;


    public void Execute(int index) {
        PositionData entityPositionWithIndex = positionsData[index];
        SpriteSheetAnimation_Data animationData = animationDataGetter[entityPositionWithIndex.Entity];

        matrixArray[startingIndex + index] = animationData.matrix;
        uvArray[startingIndex + index] = animationData.uv;
    }
}
