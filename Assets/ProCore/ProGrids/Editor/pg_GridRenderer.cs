using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProGrids
{
    public class pg_GridRenderer
    {
        private const string PREVIEW_OBJECT_NAME = "ProGridsGridObject";
        private const string MATERIAL_OBJECT_NAME = "ProGridsMaterialObject";
        private const string MESH_OBJECT_NAME = "ProGridsMeshObject";
        private const string GRID_SHADER = "Hidden/ProGrids/pg_GridShader";
        private const int MAX_LINES = 256;
        private static readonly HideFlags PG_HIDE_FLAGS = HideFlags.HideAndDontSave;

        private static GameObject gridObject;
        private static Mesh gridMesh;
        private static Material gridMaterial;

        public static int majorLineIncrement = 10;

        private static int tan_iter, bit_iter, max = MAX_LINES, div = 1;

        /**
         * Destroy any existing render objects, then initialize new ones.
         */
        public static void Init()
        {
            Destroy();

            gridObject = EditorUtility.CreateGameObjectWithHideFlags(PREVIEW_OBJECT_NAME, PG_HIDE_FLAGS,
                typeof(MeshFilter), typeof(MeshRenderer));
            majorLineIncrement = EditorPrefs.GetInt(pg_Constant.MajorLineIncrement, 10);
            if (majorLineIncrement < 2)
                majorLineIncrement = 2;

            // Force the mesh to only render in SceneView
            var renderer = gridObject.AddComponent<pg_SceneMeshRender>();

            gridMesh = new Mesh();
            gridMesh.name = MESH_OBJECT_NAME;
            gridMesh.hideFlags = PG_HIDE_FLAGS;

            gridMaterial = new Material(Shader.Find(GRID_SHADER));
            gridMaterial.name = MATERIAL_OBJECT_NAME;
            gridMaterial.hideFlags = PG_HIDE_FLAGS;

            renderer.mesh = gridMesh;
            renderer.material = gridMaterial;
        }

        public static void Destroy()
        {
            DestoryObjectsWithName(MESH_OBJECT_NAME, typeof(Mesh));
            DestoryObjectsWithName(MATERIAL_OBJECT_NAME, typeof(Material));
            DestoryObjectsWithName(PREVIEW_OBJECT_NAME, typeof(GameObject));
        }

        private static void DestoryObjectsWithName(string Name, Type type)
        {
            IEnumerable go = Resources.FindObjectsOfTypeAll(type).Where(x => x.name.Contains(Name));

            foreach (Object t in go) Object.DestroyImmediate(t);
        }

        /**
         * Returns the distance this grid is drawing
         */
        public static float DrawPlane(Camera cam, Vector3 pivot, Vector3 tangent, Vector3 bitangent, float snapValue,
            Color color, float alphaBump)
        {
            if (!gridMesh || !gridMaterial || !gridObject)
                Init();

            gridMaterial.SetFloat("_AlphaCutoff", .1f);
            gridMaterial.SetFloat("_AlphaFade", .6f);

            pivot = pg_Util.SnapValue(pivot, snapValue);

            var p = cam.WorldToViewportPoint(pivot);
            var inFrustum = p.x >= 0f && p.x <= 1f && p.y >= 0f && p.y <= 1f && p.z >= 0f;

            var distances = GetDistanceToFrustumPlanes(cam, pivot, tangent, bitangent, 24f);

            if (inFrustum)
            {
                tan_iter = (int) Mathf.Ceil((Mathf.Abs(distances[0]) + Mathf.Abs(distances[2])) / snapValue);
                bit_iter = (int) Mathf.Ceil((Mathf.Abs(distances[1]) + Mathf.Abs(distances[3])) / snapValue);

                max = Mathf.Max(tan_iter, bit_iter);

                // if the max is around 3x greater than min, we're probably skewing the camera at near-plane
                // angle, so use the min instead.
                if (max > Mathf.Min(tan_iter, bit_iter) * 2)
                    max = Mathf.Min(tan_iter, bit_iter) * 2;

                div = 1;

                var dot = Vector3.Dot(cam.transform.position - pivot, Vector3.Cross(tangent, bitangent));

                if (max > MAX_LINES)
                {
                    if (Vector3.Distance(cam.transform.position, pivot) > 50f * snapValue && Mathf.Abs(dot) > .8f)
                        while (max / div > MAX_LINES)
                            div += div;
                    else
                        max = MAX_LINES;
                }
            }

            // origin, tan, bitan, increment, iterations, divOffset, color, primary alpha bump
            DrawFullGrid(cam, pivot, tangent, bitangent, snapValue * div, max / div, div, color, alphaBump);

            return snapValue * div * (max / div);
        }

        public static void DrawGridPerspective(Camera cam, Vector3 pivot, float snapValue, Color[] colors,
            float alphaBump)
        {
            if (!gridMesh || !gridMaterial || !gridObject)
                Init();

            gridMaterial.SetFloat("_AlphaCutoff", 0f);
            gridMaterial.SetFloat("_AlphaFade", 0f);

            var camDir = (pivot - cam.transform.position).normalized;
            pivot = pg_Util.SnapValue(pivot, snapValue);

            // Used to flip the grid to match whatever direction the cam is currently
            // coming at the pivot from
            var right = camDir.x < 0f ? Vector3.right : Vector3.right * -1f;
            var up = camDir.y < 0f ? Vector3.up : Vector3.up * -1f;
            var forward = camDir.z < 0f ? Vector3.forward : Vector3.forward * -1f;

            // Get intersecting point for each axis, if it exists
            var ray_x = new Ray(pivot, right);
            var ray_y = new Ray(pivot, up);
            var ray_z = new Ray(pivot, forward);

            float x_dist = 10f, y_dist = 10f, z_dist = 10f;
            bool x_intersect = false, y_intersect = false, z_intersect = false;

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            foreach (var p in planes)
            {
                float dist;
                float t = 0;

                if (p.Raycast(ray_x, out dist))
                {
                    t = Vector3.Distance(pivot, ray_x.GetPoint(dist));
                    if (t < x_dist || !x_intersect)
                    {
                        x_intersect = true;
                        x_dist = t;
                    }
                }

                if (p.Raycast(ray_y, out dist))
                {
                    t = Vector3.Distance(pivot, ray_y.GetPoint(dist));
                    if (t < y_dist || !y_intersect)
                    {
                        y_intersect = true;
                        y_dist = t;
                    }
                }

                if (p.Raycast(ray_z, out dist))
                {
                    t = Vector3.Distance(pivot, ray_z.GetPoint(dist));
                    if (t < z_dist || !z_intersect)
                    {
                        z_intersect = true;
                        z_dist = t;
                    }
                }
            }

            var x_iter = (int) (Mathf.Ceil(Mathf.Max(x_dist, y_dist)) / snapValue);
            var y_iter = (int) (Mathf.Ceil(Mathf.Max(x_dist, z_dist)) / snapValue);
            var z_iter = (int) (Mathf.Ceil(Mathf.Max(z_dist, y_dist)) / snapValue);

            var max = Mathf.Max(Mathf.Max(x_iter, y_iter), z_iter);
            var div = 1;
            while (max / div > MAX_LINES) div++;

            Vector3[] vertices_t = null;
            Vector3[] normals_t = null;
            Color[] colors_t = null;
            int[] indices_t = null;

            var vertices_m = new List<Vector3>();
            var normals_m = new List<Vector3>();
            var colors_m = new List<Color>();
            var indices_m = new List<int>();

            // X plane
            DrawHalfGrid(cam, pivot, up, right, snapValue * div, x_iter / div, colors[0], alphaBump, out vertices_t,
                out normals_t, out colors_t, out indices_t, 0);
            vertices_m.AddRange(vertices_t);
            normals_m.AddRange(normals_t);
            colors_m.AddRange(colors_t);
            indices_m.AddRange(indices_t);

            // Y plane
            DrawHalfGrid(cam, pivot, right, forward, snapValue * div, y_iter / div, colors[1], alphaBump,
                out vertices_t, out normals_t, out colors_t, out indices_t, vertices_m.Count);
            vertices_m.AddRange(vertices_t);
            normals_m.AddRange(normals_t);
            colors_m.AddRange(colors_t);
            indices_m.AddRange(indices_t);

            // Z plane
            DrawHalfGrid(cam, pivot, forward, up, snapValue * div, z_iter / div, colors[2], alphaBump, out vertices_t,
                out normals_t, out colors_t, out indices_t, vertices_m.Count);
            vertices_m.AddRange(vertices_t);
            normals_m.AddRange(normals_t);
            colors_m.AddRange(colors_t);
            indices_m.AddRange(indices_t);

            gridMesh.Clear();
            gridMesh.vertices = vertices_m.ToArray();
            gridMesh.normals = normals_m.ToArray();
            gridMesh.subMeshCount = 1;
            gridMesh.uv = new Vector2[vertices_m.Count];
            gridMesh.colors = colors_m.ToArray();
            gridMesh.SetIndices(indices_m.ToArray(), MeshTopology.Lines, 0);
        }

        private static void DrawHalfGrid(Camera cam, Vector3 pivot, Vector3 tan, Vector3 bitan, float increment,
            int iterations, Color secondary, float alphaBump,
            out Vector3[] vertices,
            out Vector3[] normals,
            out Color[] colors,
            out int[] indices, int offset)
        {
            var primary = secondary;
            primary.a += alphaBump;

            var len = increment * iterations;

            var highlightOffsetTan =
                (int) (pg_Util.ValueFromMask(pivot, tan) % (increment * majorLineIncrement) / increment);
            var highlightOffsetBitan =
                (int) (pg_Util.ValueFromMask(pivot, bitan) % (increment * majorLineIncrement) / increment);

            iterations++;

            // this could only use 3 verts per line
            var fade = .75f;
            var fadeDist = len * fade;
            var nrm = Vector3.Cross(tan, bitan);

            vertices = new Vector3[iterations * 6 - 3];
            normals = new Vector3[iterations * 6 - 3];
            indices = new int[iterations * 8 - 4];
            colors = new Color[iterations * 6 - 3];

            vertices[0] = pivot;
            vertices[1] = pivot + bitan * fadeDist;
            vertices[2] = pivot + bitan * len;

            normals[0] = nrm;
            normals[1] = nrm;
            normals[2] = nrm;

            indices[0] = 0 + offset;
            indices[1] = 1 + offset;
            indices[2] = 1 + offset;
            indices[3] = 2 + offset;

            colors[0] = primary;
            colors[1] = primary;
            colors[2] = primary;
            colors[2].a = 0f;


            var n = 4;
            var v = 3;

            for (var i = 1; i < iterations; i++)
            {
                // MeshTopology doesn't exist prior to Unity 4
                vertices[v + 0] = pivot + i * tan * increment;
                vertices[v + 1] = pivot + bitan * fadeDist + i * tan * increment;
                vertices[v + 2] = pivot + bitan * len + i * tan * increment;

                vertices[v + 3] = pivot + i * bitan * increment;
                vertices[v + 4] = pivot + tan * fadeDist + i * bitan * increment;
                vertices[v + 5] = pivot + tan * len + i * bitan * increment;

                normals[v + 0] = nrm;
                normals[v + 1] = nrm;
                normals[v + 2] = nrm;
                normals[v + 3] = nrm;
                normals[v + 4] = nrm;
                normals[v + 5] = nrm;

                indices[n + 0] = v + 0 + offset;
                indices[n + 1] = v + 1 + offset;
                indices[n + 2] = v + 1 + offset;
                indices[n + 3] = v + 2 + offset;
                indices[n + 4] = v + 3 + offset;
                indices[n + 5] = v + 4 + offset;
                indices[n + 6] = v + 4 + offset;
                indices[n + 7] = v + 5 + offset;

                var alpha = i / (float) iterations;
                alpha = alpha < fade ? 1f : 1f - (alpha - fade) / (1 - fade);

                var col = (i + highlightOffsetTan) % majorLineIncrement == 0 ? primary : secondary;
                col.a *= alpha;

                colors[v + 0] = col;
                colors[v + 1] = col;
                colors[v + 2] = col;
                colors[v + 2].a = 0f;

                col = (i + highlightOffsetBitan) % majorLineIncrement == 0 ? primary : secondary;
                col.a *= alpha;

                colors[v + 3] = col;
                colors[v + 4] = col;
                colors[v + 5] = col;
                colors[v + 5].a = 0f;

                n += 8;
                v += 6;
            }
        }

        /**
         * Draws a plane grid using pivot point, the right and forward directions, and how far each direction should extend
         */
        private static void DrawFullGrid(Camera cam, Vector3 pivot, Vector3 tan, Vector3 bitan, float increment,
            int iterations, int div, Color secondary, float alphaBump)
        {
            var primary = secondary;
            primary.a += alphaBump;

            var len = iterations * increment;

            iterations++;

            var start = pivot - tan * (len / 2f) - bitan * (len / 2f);
            start = pg_Util.SnapValue(start, bitan + tan, increment);

            var inc = increment;
            var highlightOffsetTan = (int) (pg_Util.ValueFromMask(start, tan) % (inc * majorLineIncrement) / inc);
            var highlightOffsetBitan = (int) (pg_Util.ValueFromMask(start, bitan) % (inc * majorLineIncrement) / inc);

            var lines = new Vector3[iterations * 4];
            var indices = new int[iterations * 4];
            var colors = new Color[iterations * 4];

            int v = 0, t = 0;

            for (var i = 0; i < iterations; i++)
            {
                var a = start + tan * i * increment;
                var b = start + bitan * i * increment;

                lines[v + 0] = a;
                lines[v + 1] = a + bitan * len;

                lines[v + 2] = b;
                lines[v + 3] = b + tan * len;

                indices[t++] = v;
                indices[t++] = v + 1;
                indices[t++] = v + 2;
                indices[t++] = v + 3;

                var col = (i + highlightOffsetTan) % majorLineIncrement == 0 ? primary : secondary;

                // tan
                colors[v + 0] = col;
                colors[v + 1] = col;

                col = (i + highlightOffsetBitan) % majorLineIncrement == 0 ? primary : secondary;

                // bitan
                colors[v + 2] = col;
                colors[v + 3] = col;

                v += 4;
            }

            var nrm = Vector3.Cross(tan, bitan);
            var nrms = new Vector3[lines.Length];
            for (var i = 0; i < lines.Length; i++)
                nrms[i] = nrm;


            gridMesh.Clear();
            gridMesh.vertices = lines;
            gridMesh.normals = nrms;
            gridMesh.subMeshCount = 1;
            gridMesh.uv = new Vector2[lines.Length];
            gridMesh.colors = colors;
            gridMesh.SetIndices(indices, MeshTopology.Lines, 0);
        }

        /**
         *	\brief Returns the distance from pivot to frustum plane in the order of
         *	float[] { tan, bitan, -tan, -bitan }
         */
        private static float[] GetDistanceToFrustumPlanes(Camera cam, Vector3 pivot, Vector3 tan, Vector3 bitan,
            float minDist)
        {
            var rays = new Ray[4]
            {
                new Ray(pivot, tan),
                new Ray(pivot, bitan),
                new Ray(pivot, -tan),
                new Ray(pivot, -bitan)
            };

            var intersects = new float[4] {minDist, minDist, minDist, minDist};
            var intersection_found = new bool[4] {false, false, false, false};

            var planes = GeometryUtility.CalculateFrustumPlanes(cam);
            foreach (var p in planes)
            {
                float dist;
                float t = 0;

                for (var i = 0; i < 4; i++)
                    if (p.Raycast(rays[i], out dist))
                    {
                        t = Vector3.Distance(pivot, rays[i].GetPoint(dist));

                        if (t < intersects[i] || !intersection_found[i])
                        {
                            intersection_found[i] = true;
                            intersects[i] = Mathf.Max(minDist, t);
                        }
                    }
            }

            return intersects;
        }
    }
}