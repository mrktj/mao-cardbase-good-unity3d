using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Deck : Group {
  public DisplayCard top;
  
  private static System.Random random = new System.Random();
  private const int numGroup = 52;

  void Start() {
    _group = new HashSet<Card>();
    for (int i = 0; i < numGroup; i++) {
      _group.Add(new Card(i));
    }
    top.DrawBack();
  }

	void Update () {
  }

  [RPC]
  protected override void NetworkUpdateSprite () {
    if (group.Count <= 0) {
      top.DrawEmpty();
    }
    else { 
      top.DrawBack();
    }
	}

  public bool DealCard(Group g) {
    if (group.Count <= 0) return false;
    Card randomCard = group.ElementAt(random.Next(group.Count)); 
    Group.MoveCard(randomCard, this, g);
    //Debug.Log("dealt " + randomCard + " to " + g);
    UpdateSprite();
    g.UpdateSprite();
    return true;
  }
}
