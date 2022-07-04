using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageOverTime {
    public int id;

    public DamageType.Type type;
    public int tick;
    public int duration;
}
