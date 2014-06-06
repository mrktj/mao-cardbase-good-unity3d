using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A group acts like a stack of Cards
 */
[System.Serializable]
public class Pile : Group {
  public ImageAnimator top; // A DisplaySlot at the top of the group
  public bool purchaseable = true;

	void OnEnable () {
    _group = new List<int>();
    if (Network.isServer) {
      top = ImageSet.GetNewBlank(gameObject);
      networkView.RPC("NetworkSetTop", RPCMode.Others, top.networkView.viewID);
      Init();
      UpdateSprite();
    }
	}

  public void Setup() {
    if (Network.isServer) {
      if (top == null) {
        top = ImageSet.GetNewBlank(gameObject);
        networkView.RPC("NetworkSetTop", RPCMode.Others, top.networkView.viewID);
      }
      Init();
      UpdateSprite();
    }
  }

	void Update () {
  }

  public void ReturnTo(Group g) {
    for (int i = 0; i < group.Count; i++) {
      Card c = CardSet.GetCard(group[i]);
      if (c.HasEffect(EffectType.RETURN)) {
        Group.MoveDisplaySlot(i, this, g);
        UpdateSprite();
        g.UpdateSprite();
        ReturnTo(g);
        break;
      }
    }
  }

  public Card Peek() {
    return CardSet.GetCard(group[group.Count - 1]);
  }

  [RPC]
  private void NetworkSetTop(NetworkViewID id) {
    top = NetworkView.Find(id).observed.gameObject.GetComponent<ImageAnimator>();
  }

  [RPC]
	protected override void NetworkUpdateSprite () {
    if (group.Count <= 0) {
      top.DrawBlank();
      top.GetComponent<ImageAnimator>().SetBuyCost(false);
    }
    else { 
      int cardValue = group[group.Count - 1];
      top.DrawCard(cardValue);
      if (purchaseable) top.GetComponent<ImageAnimator>().SetBuyCost(true);
    }
	}
  
  protected override GameObject SendSlot(int idx) {
    return NewDisplaySlot(group[idx]);
  }

  protected override void ReceiveSlot(GameObject obj) {
    NetworkViewID netIdx = obj.GetComponent<NetworkView>().viewID;
    networkView.RPC("NetworkReceiveSlot", RPCMode.All, netIdx);
    networkView.RPC("NetworkTranslateDestroy", RPCMode.All, obj.networkView.viewID, Vector3.zero);
  }

  [RPC]
  private void NetworkReceiveSlot(NetworkViewID id) {
    GameObject obj = NetworkView.Find(id).observed.gameObject;
    obj.transform.parent = this.gameObject.transform;
    obj.layer = this.gameObject.layer;
    obj.GetComponent<ImageAnimator>().Revert();
  }

  public void DealCard(Group g) { 
    Group.MoveDisplaySlot(group.Count - 1, this, g);
    UpdateSprite();
    g.UpdateSprite();
  }
}
