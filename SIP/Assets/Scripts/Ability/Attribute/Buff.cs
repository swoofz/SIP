using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : Attribute {
    public Effects.Effect effect = Effects.Effect.Attack;
    public float effectDuration;
    [Range(0f, 100f)]
    public float increasEffectPercentage;
}
