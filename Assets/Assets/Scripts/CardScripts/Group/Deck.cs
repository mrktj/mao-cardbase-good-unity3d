using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/**
 * A Deck is a Group that can only be randomly drawn from
 */
[Serializable]
public class Deck : Group {
  public ImageAnimator top; // A DisplaySlot at the top of the Deck
  private static System.Random random = new System.Random(); 

  void OnEnable() {
    _group = new List<int>();
    if (Network.isServer) {
      top = ImageSet.GetNewBlank(gameObject);
      networkView.RPC("NetworkSetTop", RPCMode.Others, top.networkView.viewID);
      Init();
      UpdateSprite();
    }
  }

	void Update () {
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

  [RPC]
  private void NetworkSetTop(NetworkViewID id) {
    top = NetworkView.Find(id).observed.gameObject.GetComponent<ImageAnimator>();
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

  public void DealCard(Group g) {
    Group.MoveDisplaySlot(random.Next(group.Count), this, g);
    UpdateSprite();
    g.UpdateSprite();
  }
}
