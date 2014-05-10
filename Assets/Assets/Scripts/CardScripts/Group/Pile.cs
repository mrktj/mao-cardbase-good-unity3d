using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A Pile acts like a stack of Cards
 */
[System.Serializable]
public class Pile : Group {
  public List<int> pile { get { return (List<int>) group; } }
  public DisplaySlot top; // A DisplaySlot at the top of the Pile

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
      top.DrawBlank();
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
