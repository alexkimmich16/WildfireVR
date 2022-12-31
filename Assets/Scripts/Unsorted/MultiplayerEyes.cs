using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerEyes : MonoBehaviour
{
    public List<GameObject> AllEyes;
    public void ChangeEyes(Eyes eyes)
    {
        for (var i = 0; i < AllEyes.Count; i++)
            AllEyes[i].GetComponent<SkinnedMeshRenderer>().material = EyeController.instance.EyeMats[(int)eyes];
    }
}
