using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Scriptable Objects/Dialog")]
public class Dialogue : ScriptableObject
{
    public string characterName; // Tên nhân vật
    public List<string> dialogs;
    public AudioClip[] voiceClips;   
}
