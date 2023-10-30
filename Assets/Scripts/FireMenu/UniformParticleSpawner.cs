using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;
using System.Runtime.InteropServices;
using Menu;
using Sirenix.OdinInspector;
public class UniformParticleSpawner : SerializedMonoBehaviour
{
    public Mesh mesh;
    public ParticleSystem particleSystem;
    public int numberOfParticles;
    public bool Spawning;
    public void EmitParticles()
    {
        float[] triangleAreas = new float[mesh.triangles.Length / 3];
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Calculate the areas of the triangles
        float totalArea = 0;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 A = vertices[triangles[i]];
            Vector3 B = vertices[triangles[i + 1]];
            Vector3 C = vertices[triangles[i + 2]];

            float area = 0.5f * Vector3.Cross(B - A, C - A).magnitude;
            triangleAreas[i / 3] = area;
            totalArea += area;
        }

        // Emit particles
        for (int i = 0; i < numberOfParticles; i++)
        {
            // Choose a triangle, with probability proportional to its area
            float randomValue = Random.Range(0, totalArea);
            int triangleIndex = 0;
            for (int j = 0; j < triangleAreas.Length; j++)
            {
                randomValue -= triangleAreas[j];
                if (randomValue <= 0)
                {
                    triangleIndex = j * 3;
                    break;
                }
            }

            Vector3 A = vertices[triangles[triangleIndex]];
            Vector3 B = vertices[triangles[triangleIndex + 1]];
            Vector3 C = vertices[triangles[triangleIndex + 2]];
            Vector3 randomPoint = SamplePointOnTriangle(A, B, C);

            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
            {
                position = transform.TransformPoint(randomPoint)
            };

            particleSystem.Emit(emitParams, 1);
        }
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
    

    [Button]
    public void Start()
    {
        mesh = MenuEffect.instance.TextToMesh("FUCK");
        EmitParticles();
    }
}
