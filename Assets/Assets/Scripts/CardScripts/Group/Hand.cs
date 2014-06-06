using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A Hand is a Group where all the group are only visible to one Player
 * If player is null, then the hand is open and all group are visible
 */
public class Hand : Group {
  private List<GameObject> _slots; // A List of DisplaySlots to display group
  public List<GameObject> slots { get { return _slots;} } 
  //public GameObject displayCardPrefab; // The DisplaySlot prefab
  public Player player; // the Player this hand belongs to
  public bool visible;
  public int count { get {return _slots.Count;}}
  
	void OnEnable () {
	  _group = new List<int>();
    _slots = new List<GameObject>();
    if (Network.isServer) {
      Init();
      UpdateSprite();
    }
	}

  void Start () {
  }

  public void Setup() {
    if (Network.isServer) {
      Init();
      InitSlots();
      UpdateSprite();
    }
  }

  public void InitSlots() {
    for (int i = 0; i < _group.Count; i++) {
      ReceiveSlot(NewDisplaySlot(group[i]));
    }
  }
	
  /**
   * Clear the Hand and place all group into Group G
   */
  public void ClearInto(Group g) {
    while (slots.Count > 0) {
      MoveDisplaySlot(0, this, g);
    }
    //ShuffleInto(g);
  }
  
  /**
   * Play the Card shown in DisplaySlot obj into Group G
   */
  public void PlayCard(GameObject go, Group g) {
    int idx = -1;
    for (int i = 0; i < slots.Count; i++ ) {
      if (slots[i] == go) {
        idx = i;
      }
    }
    if (idx == -1) {
      throw new System.ArgumentException("GameObject not in hand", "go");
    }
    MoveDisplaySlot(idx, this, g);
  }


  protected override GameObject SendSlot(int idx) {
    GameObject obj = slots[idx];
    networkView.RPC("NetworkSendSlot", RPCMode.All, idx);
    return obj;
  }

  [RPC]
  private void NetworkSendSlot(int idx) {
    _slots.RemoveAt(idx);
  }

  protected override void ReceiveSlot(GameObject obj) {
    NetworkViewID netIdx = obj.gameObject.GetComponent<NetworkView>().viewID;
    networkView.RPC("NetworkReceiveSlot", RPCMode.All, netIdx);
  }

  [RPC]
  private void NetworkReceiveSlot(NetworkViewID id) {
    GameObject obj = NetworkView.Find(id).observed.gameObject;
    obj.transform.parent = this.gameObject.transform;
    obj.layer = this.gameObject.layer;
    obj.transform.localRotation = Quaternion.identity;
    obj.GetComponent<ImageAnimator>().MoveTo(new Vector3(-8 + slots.Count * 1.5f, 0, 0));
    _slots.Add(obj);
  }

  [RPC]
  protected override void NetworkUpdateSprite() {
    for (int i = 0; i < slots.Count; i++) {
      networkView.RPC("NetworkTranslateSlot", RPCMode.All, 
          _slots[i].networkView.viewID, new Vector3(-8 + i*1.5f, 0, 0));
      if (visible) {
        slots[i].GetComponent<ImageAnimator>().DrawCard(group[i]);
        slots[i].GetComponent<ImageAnimator>().SetParticles(false);
      }
      else if (Network.player == player.networkPlayer) {
        slots[i].GetComponent<ImageAnimator>().DrawCard(group[i]);
        Card c = CardSet.GetCard(group[i]);
        if (c.useCost == 0 && c.HasEffect(EffectType.ENERGY)) {
          slots[i].GetComponent<ImageAnimator>().SetParticles(true);
        }
      }
      else {
        slots[i].GetComponent<ImageAnimator>().DrawBack();
      }
    }
  }

}
