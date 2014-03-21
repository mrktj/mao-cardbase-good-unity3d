using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pile : Group {
  public List<Card> pile { get { return (List<Card>) group; } }

  public DisplayCard top;

	void Start () {
    _group = new List<Card>();
	}

	void Update () {
  }

  [RPC]
	protected override void NetworkUpdateSprite () {
    if (pile.Count <= 0) {
      top.DrawOutline();
    }
    else { 
      int cardValue = pile[pile.Count - 1].cardValue;
      top.DrawCard(cardValue);
    }
	}

  public bool DealCard(Group g) { 
    if (pile.Count <= 0) return false;
    Group.MoveCard(pile[pile.Count - 1], this, g);
    UpdateSprite();
    g.UpdateSprite();
    return true;
  }
}
