using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.SDF;
using TMPro;
using static Odin.Net;
using Sirenix.OdinInspector;
using System.Linq;
namespace Menu
{
    public enum Menu
    {
        Base = 0,
        Options = 1,
    }

    public enum DisplayType
    {
        VolumeDisplay = 0,
        VolumeButton = 1,
        VolumeTypeText = 2,
    }

    public class VFXText : SerializedMonoBehaviour
    {
        private VisualEffect VFX;
        //public string Text;

        private Mesh mesh;

        public bool RendererTest;

        private BoxCollider Collider;

        public Menu menu;
        [ShowIf("menu", Menu.Options)]public DisplayType displayType;
        [ShowIf("menu", Menu.Options)] public SoundType SoundType;

        private float[] cumulativeAreas;
        private Vector3[] vertices;
        private int[] triangles;
        private Vector3[] precomputedPoints;

        public int PointMultiplier = 1;

        MenuEffect ME => MenuEffect.instance;

        public void ResetMesh()
        {
            VFX.Stop();
            if (mesh != null) // Destroy active mesh
            {
                mesh.Clear();
                Mesh.Destroy(mesh);
            }

            ///Get Mesh
            if(menu == Menu.Base)
            {
                mesh = ME.TextToMesh(gameObject.name.ToUpper());
            }
            else if(menu == Menu.Options)
            {
                if(displayType == DisplayType.VolumeDisplay)
                {
                    mesh = ME.TextToMesh(Mathf.RoundToInt(SoundManager.instance.Volume(SoundType) * 100f).ToString());
                }
                else if (displayType == DisplayType.VolumeButton)
                {
                    Mesh MeshReference = GetComponent<MenuButtons>().Increase ? ME.PlusMesh : ME.SubtractMesh;
                    mesh = new Mesh
                    {
                        vertices = MeshReference.vertices,
                        triangles = MeshReference.triangles,
                        normals = MeshReference.normals,
                        uv = MeshReference.uv,
                        uv2 = MeshReference.uv2,
                        tangents = MeshReference.tangents,
                        colors = MeshReference.colors,
                        subMeshCount = MeshReference.subMeshCount
                    };
                }
                else if (displayType == DisplayType.VolumeTypeText)
                {
                    mesh = ME.TextToMesh(SoundType.ToString().ToUpper());
                }
            }
            if (RendererTest)
                GetComponent<MeshFilter>().mesh = mesh;
            cumulativeAreas = new float[mesh.triangles.Length / 3];
            vertices = mesh.vertices;
            triangles = mesh.triangles;

            // Calculate the cumulative areas of the triangles
            float totalArea = 0;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 A = vertices[triangles[i]];
                Vector3 B = vertices[triangles[i + 1]];
                Vector3 C = vertices[triangles[i + 2]];

                float area = 0.5f * Vector3.Cross(B - A, C - A).magnitude;
                totalArea += area;
                cumulativeAreas[i / 3] = totalArea;
            }


            int numberOfTriangles = cumulativeAreas.Length;
            int numberOfParticles = numberOfTriangles * PointMultiplier; // Total number of particles

            List<Vector3> NewprecomputedPoints = new List<Vector3>();

            for (int i = 0; i < numberOfParticles; i++)
            {
                float randomValue = Random.Range(0, cumulativeAreas[cumulativeAreas.Length - 1]);
                int triangleIndex = System.Array.BinarySearch(cumulativeAreas, randomValue);
                if (triangleIndex < 0)
                    triangleIndex = ~triangleIndex;

                triangleIndex *= 3;

                Vector3 A = vertices[triangles[triangleIndex]];
                Vector3 B = vertices[triangles[triangleIndex + 1]];
                Vector3 C = vertices[triangles[triangleIndex + 2]];
                Vector3 point = SamplePointOnTriangle(A, B, C);
                NewprecomputedPoints.Add(point);
            }

            precomputedPoints = NewprecomputedPoints.ToArray();

            Vector3 SamplePointOnTriangle(Vector3 A, Vector3 B, Vector3 C)
            {
                float r1 = Random.value;
                float r2 = Random.value;
                float sqrtR1 = Mathf.Sqrt(r1);

                float u = 1 - sqrtR1;
                float v = r2 * sqrtR1;

                return u * A + v * B + (1 - u - v) * C;
            }
        }
        public void Play()
        {
            ResetMesh();
            VFX.SetTexture("Points", GetTexture2D(precomputedPoints));
            VFX.SetInt("PointCount", precomputedPoints.Length);
            VFX.Play();
        }
        
        public void OnMenuChange(bool NewState)
        {
            if(Collider != null)
                Collider.enabled = NewState;

            if (NewState == true)
                SetVFXColor(false);

            if (NewState)
            {
                Play();
            }
            else
            {
                VFX.Stop();
            }
        }
        public Texture2D GetTexture2D(Vector3[] Positions)
        {
            Texture2D texture = new Texture2D(Positions.Length, 1, TextureFormat.RGBAFloat, false);
            for (int i = 0; i < Positions.Length; i++)
            {
                Vector3 vec = Positions[i];
                texture.SetPixel(i, 0, new Color(vec.x, vec.y, vec.z, 0f));
            }
            texture.Apply();
            return texture;
        }
        void Start()
        {
            if(GetComponent<BoxCollider>())
                Collider = GetComponent<BoxCollider>();

            if (GetComponent<VisualEffect>())
                VFX = GetComponent<VisualEffect>();

            if (RendererTest)
                gameObject.AddComponent<MeshFilter>();

            ResetMesh();


            if (menu == Menu.Base)
                MenuController.OnMenuState += OnMenuChange;
            else if(menu == Menu.Options)
                MenuController.OnOptionsState += OnMenuChange;
        }

        public void SetVFXColor(bool Touched)
        {
            VFX.SetBool("Touched", Touched);
        }
    }
}

