/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
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
        //material = GameController.Instance.EnemyMaterials;
    }

    private const int POSITION_SLICES = 20;

    private NativeArray<JobHandle> jobHandleArray;
    private NewPositionComparer newPositionComparer;

    ComponentDataFromEntity<SpriteSheetAnimation_Data> animDataGetter;

    protected override void OnCreate() {
        base.OnCreate();

        jobHandleArray = new NativeArray<JobHandle>(POSITION_SLICES, Allocator.Persistent);

        animDataGetter = GetComponentDataFromEntity<SpriteSheetAnimation_Data>(true);

        newPositionComparer = new NewPositionComparer();

    }

    protected override void OnDestroy() {
        base.OnDestroy();

        jobHandleArray.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameController.Instance == null || !GameController.Instance.ReadyToRun)
        {
            return;
        }

        for (int factionsIndex = 0; factionsIndex < (int)SlicePositionData.FactionType.Total; factionsIndex++)
        {
            if (GameController.Instance.FactionsData[factionsIndex] == null)
            {
                continue;
            }

            for (int characterIndex = 0; characterIndex < GameController.Instance.FactionsData[factionsIndex].CharacterData.Length; characterIndex++)
            {
                var characterData = GameController.Instance.FactionsData[factionsIndex].CharacterData[characterIndex];

                int visibleEntityTotal = 0;
                for (int i = 0; i < POSITION_SLICES; i++) {
                    visibleEntityTotal += characterData.SlicePositionData[i].PositionsData.Length;
                }

                // Sort by position
                for (int i = 0; i < POSITION_SLICES; i++)
                {
                    if (characterData.SlicePositionData[i].DataChanged)
                    {
                        var positionsData = characterData.SlicePositionData[i].PositionsData;
                        NewSortByPositionJob sortByPositionJob = new NewSortByPositionJob
                        {
                            sortArray = positionsData,
                            comparer = newPositionComparer
                        };
                        jobHandleArray[i] = sortByPositionJob.Schedule();
                        characterData.SlicePositionData[i].DataChanged = false;
                    }
                }

                JobHandle.CompleteAll(jobHandleArray);

                // Fill up individual Arrays
                NativeArray<Matrix4x4> matrixArray = new NativeArray<Matrix4x4>(visibleEntityTotal, Allocator.TempJob);
                NativeArray<Vector4> uvArray = new NativeArray<Vector4>(visibleEntityTotal, Allocator.TempJob);

                animDataGetter = GetComponentDataFromEntity<SpriteSheetAnimation_Data>(true);

                int startingIndex = 0;
                for (int i = 0; i < POSITION_SLICES; i++)
                {
                    var positionsData = characterData.SlicePositionData[i].PositionsData;
                    FillArraysParallelJobNew fillArraysParallelJob = new FillArraysParallelJobNew {
                        positionsData = positionsData,
                        matrixArray = matrixArray,
                        uvArray = uvArray,
                        startingIndex = startingIndex,
                        animationDataGetter = animDataGetter
                    };
                    startingIndex += positionsData.Length;
                    jobHandleArray[i] = fillArraysParallelJob.Schedule(positionsData.Length, 10);
                }

                JobHandle.CompleteAll(jobHandleArray);

                // Slice Arrays and Draw
                InitDrawMeshInstancedSlicedData();
                for (int i = 0; i < visibleEntityTotal; i += DRAW_MESH_INSTANCED_SLICE_COUNT) {
                    int sliceSize = math.min(visibleEntityTotal- i, DRAW_MESH_INSTANCED_SLICE_COUNT);

                    NativeArray<Matrix4x4>.Copy(matrixArray, i, matrixInstancedArray, 0, sliceSize);
                    NativeArray<Vector4>.Copy(uvArray, i, uvInstancedArray, 0, sliceSize);

                    materialPropertyBlock = new MaterialPropertyBlock();
                    materialPropertyBlock.SetVectorArray(shaderMainTexUVid, uvInstancedArray);

                    Graphics.DrawMeshInstanced(characterData.Mesh, 0, characterData.Material, matrixInstancedArray, sliceSize, materialPropertyBlock);
                }

                matrixArray.Dispose();
                uvArray.Dispose();
            }
        }
    }
}
