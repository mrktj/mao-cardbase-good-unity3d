using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * The type of an Effect
 * ENERGY   - Gives the player additional energy
 * ATTACK   - Gives the player additional attack
 * DEFENSE  - Gives the player additional defense
 * HEALTH   - Heals the player
 * CARD     - The player draws cards
 */
public enum EffectType {
  ENERGY,
  ATTACK,
  DEFENSE,
  HEALTH,
  CARD
};

/**
 * An effect a Card can have.
 * A Card can have multiple CardEffects
 */
[Serializable]
public class CardEffect {
#region Public Variables

  public int val;         // The value associated with the effect
  public EffectType type; // The type of the effect

#endregion
#region Constructors

  public CardEffect(int newVal, EffectType newType) {
    val = newVal;
    type = newType;
  }

#endregion
#region Public Methods
  
  /**
   * Apply the CardEffect to PLAYER based on the type and value
   */
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

#endregion
#region Override Methods

  public override string ToString() {  
    return "+" + val.ToString() + " " + type.ToString();
  }

#endregion
}
