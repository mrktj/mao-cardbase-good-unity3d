using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Pile : Group {
  public List<int> pile { get { return (List<int>) group; } }
  public DisplayCard top;

	void OnEnable () {
    _group = new List<int>();
    Init();
    NetworkUpdateSprite();
	}

	void Update () {
  }

  [RPC]
	protected override void NetworkUpdateSprite () {
    if (pile.Count <= 0) {
      top.DrawOutline();
    }
    else { 
      int cardValue = pile[pile.Count - 1];
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
