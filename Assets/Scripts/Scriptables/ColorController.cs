using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "Game Option/Color Options", order = 0)]
public class ColorController : ScriptableObject
{
    public List<ColorOption> colorOptions = new List<ColorOption>();
}

[System.Serializable]
public class ColorOption
{
    public ColorType colorType;
    public Color color;
    public Material materialOutline;
    public Material materialNormal;
}
