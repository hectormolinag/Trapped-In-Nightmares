using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    public string name;
    
    public Sentences[] sentences;
}

[System.Serializable]
public class Sentences
{
    [TextArea(3, 10)]
    public string sentence;
    public Sprite sprite;
}
