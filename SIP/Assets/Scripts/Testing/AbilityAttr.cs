
public class AbilityAttr {
    public class Buff : AbilityAttr {
        public float duration;
    }

    public class Damage : AbilityAttr {
        public float min;
        public float max;
    }

    public class Heal : AbilityAttr {
        public float min;
        public float max;
    }

    public class DOT : AbilityAttr {
        public float duration;
        public float delay;
        public float damage;
    }
}