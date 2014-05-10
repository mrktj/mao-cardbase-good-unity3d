using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A Hand is a Group where all the Cards are only visible to one Player
 */
public class Hand : Group {
  private List<DisplaySlot> _hand; // A List of DisplaySlots to display Cards
  public List<DisplaySlot> hand { get { return _hand;} } 
  public List<int> cards { get { return (List<int>) group; } }
  public GameObject displayCardPrefab; // The DisplaySlot prefab
  public Player player; // the Player this hand belongs to
  
	void OnEnable () {
	  _group = new List<int>();
    _hand = new List<DisplaySlot>();
    Init();
	}
	
  /**
   * Clear the Hand and place all Cards into Group G
   */
  public bool ClearInto(Group g) {
    networkView.RPC("NetworkClearHand", RPCMode.All);
    ShuffleInto(g);
    return true;
  }

  /**
   * Play the Card shown in DisplaySlot DC into Group G
   */
  public bool PlayCard(DisplaySlot dc, Group g) {
    Group.MoveCard(dc.cardValue, this, g);
    UpdateSprite();
    g.UpdateSprite();
    return true;
  }

  [RPC]
  private void NetworkClearHand() {
    foreach (DisplaySlot dc in hand) {
      UnityEngine.Object.Destroy(dc.gameObject);
    }
    hand.Clear();
  }

  [RPC]
  private void NetworkDestroyDisplaySlot(int idx) {
    DisplaySlot dc = hand[idx];
    _hand.RemoveAt(idx);
    UnityEngine.Object.Destroy(dc.gameObject);
  } 

  [RPC]
  private void NetworkNewDisplaySlot(float x, float y, int cardValue) {
    GameObject go = Instantiate(displayCardPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    go.layer = this.gameObject.layer;
    go.transform.parent = this.gameObject.transform;
    go.transform.localPosition = new Vector3(x,y,0);
    DisplaySlot dc = go.GetComponent<DisplaySlot>();
    _hand.Add(dc);
    //dc.DrawCard(cardValue);
  }

  [RPC]
  protected override void NetworkUpdateSprite() {
    if (hand.Count < cards.Count) {
      for (int i = hand.Count; i < cards.Count; i++) {
        networkView.RPC("NetworkNewDisplaySlot", RPCMode.All, -8 + i * 1.5f, 0f, cards[i]);
      }
    }
    if (hand.Count > cards.Count) {
      for (int i = cards.Count; i < hand.Count; i++) {
        networkView.RPC("NetworkDestroyDisplaySlot", RPCMode.All, i);
      }
    }
    
    for (int i = 0; i < hand.Count; i++) {
     if (Network.player == player.networkPlayer)
       hand[i].DrawCard(cards[i]);
     else 
       hand[i].DrawBack();
    }
  }

}
