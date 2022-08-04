using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
//https://www.phyley.com/decompose-force-into-xy-components
public enum learningState
{
    Learning = 0,
    Testing = 1,
}
public enum DebugType
{
    None = 0,
    Basic = 1,
    WithState = 2,
}

public class LearningAgent : Agent
{
    public static LearningAgent instance;
    private void Awake() { instance = this; }
    [Header("Set")]
    private static float FramePerSecond = 60;

    //[HideInInspector]
    public float Interval;

    public int DesiredCycles;
    public DebugType DebugType;

    [Header("Current")]
    public float Timer;

    [Header("Other")]

    [HideInInspector]
    public List<Motion> Motions;

    public delegate void EventHandler(bool State, int Cycle, int Set);
    public event EventHandler MoveToNextEvent;

    public delegate void EventHandlerTwo();
    public event EventHandlerTwo FinalFrame;

    public delegate void EventHandlerThree(bool State);
    public event EventHandlerThree NewState;

    public bool Right;

    public SingleInfo info;

    private float HandCamAngle, CamAngle, EndAngle;
    public static List<Vector2> ListNegitives = new List<Vector2>() { new Vector2(1,1), new Vector2(-1, 1), new Vector2(-1, -1), new Vector2(1, -1) };
    public static List<bool> InvertAngles  = new List<bool>() { false, true, false, true };
    private int Offset = 270;

    public bool Guess;

    //private float SecondTimer;
    //private int SecondCount;

    public float FixedUpdateTimer;
    private void FixedUpdate()
    {
        Interval = 1 / FramePerSecond;
        Timer += Time.deltaTime;
        if (Timer > Interval)
        {
            //SecondCount += 1;
            RequestDecision();
            CustomDebug("RequestDecision");
            Timer = 0;
        }
    }

