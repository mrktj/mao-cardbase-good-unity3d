using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[Serializable]
public abstract class Group : MonoBehaviour {
  [SerializeField]
  protected ICollection<int> _group;
  public ICollection<int> group { 
    get { 
      return _group;
    } 
  }

  [HideInInspector]
  [SerializeField]
  public int numDefault;
  [HideInInspector]
  public int defaultCard;

	void Start () {
	}
	
	void Update () {
	}

  protected void Init() {
    for (int i = 0; i < numDefault; i++) {
      _group.Add(defaultCard);
    }
  }

  private void Add(int i) {
    networkView.RPC("NetworkAdd", RPCMode.All, i);
  }

  private void Remove(int i) {
    networkView.RPC("NetworkRemove", RPCMode.All, i);
  }

  public void UpdateSprite() {
    networkView.RPC("NetworkUpdateSprite", RPCMode.All);
  }

  public void ShuffleInto(Group g) {
    foreach (int i in group) {
      g.Add(i);
    }
    networkView.RPC("NetworkClear", RPCMode.All);
    UpdateSprite();
    g.UpdateSprite();
  }

  public static void MoveCard(int i, Group from, Group to) {
    from.Remove(i);
    to.Add(i);
  }

  [RPC]
  private void NetworkAdd(int cardval) {
    group.Add(cardval);
  }

  [RPC]
  private void NetworkRemove(int cardval) {
    group.Remove(cardval);
  }

  [RPC]
  private void NetworkClear() {
    group.Clear();
  }

  [RPC]
  protected virtual void NetworkUpdateSprite() {
  }
}
