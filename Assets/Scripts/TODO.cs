using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Container/TODO")]
public class TODO : ScriptableObject
{
    public List<string> ToDo;
    public List<string> ToDoActive;
}
