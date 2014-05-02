using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum EffectType {
  ENERGY,
  ATTACK,
  DEFENSE,
  HEALTH,
  CARD
};

[Serializable]
public class CardEffect {
  public int val;
  public EffectType type;

  public CardEffect(int newVal, EffectType newType) {
    val = newVal;
    type = newType;
  }

  public void Apply(Player player) {
    switch (type) {
      case EffectType.ENERGY:
        player.GainEnergy(val);
        break;
      case EffectType.ATTACK:
        player.Attack(val);
        break;
      case EffectType.DEFENSE:
        player.Defend(val);
        break;
      case EffectType.HEALTH:
        player.Heal(val);
        break;
      case EffectType.CARD:
        player.Draw(val);
        break;
    }
  }

  public override string ToString() {  
    return "+" + val.ToString() + " " + type.ToString();
  }
}
