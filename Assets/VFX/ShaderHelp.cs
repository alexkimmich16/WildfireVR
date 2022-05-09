using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderHelp : MonoBehaviour
{
    public void GetStepValue(float FireSize, float Size1, float Size2, float Size3, out float OutFinal)
    {
        float Step1 = FireSize + Size1;
        float Step2 = Step1 + Size2;
        float Step3 = Step2 + Size3;
        OutFinal = Step3;
    }
}
