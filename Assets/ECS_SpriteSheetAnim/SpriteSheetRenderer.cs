﻿/* 
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
using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(SpriteSheetAnimation_Animate))]
//[DisableAutoCreation]
public class SpriteSheetRenderer : ComponentSystem {

    private struct RenderData {
        public Entity entity;
        public float3 position;
        public Matrix4x4 matrix;
        public Vector4 uv;
    }
    
    private struct PositionComparer : IComparer<RenderData> {
        public int Compare(RenderData a, RenderData b) {
            if (a.position.y < b.position.y)
                return 1;
            else
                return -1;
        }
    }

    [BurstCompile]
    private struct CullAndSortNativeQueueJob : IJobForEachWithEntity<Translation, SpriteSheetAnimation_Data> {

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

        /*public NativeQueue<RenderData>.Concurrent nativeQueue_1;
        public NativeQueue<RenderData>.Concurrent nativeQueue_2;
        public NativeQueue<RenderData>.Concurrent nativeQueue_3;
        public NativeQueue<RenderData>.Concurrent nativeQueue_4;
        public NativeQueue<RenderData>.Concurrent nativeQueue_5;
        public NativeQueue<RenderData>.Concurrent nativeQueue_6;
        public NativeQueue<RenderData>.Concurrent nativeQueue_7;
        public NativeQueue<RenderData>.Concurrent nativeQueue_8;
        public NativeQueue<RenderData>.Concurrent nativeQueue_9;
        public NativeQueue<RenderData>.Concurrent nativeQueue_10;
        public NativeQueue<RenderData>.Concurrent nativeQueue_11;
        public NativeQueue<RenderData>.Concurrent nativeQueue_12;
        public NativeQueue<RenderData>.Concurrent nativeQueue_13;
        public NativeQueue<RenderData>.Concurrent nativeQueue_14;
        public NativeQueue<RenderData>.Concurrent nativeQueue_15;
        public NativeQueue<RenderData>.Concurrent nativeQueue_16;
        public NativeQueue<RenderData>.Concurrent nativeQueue_17;
        public NativeQueue<RenderData>.Concurrent nativeQueue_18;
        public NativeQueue<RenderData>.Concurrent nativeQueue_19;
        public NativeQueue<RenderData>.Concurrent nativeQueue_20;*/

        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_1;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_2;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_3;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_4;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_5;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_6;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_7;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_8;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_9;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_10;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_11;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_12;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_13;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_14;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_15;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_16;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_17;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_18;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_19;
        [NativeDisableContainerSafetyRestriction] public NativeList<RenderData> nativeList_20;

        public void Execute(Entity entity, int index, [ReadOnly] ref Translation translation, [ReadOnly] ref SpriteSheetAnimation_Data spriteSheetAnimationData) {
            float positionX = translation.Value.x;
            float positionY = translation.Value.y;
            if (positionX > xMin && positionX < xMax && positionY < yTop_1 && positionY > yBottom) {
                // Valid position
                RenderData entityPosition = new RenderData {
                    entity = entity,
                    position = translation.Value,
                    uv = spriteSheetAnimationData.uv,
                    matrix = spriteSheetAnimationData.matrix
                };

                //nativeQueue_1.Enqueue(entityPosition); return;

                if (positionY < yTop_20)
                {
                    //nativeQueue_20.Enqueue(entityPosition);
                    nativeList_20.Add(entityPosition);
                }
                else if (positionY < yTop_19)
                {
                    //nativeQueue_19.Enqueue(entityPosition);
                    nativeList_19.Add(entityPosition);
                }
                else if (positionY < yTop_18)
                {
                    //nativeQueue_18.Enqueue(entityPosition);
                    nativeList_18.Add(entityPosition);
                }
                else if (positionY < yTop_17)
                {
                    //nativeQueue_17.Enqueue(entityPosition);
                    nativeList_17.Add(entityPosition);
                }
                else if (positionY < yTop_16)
                {
                    //nativeQueue_16.Enqueue(entityPosition);
                    nativeList_16.Add(entityPosition);
                }
                else if (positionY < yTop_15)
                {
                    //nativeQueue_15.Enqueue(entityPosition);
                    nativeList_15.Add(entityPosition);
                }
                else if (positionY < yTop_14)
                {
                    //nativeQueue_14.Enqueue(entityPosition);
                    nativeList_14.Add(entityPosition);
                }
                else if (positionY < yTop_13)
                {
                    //nativeQueue_13.Enqueue(entityPosition);
                    nativeList_13.Add(entityPosition);
                }
                else if (positionY < yTop_12)
                {
                    //nativeQueue_12.Enqueue(entityPosition);
                    nativeList_12.Add(entityPosition);
                }
                else if (positionY < yTop_11)
                {
                    //nativeQueue_11.Enqueue(entityPosition);
                    nativeList_11.Add(entityPosition);
                }
                else if (positionY < yTop_10)
                {
                    //nativeQueue_10.Enqueue(entityPosition);
                    nativeList_10.Add(entityPosition);
                }
                else if (positionY < yTop_9)
                {
                    //nativeQueue_9.Enqueue(entityPosition);
                    nativeList_9.Add(entityPosition);
                }
                else if (positionY < yTop_8)
                {
                    //nativeQueue_8.Enqueue(entityPosition);
                    nativeList_8.Add(entityPosition);
                }
                else if (positionY < yTop_7)
                {
                    //nativeQueue_7.Enqueue(entityPosition);
                    nativeList_7.Add(entityPosition);
                }
                else if (positionY < yTop_6)
                {
                    //nativeQueue_6.Enqueue(entityPosition);
                    nativeList_6.Add(entityPosition);
                }
                else if (positionY < yTop_5)
                {
                    //nativeQueue_5.Enqueue(entityPosition);
                    nativeList_5.Add(entityPosition);
                }
                else if (positionY < yTop_4)
                {
                    //nativeQueue_4.Enqueue(entityPosition);
                    nativeList_4.Add(entityPosition);
                }
                else if (positionY < yTop_3)
                {
                    //nativeQueue_3.Enqueue(entityPosition);
                    nativeList_3.Add(entityPosition);
                }
                else if (positionY < yTop_2)
                {
                    //nativeQueue_2.Enqueue(entityPosition);
                    nativeList_2.Add(entityPosition);
                }
                else
                {
                    //nativeQueue_1.Enqueue(entityPosition);
                    nativeList_1.Add(entityPosition);
                }
            }
        }
    }

    NativeQueue<RenderData> queue;
    /*[BurstCompile]
    private struct NativeQueueToArrayJob : IJob {

        public NativeQueue<RenderData> nativeQueue;
        public NativeArray<RenderData> nativeArray;

        public void Execute() {
            int index = 0;
            RenderData entity;
            while (nativeQueue.TryDequeue(out entity)) {
                nativeArray[index] = entity;
                index++;
            }
        }
    }*/

    [BurstCompile]
    private struct SortByPositionJob : IJob {

        public PositionComparer comparer;
        public NativeArray<RenderData> sortArray;

        public void Execute() {
            sortArray.Sort(comparer);
        }
    }

    [BurstCompile]
    private struct FillArraysParallelJob : IJobParallelFor {

        [ReadOnly] public NativeArray<RenderData> nativeArray;
        [NativeDisableContainerSafetyRestriction] public NativeArray<Matrix4x4> matrixArray;
        [NativeDisableContainerSafetyRestriction] public NativeArray<Vector4> uvArray;
        public int startingIndex;

        public void Execute(int index) {
            RenderData entityPositionWithIndex = nativeArray[index];
            matrixArray[startingIndex + index] = entityPositionWithIndex.matrix;
            uvArray[startingIndex + index] = entityPositionWithIndex.uv;
        }
    }

    [BurstCompile]
    private struct ClearQueueJob : IJob {
        //public NativeQueue<RenderData> nativeQueue;
        public NativeList<RenderData> nativeList;
        public void Execute() {
            //nativeQueue.Clear();
            nativeList.Clear();
        }
    }

    private const int DRAW_MESH_INSTANCED_SLICE_COUNT = 1023;
    private Matrix4x4[] matrixInstancedArray;
    private Vector4[] uvInstancedArray;
    private MaterialPropertyBlock materialPropertyBlock;
    private Mesh mesh;
    private Material material;
    private int shaderMainTexUVid;

    private void InitDrawMeshInstancedSlicedData() {
        if (matrixInstancedArray != null) return; // Already initialized
        matrixInstancedArray = new Matrix4x4[DRAW_MESH_INSTANCED_SLICE_COUNT];
        uvInstancedArray = new Vector4[DRAW_MESH_INSTANCED_SLICE_COUNT];
        materialPropertyBlock = new MaterialPropertyBlock();
        shaderMainTexUVid = Shader.PropertyToID("_MainTex_UV");
        mesh = GameController.Instance.Mesh;
        material = GameController.Instance.Material;
    }

    private const int POSITION_SLICES = 20;

    //private NativeQueue<RenderData>[] nativeQueueArray;
    private NativeList<RenderData>[] nativeListArray;
    private NativeArray<JobHandle> jobHandleArray;
    //private NativeArray<RenderData>[] nativeArrayArray;
    private PositionComparer positionComparer;
    //NativeMultiHashMap<int, NativeList<RenderData>> hashMap;

    protected override void OnCreate() {
        base.OnCreate();

        //nativeQueueArray = new NativeQueue<RenderData>[POSITION_SLICES];
        nativeListArray = new NativeList<RenderData>[POSITION_SLICES];
        //hashMap = new NativeMultiHashMap<int, NativeList<RenderData>>();

        for (int i = 0; i < POSITION_SLICES; i++) {
            //nativeQueueArray[i] = new NativeQueue<RenderData>(Allocator.Persistent);
            nativeListArray[i] = new NativeList<RenderData>(Allocator.Persistent);
            //hashMap.Add(i, new NativeList<RenderData>(Allocator.Persistent));
        }

        jobHandleArray = new NativeArray<JobHandle>(POSITION_SLICES, Allocator.Persistent);

        //nativeArrayArray = new NativeArray<RenderData>[POSITION_SLICES];

        positionComparer = new PositionComparer();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        for (int i = 0; i < POSITION_SLICES; i++) {
            nativeListArray[i].Dispose();
        }

        jobHandleArray.Dispose();
    }

    protected override void OnUpdate() {

        for (int i = 0; i < POSITION_SLICES; i++) {
            ClearQueueJob clearQueueJob = new ClearQueueJob {
                //nativeQueue = nativeQueueArray[i],
                nativeList = nativeListArray[i]
            };
            jobHandleArray[i] = clearQueueJob.Schedule();
        }

        JobHandle.CompleteAll(jobHandleArray);

        Camera camera = Camera.main;
        float cameraWidth = camera.aspect * camera.orthographicSize;
        float3 cameraPosition = camera.transform.position;
        float marginX = cameraWidth / 10f;
        float xMin = cameraPosition.x - cameraWidth - marginX;
        float xMax = cameraPosition.x + cameraWidth + marginX;
        float cameraSliceSize = camera.orthographicSize * 2f / POSITION_SLICES;
        float yBottom = cameraPosition.y - camera.orthographicSize; // Bottom cull position
        float yTop_1 = cameraPosition.y + camera.orthographicSize; // Top most cull position

        float yTop_2 = yTop_1 - cameraSliceSize * 1f;
        float yTop_3 = yTop_1 - cameraSliceSize * 2f;
        float yTop_4 = yTop_1 - cameraSliceSize * 3f;
        float yTop_5 = yTop_1 - cameraSliceSize * 4f;
        float yTop_6 = yTop_1 - cameraSliceSize * 5f;
        float yTop_7 = yTop_1 - cameraSliceSize * 6f;
        float yTop_8 = yTop_1 - cameraSliceSize * 7f;
        float yTop_9 = yTop_1 - cameraSliceSize * 8f;
        float yTop_10 = yTop_1 - cameraSliceSize * 9f;
        float yTop_11 = yTop_1 - cameraSliceSize * 10f;
        float yTop_12 = yTop_1 - cameraSliceSize * 11f;
        float yTop_13 = yTop_1 - cameraSliceSize * 12f;
        float yTop_14 = yTop_1 - cameraSliceSize * 13f;
        float yTop_15 = yTop_1 - cameraSliceSize * 14f;
        float yTop_16 = yTop_1 - cameraSliceSize * 15f;
        float yTop_17 = yTop_1 - cameraSliceSize * 16f;
        float yTop_18 = yTop_1 - cameraSliceSize * 17f;
        float yTop_19 = yTop_1 - cameraSliceSize * 18f;
        float yTop_20 = yTop_1 - cameraSliceSize * 19f;

        float marginY = camera.orthographicSize / 10f;
        yTop_1 += marginY;
        yBottom -= marginY;

        CullAndSortNativeQueueJob cullAndSortNativeQueueJob = new CullAndSortNativeQueueJob {
            xMin = xMin,
            xMax = xMax,
            yBottom = yBottom,

            yTop_1 = yTop_1,
            yTop_2 = yTop_2,
            yTop_3 = yTop_3,
            yTop_4 = yTop_4,
            yTop_5 = yTop_5,
            yTop_6 = yTop_6,
            yTop_7 = yTop_7,
            yTop_8 = yTop_8,
            yTop_9 = yTop_9,
            yTop_10 = yTop_10,
            yTop_11 = yTop_11,
            yTop_12 = yTop_12,
            yTop_13 = yTop_13,
            yTop_14 = yTop_14,
            yTop_15 = yTop_15,
            yTop_16 = yTop_16,
            yTop_17 = yTop_17,
            yTop_18 = yTop_18,
            yTop_19 = yTop_19,
            yTop_20 = yTop_20,

            /*nativeQueue_1 = nativeQueueArray[0].ToConcurrent(),
            nativeQueue_2 = nativeQueueArray[1].ToConcurrent(),
            nativeQueue_3 = nativeQueueArray[2].ToConcurrent(),
            nativeQueue_4 = nativeQueueArray[3].ToConcurrent(),
            nativeQueue_5 = nativeQueueArray[4].ToConcurrent(),
            nativeQueue_6 = nativeQueueArray[5].ToConcurrent(),
            nativeQueue_7 = nativeQueueArray[6].ToConcurrent(),
            nativeQueue_8 = nativeQueueArray[7].ToConcurrent(),
            nativeQueue_9 = nativeQueueArray[8].ToConcurrent(),
            nativeQueue_10 = nativeQueueArray[9].ToConcurrent(),
            nativeQueue_11 = nativeQueueArray[10].ToConcurrent(),
            nativeQueue_12 = nativeQueueArray[11].ToConcurrent(),
            nativeQueue_13 = nativeQueueArray[12].ToConcurrent(),
            nativeQueue_14 = nativeQueueArray[13].ToConcurrent(),
            nativeQueue_15 = nativeQueueArray[14].ToConcurrent(),
            nativeQueue_16 = nativeQueueArray[15].ToConcurrent(),
            nativeQueue_17 = nativeQueueArray[16].ToConcurrent(),
            nativeQueue_18 = nativeQueueArray[17].ToConcurrent(),
            nativeQueue_19 = nativeQueueArray[18].ToConcurrent(),
            nativeQueue_20 = nativeQueueArray[19].ToConcurrent(),*/

            nativeList_1 = nativeListArray[0],
            nativeList_2 = nativeListArray[1],
            nativeList_3 = nativeListArray[2],
            nativeList_4 = nativeListArray[3],
            nativeList_5 = nativeListArray[4],
            nativeList_6 = nativeListArray[5],
            nativeList_7 = nativeListArray[6],
            nativeList_8 = nativeListArray[7],
            nativeList_9 = nativeListArray[8],
            nativeList_10 = nativeListArray[9],
            nativeList_11 = nativeListArray[10],
            nativeList_12 = nativeListArray[11],
            nativeList_13 = nativeListArray[12],
            nativeList_14 = nativeListArray[13],
            nativeList_15 = nativeListArray[14],
            nativeList_16 = nativeListArray[15],
            nativeList_17 = nativeListArray[16],
            nativeList_18 = nativeListArray[17],
            nativeList_19 = nativeListArray[18],
            nativeList_20 = nativeListArray[19],
        };
        JobHandle cullAndSortNativeQueueJobHandle = cullAndSortNativeQueueJob.Schedule(this);
        cullAndSortNativeQueueJobHandle.Complete();

        int visibleEntityTotal = 0;
        for (int i = 0; i < POSITION_SLICES; i++) {
            visibleEntityTotal += nativeListArray[i].Length;
        }


        /*for (int i = 0; i < POSITION_SLICES; i++) {
            NativeArray<RenderData> nativeArray = new NativeArray<RenderData>(nativeQueueArray[i].Count, Allocator.TempJob);
            nativeArrayArray[i] = nativeArray;
        }*/


        /*for (int i = 0; i < POSITION_SLICES; i++) {
            NativeQueueToArrayJob nativeQueueToArrayJob = new NativeQueueToArrayJob {
                nativeQueue = nativeQueueArray[i],
                nativeArray = nativeArrayArray[i],
            };
            jobHandleArray[i] = nativeQueueToArrayJob.Schedule();
        }

        JobHandle.CompleteAll(jobHandleArray);*/

        // Sort by position
        for (int i = 0; i < POSITION_SLICES; i++) {
            SortByPositionJob sortByPositionJob = new SortByPositionJob {
                sortArray = nativeListArray[i],
                comparer = positionComparer
            };
            jobHandleArray[i] = sortByPositionJob.Schedule();
        }

        JobHandle.CompleteAll(jobHandleArray);

        // Fill up individual Arrays
        NativeArray<Matrix4x4> matrixArray = new NativeArray<Matrix4x4>(visibleEntityTotal, Allocator.TempJob);
        NativeArray<Vector4> uvArray = new NativeArray<Vector4>(visibleEntityTotal, Allocator.TempJob);

        int startingIndex = 0;
        for (int i = 0; i < POSITION_SLICES; i++) {
            //if (i != 4) continue;
            FillArraysParallelJob fillArraysParallelJob = new FillArraysParallelJob {
                nativeArray = nativeListArray[i],
                matrixArray = matrixArray,
                uvArray = uvArray,
                startingIndex = startingIndex
            };
            startingIndex += nativeListArray[i].Length;
            jobHandleArray[i] = fillArraysParallelJob.Schedule(nativeListArray[i].Length, 10);
        }

        JobHandle.CompleteAll(jobHandleArray);

        //jobHandleArray.Dispose();

        /*for (int i = 0; i < POSITION_SLICES; i++) {
            nativeArrayArray[i].Dispose();
        }*/


        // Slice Arrays and Draw
        InitDrawMeshInstancedSlicedData();
        for (int i = 0; i < visibleEntityTotal; i += DRAW_MESH_INSTANCED_SLICE_COUNT) {
            int sliceSize = math.min(visibleEntityTotal- i, DRAW_MESH_INSTANCED_SLICE_COUNT);

            NativeArray<Matrix4x4>.Copy(matrixArray, i, matrixInstancedArray, 0, sliceSize);
            NativeArray<Vector4>.Copy(uvArray, i, uvInstancedArray, 0, sliceSize);

            materialPropertyBlock.SetVectorArray(shaderMainTexUVid, uvInstancedArray);

            Graphics.DrawMeshInstanced(mesh, 0, material, matrixInstancedArray, sliceSize, materialPropertyBlock);
        }

        matrixArray.Dispose();
        uvArray.Dispose();
    }

}
