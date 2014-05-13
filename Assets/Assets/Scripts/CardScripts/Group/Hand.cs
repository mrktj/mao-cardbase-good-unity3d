using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A Hand is a Group where all the Cards are only visible to one Player
 * If player is null, then the hand is open and all cards are visible
 */
public class Hand : Group {
  private List<GameObject> _slots; // A List of DisplaySlots to display Cards
  public List<GameObject> slots { get { return _slots;} } 
  public List<int> cards { get { return (List<int>) group; } }
  //public GameObject displayCardPrefab; // The DisplaySlot prefab
  public Player player; // the Player this hand belongs to
  
	void OnEnable () {
	  _group = new List<int>();
    _slots = new List<GameObject>();
    Init();
	}
	
  /**
   * Clear the Hand and place all Cards into Group G
   */
  public bool ClearInto(Group g) {
    while (slots.Count > 0) {
      MoveDisplaySlot(slots[0], this, g);
    }
    //ShuffleInto(g);
    return true;
  }

  /**
   * Play the Card shown in DisplaySlot obj into Group G
   */
  public bool PlayCard(GameObject obj, Group g) {
    Hand.MoveDisplaySlot(obj, this, (Hand) g);
    return true;
  }

  /**
   * A hand is open if player is null. 
   * Anyone can see the contents of an open hand
   */
  public bool IsOpen() {
    if (player == null) return true;
    return false;
  }

  [RPC]
  private void NetworkClearHand() {
    slots.Clear();
  }

  protected override void SendSlot(GameObject obj) {
    int idx = _slots.IndexOf(obj);
    networkView.RPC("NetworkSendSlot", RPCMode.All, idx);
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
      if (player == null || Network.player == player.networkPlayer)
        slots[i].GetComponent<ImageAnimator>().DrawCard(cards[i]);
      else 
        slots[i].GetComponent<ImageAnimator>().DrawBack();
    }
  }

}
