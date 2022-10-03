using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBook : MonoBehaviour {
    public struct Data {
        public float[] values;
    }
    public struct AbilityData {
        public Ability ability;
        public float[] weights;

        public Dictionary<string, List<Data>> selfStats;
        public Dictionary<string, List<Data>> friendlyStats;
        public Dictionary<string, List<Data>> enemyStats;
    }

    public List<Ability> abilities;

    public Dictionary<string, List<Data>> GetStats(List<Attribute> targetType) {
        Dictionary<string, List<Data>> stats = new();
        foreach (var attr in targetType) {
            switch (attr.GetType().Name) {
                case "Damage":
                    var damage = attr as Damage;
                    if (!stats.ContainsKey("Damage")) {
                        stats.Add("Damage", new());
                        stats["Damage"].Add(new() {
                            values = new[] { (int)AttributeType.Damage, damage.min, damage.max }
                        });
                    } else {
                        stats["Damage"][0].values[1] += damage.min;
                        stats["Damage"][0].values[2] += damage.max;
                    }
                    break;

                case "Heal":
                    var heal = attr as Heal;
                    if (!stats.ContainsKey("Heal")) {
                        stats.Add("Heal", new());
                        stats["Heal"].Add(new() {
                            values = new[] { (int)AttributeType.Heal, heal.min, heal.max }
                        });
                    } else {
                        stats["Heal"][0].values[1] += heal.min;
                        stats["Heal"][0].values[2] += heal.max;
                    }
                    break;

                case "Buff":
                    var buff = attr as Buff;
                    if (!stats.ContainsKey("Buff")) {
                        stats.Add("Buff", new());
                    }
                    stats["Buff"].Add(new() {
                        values = new[] { (int)AttributeType.Buff, (int)buff.effect, buff.effectDuration, buff.increasEffectPercentage }
                    });
                    break;

                case "Debuff":
                    var debuff = attr as Debuff;
                    if (!stats.ContainsKey("DeBuff")) {
                        stats.Add("DeBuff", new());
                    }
                    stats["DeBuff"].Add(new() {
                        values = new[] { (int)AttributeType.Debuff, (int)debuff.effect, debuff.effectDuration, debuff.decreasEffectPercentage }
                    });
                    break;
            }
        }

        return stats;
    }

}
