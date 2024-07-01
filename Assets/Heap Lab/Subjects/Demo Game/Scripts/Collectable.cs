using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectableType
{
    SpeedUp,
    PowerUp,
    Healer
}

public class Collectable : MonoBehaviour
{
    public CollectableType CollectableType;
    public MeshRenderer MeshRenderer;
    public List<Material> Materials;

    public void SetType(CollectableType type)
    {
        Debug.Log("coolectable : " + type);
        CollectableType = type;
        MeshRenderer.material = Materials[(int)type];
    }
}
