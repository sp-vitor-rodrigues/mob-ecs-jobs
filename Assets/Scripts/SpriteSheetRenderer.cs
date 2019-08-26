/*using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpriteSheetRenderer : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Translation translation) =>
        {
            Graphics.DrawMesh(GameController.Instance.Mesh,
                translation.Value,
                Quaternion.identity,
                GameController.Instance.Material,
                0);
        });
    }
}
*/