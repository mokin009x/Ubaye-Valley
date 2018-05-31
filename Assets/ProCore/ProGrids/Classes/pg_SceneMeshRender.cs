#if UNITY_EDITOR
using UnityEngine;

/**
 * Despite the MonoBehaviour inheritance, this is an Editor-only script.
 */
[ExecuteInEditMode]
public class pg_SceneMeshRender : MonoBehaviour
{
    public Material material;

    public Mesh mesh;

    // HideFlags.DontSaveInEditor isn't exposed for whatever reason, so do the bit math on ints 
    // and just cast to HideFlags.
    // HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.NotEditable
    private readonly HideFlags SceneCameraHideFlags = (HideFlags) (1 | 4 | 8);

    private void OnDestroy()
    {
        if (mesh) DestroyImmediate(mesh);
        if (material) DestroyImmediate(material);
    }

    private void OnRenderObject()
    {
        // instead of relying on 'SceneCamera' string comparison, check if the hideflags match.
        // this could probably even just check for one bit match, since chances are that any 
        // game view camera isn't going to have hideflags set.
        if ((Camera.current.gameObject.hideFlags & SceneCameraHideFlags) != SceneCameraHideFlags ||
            Camera.current.name != "SceneCamera")
            return;

        if (material == null || mesh == null)
        {
            DestroyImmediate(gameObject);
            // Debug.Log("NULL MESH || MATERIAL");
            return;
        }

        material.SetPass(0);
        Graphics.DrawMeshNow(mesh, Vector3.zero, Quaternion.identity, 0);
    }
}
#endif