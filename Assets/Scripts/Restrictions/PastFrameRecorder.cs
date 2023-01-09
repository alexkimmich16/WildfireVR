using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace RestrictionSystem
{
    public class PastFrameRecorder : SerializedMonoBehaviour
    {
        public static PastFrameRecorder instance;
        private void Awake() { instance = this; }
        [FoldoutGroup("NeatDisplay"), ReadOnly] public List<SingleInfo> RightInfo;
        [FoldoutGroup("NeatDisplay"), ReadOnly] public List<SingleInfo> LeftInfo;

        

        [FoldoutGroup("Test")] public List<Transform> TestMain;
        [FoldoutGroup("Test")] public List<Transform> TestCam;
        [FoldoutGroup("Test")] public List<Transform> TestHand;

        [FoldoutGroup("Real")] public List<Transform> PlayerHands;
        [FoldoutGroup("Real")] public Transform Cam;

        public int MaxStoreInfo = 10;
        public int FramesAgo = 10;
        public SingleInfo GetControllerInfo(Side side)
        {
            ResetStats();
            TestCam[(int)side].position = Vector3.zero;
            TestHand[(int)side].position = TestHand[(int)side].position - Cam.localPosition;

            float YDifference = -Cam.localRotation.eulerAngles.y;

            //invert main to y distance
            if (side == Side.left)
            {
                TestMain[(int)side].localScale = new Vector3(-1, 1, 1);
                Vector3 Rot = TestCam[(int)side].eulerAngles;
                TestCam[(int)side].eulerAngles = new Vector3(Rot.x, -Rot.y, -Rot.z);
            }

            TestMain[(int)side].rotation = Quaternion.Euler(0, YDifference, 0);
            //TestCam[(int)side].localRotation = Cam.localRotation;
            return new SingleInfo(TestHand[(int)side].position, TestHand[(int)side].rotation.eulerAngles, TestCam[(int)side].position, TestCam[(int)side].rotation.eulerAngles);

            void ResetStats()
            {
                TestMain[(int)side].position = Vector3.zero;
                TestMain[(int)side].rotation = Quaternion.identity;
                TestMain[(int)side].localScale = new Vector3(1, 1, 1);
                SetEqual(Cam, TestCam[(int)side]);
                SetEqual(PlayerHands[(int)side].transform, TestHand[(int)side]);
                void SetEqual(Transform Info, Transform Set)
                {
                    Set.localPosition = Info.localPosition;
                    Set.localRotation = Info.localRotation;
                }
            }
        }

        void Start()
        {
            StartCoroutine(ManageLists(1f / 60f));
            //InvokeRepeating("ManageLists", 0f, 1f/60f);
        }
        public SingleInfo PastFrame(Side side)
        {
            List<SingleInfo> SideList = (side == Side.right) ? RightInfo : LeftInfo;
            return SideList[SideList.Count - FramesAgo];
        }
        public SingleInfo PastFrame(Side side, int FramesAgo)
        {
            List<SingleInfo> SideList = (side == Side.right) ? RightInfo : LeftInfo;
            return SideList[SideList.Count - FramesAgo];
        }
        IEnumerator ManageLists(float Interval)
        {
            while (true)
            {
                RightInfo.Add(GetControllerInfo(Side.right));
                if (RightInfo.Count > MaxStoreInfo)
                    RightInfo.RemoveAt(0);
                LeftInfo.Add(GetControllerInfo(Side.left));
                if (LeftInfo.Count > MaxStoreInfo)
                    LeftInfo.RemoveAt(0);

                if (LeftInfo.Count > MaxStoreInfo - 1)
                    RestrictionManager.instance.TriggerFrameEvents();

                yield return new WaitForSeconds(Interval);
            }
        }
        public bool AtMax() { return LeftInfo.Count > MaxStoreInfo - 1; }
    }
    [System.Serializable]
    public class SingleInfo
    {
        public Vector3 HeadPos, HeadRot, HandPos, HandRot;
        public float CurrentTime;
        public SingleInfo(Vector3 HandPosStat, Vector3 HandRotStat, Vector3 HeadPosStat, Vector3 HeadRotStat)
        {
            HeadPos = HeadPosStat;
            HeadRot = HeadRotStat;
            HandPos = HandPosStat;
            HandRot = HandRotStat;
            CurrentTime = Time.timeSinceLevelLoad;
        }
    }
}

