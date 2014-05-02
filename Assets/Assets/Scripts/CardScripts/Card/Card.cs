using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Card {
  [SerializeField]
  private int _buyCost;
  [SerializeField]
  private int _useCost;
  [SerializeField]
  private int _cardValue;
  [SerializeField]
  private string _name;
  [SerializeField]
  private string _text;
  [SerializeField]
  private List<CardEffect> _effects;

  public int buyCost { get { return _buyCost; }}
  public int useCost { get { return _useCost; }}
  public int cardValue { get { return _cardValue; }}
  public string name { get { return _name; }}
  public string text { get { return _text; }}
  public List<CardEffect> effects { get { return _effects;}}

  public string fullText {
    get {
      string output = "";
      foreach (CardEffect e in effects) {
        output += e.ToString() + "\n";
      }
      return output;
    }
  }

  public Card(int val) : this(CardSet.GetCard(val)){
  }

  public Card(Card card) {
    _buyCost = card.buyCost;
    _useCost = card.useCost;
    _name = card.name;
    _text = card.text;
    _cardValue = card.cardValue;
    _effects = card.effects;
  }

  public Card () {
    _buyCost = 0;
    _useCost = 0;
    _cardValue = 0;
    _name = "";
    _text = "";
    _effects = new List<CardEffect>();
  }

  public Card(string name, string text, int buy, int use, int val, CardEffect[] eff) {
    _name = name;
    _text = text;
    _buyCost = buy;
    _useCost = use;
    _cardValue = val;
    _effects = new List<CardEffect>(eff);
  }

  public override string ToString() {
    return name + "(" + buyCost + "/" + useCost + ")";
  }

  public void ApplyCardEffects(Player player) {
    foreach (CardEffect ce in effects) {
      ce.Apply(player);
    }
  }
}
