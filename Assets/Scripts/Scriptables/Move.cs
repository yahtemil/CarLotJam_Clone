using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Game Option/Move", order = 0)]
public class Move : ScriptableObject
{
    [Range(0.05f,0.5f)]
    public float moveSpeed;

    [Range(0f, 0.5f)]
    public float rotateSpeed;
}
