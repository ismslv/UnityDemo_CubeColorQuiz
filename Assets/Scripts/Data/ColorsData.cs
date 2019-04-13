using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorsSet", menuName = "ColorsSet", order = 1)]
public class ColorsData : ScriptableObject
{
    public ColorItem[] colors;

    public ColorItem RandomColor
    {
        get
        {
            return colors[Random.Range(0, colors.Length)];
        }
    }

    public Color Get(string name) {
        return colors.First(c => {return c.name == name;}).color;
    }
}

[System.Serializable]
public struct ColorItem
{
    public Color color;
    public string name;
}