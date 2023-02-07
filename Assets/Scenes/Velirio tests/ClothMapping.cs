using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class ClothMapping : MonoBehaviour
{
    public IK ik;

    [System.Serializable]
    public class Map
    {
        public Transform clothBone;
        public Transform closestBone;

        private Vector3 relPos;
        private Quaternion relRot = Quaternion.identity;

        public void Init()
        {
            relPos = closestBone.InverseTransformPoint(clothBone.position);
            relRot = Quaternion.Inverse(closestBone.rotation) * clothBone.rotation;
        }

        public void Apply()
        {
            clothBone.position = closestBone.TransformPoint(relPos);
            clothBone.rotation = closestBone.rotation * relRot;
        }
    }

    public Map[] maps = new Map[0];

    private void Start()
    {
        foreach (Map map in maps) map.Init();
    }

    private void OnEnable()
    {
        ik.GetIKSolver().OnPostUpdate += OnPostUpdate;
    }

    void OnPostUpdate()
    {
        foreach (Map map in maps) map.Apply();
    }

    private void OnDisable()
    {
        if (ik != null) ik.GetIKSolver().OnPostUpdate -= OnPostUpdate;
    }
}
