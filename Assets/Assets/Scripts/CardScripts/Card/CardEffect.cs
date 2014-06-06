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
  PASSIVE,  // 7
  RETURN    // 8
}

public enum GeneralType {
  BASIC = 0,
  CARDMOD = 4,
  PASSIVE = 7,
  SPECIAL = 8
}

public enum EffectTrigger {
  PLAY,
  DISCARD,
  BUY
}

[StructLayout(LayoutKind.Explicit)]
public struct EffectData {
  [FieldOffset(0)]
  public EffectTrigger trigger;
}

/**
 * An effect a Card can have.
 * A Card can have multiple CardEffectGeneralType
 */
[Serializable]
public class CardEffect {

#region Public Variables

  public EffectType type;       // The type of the effect
  public EffectTrigger trigger; // When the effect triggers
  public bool opponentEffect;   // Whether the effect applies to the opponent
  public int num;
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
  public Action<Player, GameObject> EffectAction;

#endregion
#region Constructors
  
  public CardEffect(EffectType newType, int newNum) : 
    this(newType, EffectTrigger.PLAY, false, newNum) {
  }
  
  public CardEffect(EffectType newType, bool opponent, int newNum) : 
    this(newType, EffectTrigger.PLAY, opponent, newNum) {
  }

  public CardEffect(EffectType newType, EffectTrigger newtrigger, bool opponent, int newNum) {
    trigger = newtrigger;
    opponentEffect = opponent;
    num = newNum;
    type = newType;
    
    SetActions();
  }

#endregion
#region Public Methods
  
  private void SetActions() {
    switch (type) {
      case EffectType.ENERGY:
        EffectAction = (p1, obj) => p1.GainEnergy(num); break;
      case EffectType.ATTACK:
        EffectAction = (p1, obj) => p1.Attack(num); break;
      case EffectType.HEALTH:
        if (num > 0) EffectAction = (p1, obj) => p1.Heal(num);
        else if (num < 0) EffectAction = (p1, obj) => p1.TakeDamage(-1 * num);
        break;
      case EffectType.CARD:
        EffectAction = (p1, obj) => p1.Draw(num); break;
      case EffectType.GAIN:
        EffectAction = (p1, obj) => p1.GetNew(num); break;
      case EffectType.TRASH:
        EffectAction = (p1, obj) => p1.TrashCard(obj);
        break;
      case EffectType.DISCARD:
        if (num == -2) EffectAction = (p1, obj) => p1.Clear(p1._hand);
        break;
      case EffectType.RETURN:
        EffectAction = (p1, obj) => p1.ReturnCards(); break;
      default:
        break;
    }
  }

  /**
   * Apply the CardEffectType to PLAYER based on the type and dataue
   */
  public void Effect(Player player, Player opponent, GameObject display) {
    Player target = player;
    if (opponentEffect == true) target = opponent;
    if (EffectAction != null) EffectAction(target, display);
  }

#endregion
#region Override Methods

  public override string ToString() {  
    string output = "";
    switch (trigger) {
      case (EffectTrigger.PLAY):
        break;
      case (EffectTrigger.DISCARD):
        output += "Discard: ";
        break;
      case (EffectTrigger.BUY):
        output += "Buy: ";
        break;
      default:
        break;
    }
    if (opponentEffect == true) output += "Enemy: ";
    if (generalType == GeneralType.BASIC) {
      if (num > 0) output += "+";
      output += num.ToString();
      output += " ";
      output += type.ToString();
    }
    else if (generalType == GeneralType.CARDMOD) {
      output += type.ToString();
      if (num >= 0) output += " " + CardSet.GetCard(num).name;
      else if (num == -2) output += " hand";
      else if (num == -1) output += " this";
    }
    else if (generalType == GeneralType.SPECIAL) {
      if (type == EffectType.RETURN) output += "RETURN to hand";
    }
    return output;
  }

#endregion
}
