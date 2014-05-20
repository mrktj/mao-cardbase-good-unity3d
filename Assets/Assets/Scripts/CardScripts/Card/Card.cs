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

  /* Apply the Effects of the Card to PLAYER */
  public void OnPlayEffects(Player player, Player opponent) {
    foreach (CardEffect ce in effects) {
      ce.OnPlay(player, opponent);
    }
  }

  public void OnDiscardEffects(Player player, Player opponent) {
    foreach (CardEffect ce in effects) {
      ce.OnDiscard(player, opponent);
    }
  }

#endregion
#region Override Methods

  public override string ToString() {
    return name + "(" + buyCost + "/" + useCost + ")";
  }

#endregion
}
