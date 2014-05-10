using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/**
 * A Deck is a Group that can only be randomly drawn from
 */
[Serializable]
public class Deck : Group {
  public DisplaySlot top; // A DisplaySlot at the top of the Deck
  
  private static System.Random random = new System.Random(); 

  void OnEnable() {
    _group = new List<int>();
    Init();
    NetworkUpdateSprite();
  }

	void Update () {
  }

  [RPC]
  protected override void NetworkUpdateSprite () {
    if (group.Count <= 0) {
      top.DrawBlank();
    }
    else { 
      top.DrawBack();
    }
	}

  public bool DealCard(Group g) {
    if (group.Count <= 0) return false;
    int randomCard = group.ElementAt(random.Next(group.Count)); 
    Group.MoveCard(randomCard, this, g);
    //Debug.Log("dealt " + randomCard + " to " + g);
    UpdateSprite();
    g.UpdateSprite();
    return true;
  }
}
