using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/**
 * The type of an EffectType
 * ENERGY   - Gives the player additional energy
 * ATTACK   - Gives the player additional attack
 * DEFENSE  - Gives the player additional defense
 * HEALTH   - Heals the player
 * CARD     - The player draws cards
 */
public enum EffectType {
  ENERGY,   // 0
  ATTACK,
  HEALTH,
  CARD,
  GAIN,     // 4
  GIVE,
  TRASH,
  RETURN    // 7
}

public enum GeneralType {
  BASIC = 0,
  CARDMOD = 4,
  SPECIAL = 7
}


[StructLayout(LayoutKind.Explicit)]
public struct EffectData {
  // BasicType Data
  [FieldOffset(0)]
  public int num;

  // CardType Data
  [FieldOffset(0)]
  public int cardValue;

  public static implicit operator EffectData(int i) {
    return new EffectData() {num = i};
  }

  public static implicit operator EffectData(Card c) {
    if (c == null) return new EffectData() {cardValue = -1};
    return new EffectData() {cardValue = c.cardValue};
  }
}

/**
 * An effect a Card can have.
 * A Card can have multiple CardEffectGeneralType
 */
[Serializable]
public class CardEffect {

#region Public Variables

  public EffectData data;
  public EffectType type; // The type of the effect
  public GeneralType generalType {
    get {
      GeneralType output = GeneralType.BASIC;
      foreach (GeneralType t in Enum.GetValues(typeof(GeneralType))) {
        if ((int) t > (int) type) break;
        else output = t;
      }
      return output;
    }
  }

#endregion
#region Constructors

  public CardEffect(EffectData newData, EffectType newType) {
    data = newData;
    type = newType;
  }

#endregion
#region Public Methods
  
  /**
   * Apply the CardEffectType to PLAYER based on the type and dataue
   */
  public void OnPlay(Player player, Player opponent) {
    switch (type) {
      case EffectType.ENERGY:
        player.GainEnergy(data.num);
        break;
      case EffectType.ATTACK:
        player.Attack(data.num);
        break;
      case EffectType.HEALTH:
        if (data.num > 0) player.Heal(data.num);
        else if (data.num < 0) player.TakeDamage(-1 * data.num);
        break;
      case EffectType.CARD:
        player.Draw(data.num);
        break;

      case EffectType.GAIN:
        player.GetNew(data.cardValue);
        break;
      case EffectType.GIVE:
        player.GiveNew(data.cardValue);
        break;
      case EffectType.TRASH:
        if (data.cardValue < 0) player.TrashCard(-1);
        else {
        }
        break;

      default:
        break;
    }
  }

  public void OnDiscard(Player player, Player opponent) {
    switch (type) {
      case EffectType.RETURN:
        break;
      default:
        break;
    }
  }

#endregion
#region Override Methods

  public override string ToString() {  
    string output = "";
    if (generalType == GeneralType.BASIC) {
      if (data.num > 0) output += "+";
      output += data.num.ToString();
      output += " ";
      output += type.ToString();
      return output;
    }
    else if (generalType == GeneralType.CARDMOD) {
      if (type == EffectType.GIVE) output += "Opponent GAINS";
      else output += type.ToString();
      if (data.cardValue >= 0) output += " " + CardSet.GetCard(data.cardValue).name;
      else output += " this";
    }
    else if (generalType == GeneralType.SPECIAL) {
      if (type == EffectType.RETURN) output += "RETURN to hand on discard";
    }
    return output;
  }

#endregion
}
