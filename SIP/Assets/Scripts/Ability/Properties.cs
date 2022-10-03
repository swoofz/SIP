using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ElementTpye { Fire, Water, Air, Earth }

public class Properties : Node {
    public enum CastType { Free, Locked }

    public ElementTpye element = ElementTpye.Fire;
    public CastType castType = CastType.Locked;
    public GameObject abilityPrefab;
    [Range(0f, 10f)]
    public float castTime;
    [Range(5, 25)]
    public int timesToUpdate = 10;
    [TextArea]
    public string description;
}