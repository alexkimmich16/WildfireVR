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
    public class MenuEffect : SerializedMonoBehaviour
    {
        public static MenuEffect instance;
        void Awake() { instance = this; }

        public List<Mesh> Characters;

        [FoldoutGroup("MeshCreation")] public int maxResolution = 64;
        [FoldoutGroup("MeshCreation")] public Vector3 center;
        [FoldoutGroup("MeshCreation")] public Vector3 sizeBox;
        [FoldoutGroup("MeshCreation")] public int signPassCount = 1;
        [FoldoutGroup("MeshCreation")] public float threshold = 0.5f;

        public Vector3 scaleFactor = new Vector3(1.5f, 1.5f, 1.5f); // Scale for the mesh

        [FoldoutGroup("PositionPlacement")] public float MenuDistance = 5f; // Distance from the camera
        [FoldoutGroup("PositionPlacement")] public float rotationSpeed = 5f; // Speed of the rotation lerp
        [FoldoutGroup("PositionPlacement")] public float positionSpeed = 5f; // Speed of the rotation lerp

        [FoldoutGroup("MeshPlacement")] public float SideSpacing = 1.2f, HeightSpacing = 1.2f; // Spacing between letters


        [FoldoutGroup("Ref")] public Mesh PlusMesh;
        [FoldoutGroup("Ref")] public Mesh SubtractMesh;
        [FoldoutGroup("Ref")] public List<VFXText> VolumeDisplays;
        [FoldoutGroup("Ref")] public VFXText QualityDisplay;


        [FoldoutGroup("VFX")] public float ButtonColorTime;


        //[FoldoutGroup("Color")] Color 

        public Mesh TextToMesh(string text)
        {
            float MiddleOffset = ((text.Length - 1) * SideSpacing) / 2f;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            float offset = 0f;
            foreach (char c in text.ToUpper()) // Convert to upper case to handle both cases
            {
                if (c == ' ')
                {
                    offset += SideSpacing;
                    continue; // Skip spaces
                }

                int index;
                if (c >= 'A' && c <= 'Z') index = c - 'A'; // Assumes letters are indexed from 0-25
                else if (c >= '0' && c <= '9') index = 26 + c - '0'; // Assumes numbers are indexed from 26-35, following the letters
                else continue; // Skip any other characters

                if (index >= 0 && index < Characters.Count) // Use Numbers list instead of Letters
                {
                    // Get the mesh for this character (could be a letter or number)
                    Mesh letterMesh = Characters[index];

                    // Offset the vertices for this character
                    foreach (Vector3 vertex in letterMesh.vertices)
                        vertices.Add(Vector3.Scale(vertex, scaleFactor) + new Vector3(offset - MiddleOffset, 0f, 0f));

                    // Add the triangles for this character
                    foreach (int triangleVertex in letterMesh.triangles)
                        triangles.Add(triangleVertex + vertices.Count - letterMesh.vertexCount);

                    // Adjust the offset for the next character
                    offset += SideSpacing;
                }
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.vertices = vertices.ToArray();
            combinedMesh.triangles = triangles.ToArray();

            return combinedMesh;
        }
        
        private void Update()
        {
            //move rot and pos of options menu
            Vector3 ForwardRot = Quaternion.Euler(new Vector3(0, AIMagicControl.instance.Cam.eulerAngles.y, 0)) * Vector3.forward;
            transform.position = Vector3.Lerp(transform.position, AIMagicControl.instance.Cam.position + (ForwardRot * MenuDistance), Time.deltaTime * positionSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0, AIMagicControl.instance.Cam.eulerAngles.y, 0)), Time.deltaTime * rotationSpeed);
        }

        public void ButtonTouched(SoundType soundType, bool Increase)
        {
            SoundManager.instance.SetVolume(soundType, MenuController.VolumeChange * (Increase ? 1f : -1f));
            VolumeDisplays[(int)soundType].Play();
            //ResetText
        }

        public void QualityChange(bool Increase)
        {
            SettingsControl.ChangeSettings(Increase ? 1 : -1);

            QualityDisplay.Play();
            //ResetText
        }

    }
}

