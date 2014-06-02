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
  TRASH,
  DISCARD,
  RETURN    // 7
}

public enum GeneralType {
  BASIC = 0,
  CARDMOD = 4,
  SPECIAL = 7
}


[StructLayout(LayoutKind.Explicit)]
public struct EffectData {
  [FieldOffset(0)]
  public bool opponent;
  // BasicType Data
  [FieldOffset(1)]
  public int num;

  // CardType Data
  [FieldOffset(1)]
  public int cardValue;

  public EffectData(int i, bool b = false) {
    this.opponent = b;
    this.num = i;
    this.cardValue = this.num;
  }

  public EffectData(Card c, bool b = false) {
    this.opponent = b;
    this.cardValue = c.cardValue;
    this.num = this.cardValue;
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
    Player target = player;
    if (data.opponent == true) target = opponent;
    switch (type) {
      case EffectType.ENERGY:
        target.GainEnergy(data.num);
        break;
      case EffectType.ATTACK:
        target.Attack(data.num);
        break;
      case EffectType.HEALTH:
        if (data.num > 0) target.Heal(data.num);
        else if (data.num < 0) target.TakeDamage(-1 * data.num);
        break;
      case EffectType.CARD:
        target.Draw(data.num);
        break;

      case EffectType.GAIN:
        target.GetNew(data.cardValue);
        break;
      case EffectType.TRASH:
        if (data.cardValue == -1) target.TrashCard(-1);
        else {
        }
        break;
      case EffectType.DISCARD:
        if (data.cardValue == -2) target.Discard(target._hand.count);
        break;

      default:
        break;
    }
  }

  public void OnDiscard(Player player, Player opponent) {
    switch (type) {
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
      if (data.opponent == true) output += " for Opponent";
      return output;
    }
    else if (generalType == GeneralType.CARDMOD) {
      if (data.opponent == true) output += "Opponent ";
      output += type.ToString();
      if (data.cardValue >= 0) output += " " + CardSet.GetCard(data.cardValue).name;
      else if (data.cardValue == -2) output += " hand";
      else if (data.cardValue == -1) output += " this";
    }
    else if (generalType == GeneralType.SPECIAL) {
      if (type == EffectType.RETURN) output += "RETURN to hand every turn";
    }
    return output;
  }

#endregion
}
