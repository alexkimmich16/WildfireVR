using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionPlayback : MonoBehaviour
{
    public Motion motion;
    public float PlaybackSpeed = 1f;
    public float FrameWait;
    public Transform Head;
    public Transform Hand;
    public int Frame;
    public float Timer;

    // Update is called once per frame
    void Update()
    {
        FrameWait = 1 / PlaybackSpeed;

        Timer += Time.deltaTime;
        if (Timer < FrameWait)
            return;
        SingleInfo info = motion.Infos[Frame];
        Head.position = info.HeadPos;
        Head.eulerAngles = info.HeadRot;
        Hand.position = info.HandPos;
        Hand.eulerAngles = info.HandRot;
        Timer = 0;
        Frame += 1;
        if (Frame == motion.Infos.Count)
            Frame = 0;
    }
}