    private void Update()
    {
        
        /*
        SecondTimer += Time.deltaTime;
        if (SecondTimer > 1)
        {
            SecondTimer = 0;
            Debug.Log(SecondCount);
            SecondCount = 0;
        }
        */
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        CustomDebug("CollectObservations");
        //Debug.Log("Observe  Frame: " + Frame + " Set: " + Set);
        LearnManager LM = LearnManager.instance;
        SingleInfo info = CurrentControllerInfo();

        
        if (LM.HeadPos)
            sensor.AddObservation(info.HeadPos);
        if (LM.HeadRot)
            sensor.AddObservation(info.HeadRot.normalized);
        if (LM.HandPos)
            sensor.AddObservation(info.HandPos);
        if (LM.HandRot)
            sensor.AddObservation(info.HandRot.normalized);
        if (LM.HandVel)
            sensor.AddObservation(info.HandVel);
        if (LM.AdjustedHandPos)
            sensor.AddObservation(info.AdjustedHandPos);

        RequestAction();
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        CustomDebug("OnActionReceived");
        
        bool CurrentGuess = actions.DiscreteActions[0] == 1;
        Guess = CurrentGuess;
        //GuessNum = actions.DiscreteActions[0];
        CustomDebug(CurrentGuess.ToString());
        NewState(CurrentGuess);
    }
    public void CustomDebug(string text)
    {
        if (DebugType == DebugType.None)
            return;
        string FrameReference = "";
        //if (DebugType == DebugType.WithState)
            //FrameReference = " Timer: " + Timer + "|Frame: " + Frame + "|Set: " + Set + "" + "|CycleNum: " + CycleNum + "|";
        Debug.Log(text + FrameReference);
    }
    public SingleInfo CurrentControllerInfo()
    {
        SingleInfo newInfo = new SingleInfo();
        LearnManager LM = LearnManager.instance;
        HandActions controller = MyHand();

        newInfo.HeadPos = LM.Cam.localPosition;
        newInfo.HeadRot = LM.Cam.rotation.eulerAngles;
        if (Right)
        {
            newInfo.HandPos = controller.transform.localPosition;
            newInfo.HandRot = controller.transform.localRotation.eulerAngles;
            newInfo.HandVel = controller.Velocity;

            newInfo.AdjustedHandPos = LM.Cam.position - controller.transform.position;

            newInfo.Works = controller.TriggerPressed();

            info = newInfo;

            return newInfo;
        }
        else
        {
            //CamRot
            float CamRot = LM.Cam.rotation.eulerAngles.y;
            CamAngle = CamRot;
            Vector3 handPos = controller.transform.localPosition;
            float Distance = Vector3.Distance(new Vector3(handPos.x, 0, handPos.z), new Vector3(newInfo.HeadPos.x, 0, newInfo.HeadPos.z));
            //pos
            Vector3 LevelCamPos = new Vector3(newInfo.HeadPos.x, 0, newInfo.HeadPos.z);
            Vector3 LevelHandPos = new Vector3(handPos.x, 0, handPos.z);
            Vector3 targetDir = LevelCamPos - LevelHandPos;

            Vector3 StartEulerAngles = LM.Cam.eulerAngles;
            LM.Cam.eulerAngles = new Vector3(0, CamRot, 0);

            Vector3 forwardDir = LM.Cam.rotation * Vector3.forward;
            LM.Cam.eulerAngles = StartEulerAngles;

            HandCamAngle = Vector3.SignedAngle(targetDir, forwardDir, Vector3.up) + 180;

            float Angle = GetAngle(CamRot);
            EndAngle = Angle;

            newInfo.AdjustedHandPos = IntoComponents(Angle);
            Vector2 XYForce = IntoComponents(Angle);
            Vector3 AdjustedCamPos = new Vector3(XYForce.x, 0, XYForce.y);

            Vector3 Point = (AdjustedCamPos * Distance) + new Vector3(newInfo.HeadPos.x, 0, newInfo.HeadPos.z);
            Point = new Vector3(Point.x, handPos.y, Point.z);
            newInfo.HandPos = Point;

            ///additional rotation
            Vector3 Rotation = controller.transform.rotation.eulerAngles;
            newInfo.HandRot = new Vector3(Rotation.x, Rotation.y, -Rotation.z);

            ///velocity
            Vector2 InputVelocity = new Vector2(controller.Velocity.x, controller.Velocity.z);
            Vector2 FoundVelocity = mirrorImage(IntoComponents(CamAngle), InputVelocity);
            newInfo.HandVel = new Vector3(FoundVelocity.x, controller.Velocity.y, FoundVelocity.y);

            newInfo.Works = controller.TriggerPressed();

            info = newInfo;
            return newInfo;
            Vector2 IntoComponents(float Angle)
            {
                int Quad = GetQuad(Angle, out float RemainingAngle);
                if (InvertAngles[Quad])
                    RemainingAngle = 90 - RemainingAngle;

                //Negitives
                float Radians = RemainingAngle * Mathf.Deg2Rad;
                //Debug.Log("remainAngle: " + Radians);

                float XForce = Mathf.Cos(Radians) * ListNegitives[Quad].x;
                float YForce = Mathf.Sin(Radians) * ListNegitives[Quad].y;

                return new Vector2(XForce, YForce);
               
                int GetQuad(float Angle, out float RemainAngle)
                {
                    //0-360
                    int TotalQuads = (int)(Angle / 90);
                    RemainAngle = Angle - TotalQuads * 90;
                    //int Quad = StartQuad;
                    if (TotalQuads < 0)
                    {
                        TotalQuads += 4;
                    }
                        
                    int RemoveExcessQuads = (int)(TotalQuads / 4);
                    //RemainAngle = Angle;
                    return TotalQuads - RemoveExcessQuads * 4;
                }
            }
            static Vector2 mirrorImage(Vector2 Line, Vector2 Pos)
            {
                float temp = (-2 * (Line.x * Pos.x + Line.y * Pos.y)) /(Line.x * Line.x + Line.y * Line.y);
                float x = (temp * Line.x) + Pos.x;
                float y = (temp * Line.y) + Pos.y;
                return new Vector2(x, y);
            }
            float GetAngle(float CamRot)
            {
                float Angle = 360 - (CamRot + HandCamAngle + Offset);
                //Offset
                if (Angle > 360)
                    Angle -= 360;
                else if (Angle < -360)
                    Angle += 360;
                return Angle;
            }
        }
    }

    HandActions MyHand()
    {
        if (Right)
            return LearnManager.instance.Right;
        else
            return LearnManager.instance.Left;
    }
}
[System.Serializable]
public class SingleInfo
{
    public Vector3 HeadPos, HeadRot, HandPos, HandRot, HandVel, AdjustedHandPos;
    public bool Works;
}
[System.Serializable]
public class Motion
{
    public List<SingleInfo> Infos;
    public int TrueIndex;
}

///should contain: velocity, hand rot, hand pos, head rot, head pos, 
///
///possible ways of input/recording:
///1: as lists containing info, generated from player motions
///2: randomly generated motions
///3: doing it in engine
///
///possible ways of learning given info:
///1: operator gives start and end time if at all(would require display and repeat motion)
///2: 
///

///should be able to tell if motion is true between 2 given frames

///OR give it lists, with active times determined ahead of time when given