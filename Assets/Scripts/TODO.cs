using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Container/TODO")]
public class TODO : ScriptableObject
{
    public List<SubList> ToDoActive;

    [System.Serializable]
    public class SubList
    {
        public string Sub;
        public List<string> ToDo;
        public List<string> Done;
    }
    
    
}
