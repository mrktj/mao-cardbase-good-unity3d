using UnityEngine;
using System;
using System.Collections.Generic;


/**
 * A class to represent a Card.
 * Each card has a name, a purchase cost, a play cost, and effects when played.
 */
[Serializable]
public class Card {
#region Private Variables

  [SerializeField]
  private string _name;   // The name of the Card
  [SerializeField]
  private int _buyCost;   // The cost to purchase the Card
  [SerializeField]
  private int _useCost;   // The cost to play the Card
  [SerializeField]
  private List<CardEffect> _effects;  // The effects of the Card when played
  [SerializeField]
  private int _cardValue; // The value of the Card in the current CardSet

#endregion
#region Accessors

  public string name    { get { return _name; }}
  public int buyCost    { get { return _buyCost; }}
  public int useCost    { get { return _useCost; }}
  public List<CardEffect> effects { get { return _effects;}}
  public int cardValue  { get { return _cardValue; }}

  public Action<Player, Player, GameObject> OnPlay;
  public Action<Player, Player, GameObject> OnDiscard;
  public Action<Player, Player, GameObject> OnBuy;

  /* The effects of the Card in text. */
  public string fullText {
    get {
      string output = "";
      foreach (CardEffect e in effects) {
        output += e.ToString() + "\n";
      }
      return output.Trim();
    }
  }

#endregion
#region Constructors

  public Card () {
    _buyCost = 0;
    _useCost = 0;
    _cardValue = 0;
    _name = "";
    _effects = new List<CardEffect>();
  }

  public Card(string name, int buy, int use, int val, CardEffect[] eff) {
    _name = name;
    _buyCost = buy;
    _useCost = use;
    _cardValue = val;
    _effects = new List<CardEffect>(eff);
    OnDiscard += EmptyFunction;
    OnPlay += EmptyFunction;
    OnBuy += EmptyFunction;
    foreach (CardEffect ce in effects) {
      if (ce.trigger == EffectTrigger.PLAY) OnPlay += ce.Effect;
      else if (ce.trigger == EffectTrigger.DISCARD) OnDiscard += ce.Effect;
      else if (ce.trigger == EffectTrigger.BUY) OnBuy += ce.Effect;
    }
  }
  
  public void EmptyFunction(Player p1, Player p2, GameObject obj) {
    return;
  }

  public Card(Card card) {
    _buyCost = card.buyCost;
    _useCost = card.useCost;
    _name = card.name;
    _cardValue = card.cardValue;
    _effects = card.effects;
  }
  
  public Card(int val) : this(CardSet.GetCard(val)){
  }

#endregion
#region Public Methods

  public bool HasEffect(EffectType type) {
    foreach (CardEffect ce in effects) {
      if (ce.type == type) return true;
    }
    return false;
  }

#endregion
#region Override Methods

  public override string ToString() {
    return name + "(" + buyCost + "/" + useCost + ")";
  }

#endregion
}
