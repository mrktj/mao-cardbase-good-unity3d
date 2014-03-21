using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : Group {
  private const int handSize = 4;

  private int _leftIndex;
  public int leftIndex { get { return _leftIndex;} }

  public DisplayCard[] cards;
  public List<Card> hand { get { return (List<Card>) group; } }
  public bool isFull { get { return hand.Count >= handSize; } }

  public NetworkPlayer player;

	void Start () {
	  _group = new List<Card>();
    _leftIndex = 0;
	}
	
	void Update () {
	}

  [RPC]
  protected override void NetworkUpdateSprite() {
    for (int i = 0; i < handSize; i++) {
        if (i >= hand.Count)
          cards[i].DrawEmpty();
        else if (Network.player == player) 
          cards[i].DrawCard(hand[i + leftIndex].cardValue);
        else 
          cards[i].DrawBack();
    }
  }

  public bool PlayCard(DisplayCard card, Group g) {
    if (group.Count <= 0) return false;
    int idx = -1;
    for (idx = 0; idx < hand.Count; idx++) {
      if (cards[idx] == card) {
        //Debug.Log("played " + hand[idx]  + " to " + g);
        Group.MoveCard(hand[idx], this, g);
        UpdateSprite();
        g.UpdateSprite();
        return true;
      }
    }
    return false;
  }

}
