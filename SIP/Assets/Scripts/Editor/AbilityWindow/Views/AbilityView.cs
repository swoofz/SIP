using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

public class AbilityView {
    public enum ElementTpye { Fire, Water, Air, Earth }

    public string name;
    public ElementTpye type = ElementTpye.Fire;
    public VisualElement[] views = new VisualElement[3];
}
