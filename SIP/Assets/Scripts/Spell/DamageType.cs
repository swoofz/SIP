using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageType {
    public enum Type { 
        Fire, Water, Earth, Air 
    }

    static public bool isOppisiteType(Type objectType, Type spellType) {
        switch (objectType) {
            case Type.Fire:
                if(spellType == Type.Water) return true;
                break;
            case Type.Water:
                if(spellType == Type.Fire) return true;
                break;
            case Type.Earth:
                if(spellType == Type.Air) return true;
                break;
            case Type.Air:
                if(spellType == Type.Earth) return true;
                break;
            default: break;
        }
    
        return false;
    }
}
