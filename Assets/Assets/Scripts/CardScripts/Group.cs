using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[Serializable]
public abstract class Group : MonoBehaviour {
  [SerializeField]
  protected ICollection<Card> _group;
  public ICollection<Card> group { 
    get { 
      return _group;
    } 
  }

	void Start () {
	}
	
	void Update () {
	}

  private void Add(Card c) {
    networkView.RPC("NetworkAdd", RPCMode.All, c.cardValue);
  }

  private void Remove(Card c) {
    networkView.RPC("NetworkRemove", RPCMode.All, c.cardValue);
  }

  public void UpdateSprite() {
    networkView.RPC("NetworkUpdateSprite", RPCMode.All);
  }

  public void ShuffleInto(Group g) {
    Debug.Log("shuffled " + g + " into " + this);
    foreach (Card card in group) {
      g.Add(card);
    }
    networkView.RPC("NetworkClear", RPCMode.All);
    UpdateSprite();
    g.UpdateSprite();
  }

  public static void MoveCard(Card c, Group from, Group to) {
    from.Remove(c);
    to.Add(c);
  }
  
  [RPC]
  private void NetworkAdd(int cardval) {
    group.Add(new Card(cardval));
  }
  
  [RPC]
  private void NetworkRemove(int cardval) {
    Card c = null;
    foreach (Card card in group) {
      if (card.cardValue == cardval)
        c = card;
    }
    if (c != null) 
      group.Remove(c);
  }

  [RPC]
  private void NetworkClear() {
    group.Clear();
  }

  [RPC]
  protected virtual void NetworkUpdateSprite() {
  }
}
