/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

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
                    entity = entity,
                    position = translation.Value,
                    uv = spriteSheetAnimationData.uv,
                    matrix = spriteSheetAnimationData.matrix
                };

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
    private struct FilterJob : IJobParallelForFilter
    {
        [ReadOnly]
        public NativeArray<Entity> nativeArray;

        public float MinY;
        public float MaxY;

        [ReadOnly]
        public ComponentDataFromEntity<Translation> translationGetter;

        public bool Execute(int index)
        {
            Translation translation = translationGetter[nativeArray[index]];
            return translation.Value.y > MinY && translation.Value.y <= MaxY;
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
    //private NativeList<RenderData>[] nativeListArray;
    private NativeArray<JobHandle> jobHandleArray;
    private NativeArray<RenderData>[] nativeArrayArray;
    private PositionComparer positionComparer;
//    NativeArray<int> counters;
    NativeArray<int> indexers;
    NativeList<int>[] counters;


    ComponentDataFromEntity<Translation> translationGetter;


    //NativeMultiHashMap<int, NativeList<RenderData>> hashMap;

    protected override void OnCreate() {
        base.OnCreate();

        //nativeQueueArray = new NativeQueue<RenderData>[POSITION_SLICES];
        nativeArrayArray = new NativeArray<RenderData>[POSITION_SLICES];

        counters = new NativeList<int>[POSITION_SLICES];
        indexers = new NativeArray<int>(POSITION_SLICES, Allocator.Persistent);
        for (int i = 0; i < POSITION_SLICES; i++)
        {
            counters[i] = new NativeList<int>(0, Allocator.Persistent);
        }
        //hashMap = new NativeMultiHashMap<int, NativeList<RenderData>>();

        /*for (int i = 0; i < POSITION_SLICES; i++) {
            //nativeQueueArray[i] = new NativeQueue<RenderData>(Allocator.Persistent);
            nativeListArray[i] = new NativeList<RenderData>(Allocator.Persistent);
            //hashMap.Add(i, new NativeList<RenderData>(Allocator.Persistent));
        }*/

        jobHandleArray = new NativeArray<JobHandle>(POSITION_SLICES, Allocator.Persistent);

        translationGetter = GetComponentDataFromEntity<Translation>(true);

        //nativeArrayArray = new NativeArray<RenderData>[POSITION_SLICES];

        positionComparer = new PositionComparer();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        for (int i = 0; i < POSITION_SLICES; i++) {
            if(nativeArrayArray[i].IsCreated)
                nativeArrayArray[i].Dispose();
            counters[i].Clear();
            counters[i].Dispose();
        }
        indexers.Dispose();

        jobHandleArray.Dispose();
    }

    protected override void OnUpdate()
    {
        for (int i = 0; i < POSITION_SLICES; i++)
        {
            counters[i].Clear();
        }

        Camera camera = Camera.main;
        float cameraWidth = camera.aspect * camera.orthographicSize;
        float3 cameraPosition = camera.transform.position;
        float marginX = cameraWidth / 10f;
        float xMin = cameraPosition.x - cameraWidth - marginX;
        float xMax = cameraPosition.x + cameraWidth + marginX;
        float cameraSliceSize = camera.orthographicSize * 2f / POSITION_SLICES;
        float yBottom = cameraPosition.y - camera.orthographicSize; // Bottom cull position

        float[] tops = new float[POSITION_SLICES];
        float yTop_1 = tops[0] = cameraPosition.y + camera.orthographicSize; // Top most cull position

        float yTop_2 = tops[1] = yTop_1 - cameraSliceSize * 1f;
        float yTop_3 = tops[2] = yTop_1 - cameraSliceSize * 2f;
        float yTop_4 = tops[3] = yTop_1 - cameraSliceSize * 3f;
        float yTop_5 = tops[4] = yTop_1 - cameraSliceSize * 4f;
        float yTop_6 = tops[5] = yTop_1 - cameraSliceSize * 5f;
        float yTop_7 = tops[6] = yTop_1 - cameraSliceSize * 6f;
        float yTop_8 = tops[7] = yTop_1 - cameraSliceSize * 7f;
        float yTop_9 = tops[8] = yTop_1 - cameraSliceSize * 8f;
        float yTop_10 = tops[9] = yTop_1 - cameraSliceSize * 9f;
        float yTop_11 = tops[10] = yTop_1 - cameraSliceSize * 10f;
        float yTop_12 = tops[11] = yTop_1 - cameraSliceSize * 11f;
        float yTop_13 = tops[12] = yTop_1 - cameraSliceSize * 12f;
        float yTop_14 = tops[13] = yTop_1 - cameraSliceSize * 13f;
        float yTop_15 = tops[14] = yTop_1 - cameraSliceSize * 14f;
        float yTop_16 = tops[15] = yTop_1 - cameraSliceSize * 15f;
        float yTop_17 = tops[16] = yTop_1 - cameraSliceSize * 16f;
        float yTop_18 = tops[17] = yTop_1 - cameraSliceSize * 17f;
        float yTop_19 = tops[18] = yTop_1 - cameraSliceSize * 18f;
        float yTop_20 = tops[19] = yTop_1 - cameraSliceSize * 19f;

        for (int i = 0; i < POSITION_SLICES; i++)
        {
            indexers[i] = 0;
        }

        float marginY = camera.orthographicSize / 10f;
        yTop_1 += marginY;
        yBottom -= marginY;

        translationGetter = GetComponentDataFromEntity<Translation>(true);

        for (int i = 0; i < POSITION_SLICES; i++)
        {
            var filterJob = new FilterJob
            {
                nativeArray = GameController.Instance.EntityArray,
                translationGetter = translationGetter,
                MinY = i < POSITION_SLICES - 1 ? tops[i + 1] : float.MinValue,
                MaxY = tops[i],
            };
            var handle = filterJob.ScheduleAppend(counters[i], GameController.Instance.EntityArray.Length, POSITION_SLICES);
            handle.Complete();
        }
        for (int i = 0; i < POSITION_SLICES; i++)
        {
            nativeArrayArray[i] = new NativeArray<RenderData>(counters[i].Length, Allocator.TempJob);
        }
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

            indexers = indexers,

            nativeArray_1 = nativeArrayArray[0],
            nativeArray_2 = nativeArrayArray[1],
            nativeArray_3 = nativeArrayArray[2],
            nativeArray_4 = nativeArrayArray[3],
            nativeArray_5 = nativeArrayArray[4],
            nativeArray_6 = nativeArrayArray[5],
            nativeArray_7 = nativeArrayArray[6],
            nativeArray_8 = nativeArrayArray[7],
            nativeArray_9 = nativeArrayArray[8],
            nativeArray_10 = nativeArrayArray[9],
            nativeArray_11 = nativeArrayArray[10],
            nativeArray_12 = nativeArrayArray[11],
            nativeArray_13 = nativeArrayArray[12],
            nativeArray_14 = nativeArrayArray[13],
            nativeArray_15 = nativeArrayArray[14],
            nativeArray_16 = nativeArrayArray[15],
            nativeArray_17 = nativeArrayArray[16],
            nativeArray_18 = nativeArrayArray[17],
            nativeArray_19 = nativeArrayArray[18],
            nativeArray_20 = nativeArrayArray[19],
        };
        JobHandle cullAndSortNativeQueueJobHandle = cullAndSortNativeQueueJob.Schedule(this);
        cullAndSortNativeQueueJobHandle.Complete();

        int visibleEntityTotal = 0;
        for (int i = 0; i < POSITION_SLICES; i++) {
            visibleEntityTotal += nativeArrayArray[i].Length;
        }

        // Sort by position
        for (int i = 0; i < POSITION_SLICES; i++) {
            SortByPositionJob sortByPositionJob = new SortByPositionJob {
                sortArray = nativeArrayArray[i],
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
                nativeArray = nativeArrayArray[i],
                matrixArray = matrixArray,
                uvArray = uvArray,
                startingIndex = startingIndex
            };
            startingIndex += nativeArrayArray[i].Length;
            jobHandleArray[i] = fillArraysParallelJob.Schedule(nativeArrayArray[i].Length, 10);
        }

        JobHandle.CompleteAll(jobHandleArray);

        //jobHandleArray.Dispose();

        for (int i = 0; i < POSITION_SLICES; i++) {
            nativeArrayArray[i].Dispose();
        }


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
