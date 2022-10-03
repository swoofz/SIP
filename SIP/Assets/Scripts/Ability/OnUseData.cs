using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OnUseData  {
    public List<AbilityBook.AbilityData> abilities;
    public int abilityIndex;
    public ElementTpye target_ele;
    public int target_lvl;
    public UISimController log;
}
