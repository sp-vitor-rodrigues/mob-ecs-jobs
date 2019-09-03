/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Linq;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateAfter(typeof(SpriteSheetAnimation_Animate))]
//[DisableAutoCreation]
public class SpriteSheetRenderer : ComponentSystem {

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

    private NativeArray<JobHandle> jobHandleArray;
    private NativeArray<RenderData>[] nativeArrayArray;
    private PositionComparer positionComparer;
    NativeArray<int> totalCounters;
    NativeArray<int> indexers;
    NativeList<int>[] counters;

    ComponentDataFromEntity<Translation> translationGetter;

    protected override void OnCreate() {
        base.OnCreate();

        nativeArrayArray = new NativeArray<RenderData>[POSITION_SLICES];

        counters = new NativeList<int>[POSITION_SLICES];
        indexers = new NativeArray<int>(POSITION_SLICES, Allocator.Persistent);
        totalCounters = new NativeArray<int>(POSITION_SLICES, Allocator.Persistent);
        for (int i = 0; i < POSITION_SLICES; i++)
        {
            counters[i] = new NativeList<int>(0, Allocator.Persistent);
        }

        jobHandleArray = new NativeArray<JobHandle>(POSITION_SLICES, Allocator.Persistent);

        translationGetter = GetComponentDataFromEntity<Translation>(true);

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
        totalCounters.Dispose();

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
            totalCounters[i] = 0;
        }

        float marginY = camera.orthographicSize / 10f;
        yTop_1 += marginY;
        yBottom -= marginY;

        //var entities = World.Active.EntityManager.GetAllEntities(Allocator.Persistent);
        //var length = entities.Length;
        //translationGetter = GetComponentDataFromEntity<Translation>(true);

        /*for (int i = 0; i < POSITION_SLICES; i++)
        {
            var filterJob = new FilterJob
            {
                nativeArray = entities,
                translationGetter = translationGetter,
                MinY = i < POSITION_SLICES - 1 ? tops[i + 1] : float.MinValue,
                MaxY = tops[i],
                xMin = xMin,
                xMax = xMax,
                yBottom = yBottom,
                yTop_1 = yTop_1,
            };
            var handle = filterJob.ScheduleAppend(counters[i], length, POSITION_SLICES);
            handle.Complete();
        }
        entities.Dispose();*/

        var countersB = new NativeArray<int>(POSITION_SLICES, Allocator.TempJob);
        
        CullAndSortNativeCounterJob cullAndSortNativeCountJob = new CullAndSortNativeCounterJob {
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

            indexers = countersB,
        };
        JobHandle cullAndSortNativeCountJobHandle = cullAndSortNativeCountJob.Schedule(this);
        cullAndSortNativeCountJobHandle.Complete();

        for (int i = 0; i < POSITION_SLICES; i++)
        {
            nativeArrayArray[i] = new NativeArray<RenderData>(countersB[i] + 5, Allocator.TempJob);
        }
        countersB.Dispose();
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

            /*if (nativeArrayArray[i].Length != indexers[i])
            {
                Debug.Log("Differente Lengths!");
            }*/
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

        //CheckAndUpdateArray(matrixArray.ToArray(), uvArray.ToArray(), nativeArrayArray);

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

    Matrix4x4[] matrixArrayTest;
    Vector4[] uvArrayTest;
    RenderData[][] rdArrayArray;

    void CheckAndUpdateArray(Matrix4x4[] matrixes, Vector4[] uvs, NativeArray<RenderData>[] arrayArray)
    {
        if (matrixArrayTest == null)
        {
            matrixArrayTest = matrixes;
            uvArrayTest = uvs;
            rdArrayArray = new RenderData[POSITION_SLICES][];

            for (int i = 0; i < POSITION_SLICES; i++)
            {
                rdArrayArray[i] = arrayArray[i].ToArray();
            }
        }
        else
        {
            if (matrixArrayTest.Length != matrixes.Length)
            {
                matrixArrayTest = matrixes;

                Debug.Log("Matrixes have different size!");
                return;
            }
            if (uvArrayTest.Length != uvs.Length)
            {
                uvArrayTest = uvs;

                Debug.Log("UVs have different size!");
                return;
            }

            int totalDifferentEntities = 0;
            for (int i = 0; i < POSITION_SLICES; i++)
            {
                if (rdArrayArray[i].Length != arrayArray[i].Length)
                {
                    Debug.Log("Arrays have different sizes!");
                    for (int j = 0; j < POSITION_SLICES; j++)
                    {
                        rdArrayArray[j] = arrayArray[j].ToArray();
                    }

                    return;
                }
                int differentEntities = 0;

                for (int j = 0; j < arrayArray[i].Length; j++)
                {
                    if (arrayArray[i][j].entity == Entity.Null)
                    {
                        Debug.Log("A null entity was found at slice " + i + ", index " + j);
                    }

                    if (rdArrayArray[i][j].entity != arrayArray[i][j].entity)
                    {
                        differentEntities++;
                        totalDifferentEntities++;
                        Debug.Log("Received " + arrayArray[i][j].entity + ", expected: " + rdArrayArray[i][j].entity);
                    }
                }

                if (differentEntities > 0)
                {
                    Debug.Log(differentEntities + " different entities were found at slice " + i + "!");
                }
            }
            if (totalDifferentEntities > 0)
            {
                Debug.Log(totalDifferentEntities + " different entities were found at the arrays!");
            }

            /*for (int i = 0; i < matrixArrayTest.Length; i++)
            {
                if (matrixArrayTest[i] != matrixes[i])
                {
                    Debug.Log("Matrixes have different values! Expected " + matrixArrayTest[i] + ", got " + matrixes[i]);
                }
            }*/
            /*for (int i = 0; i < uvArrayTest.Length; i++)
            {
                if (uvArrayTest[i] != uvs[i])
                {
                    Debug.Log("UVs have different values! Expected " + uvArrayTest[i] + ", got " + uvs[i]);
                }
            }*/
        }
    }
}
