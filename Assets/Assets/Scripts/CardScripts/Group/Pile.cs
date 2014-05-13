using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A Pile acts like a stack of Cards
 */
[System.Serializable]
public class Pile : Group {
  public List<int> pile { get { return (List<int>) group; } }
  public ImageAnimator top; // A DisplaySlot at the top of the Pile

	void OnEnable () {
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

  [RPC]
  private void NetworkSetTop(NetworkViewID id) {
    top = NetworkView.Find(id).observed.gameObject.GetComponent<ImageAnimator>();
  }

  [RPC]
	protected override void NetworkUpdateSprite () {
    if (pile.Count <= 0) {
      top.DrawBlank();
    }
    else { 
      int cardValue = pile[pile.Count - 1];
      top.DrawCard(cardValue);
    }
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

  public bool DealCard(Group g) { 
    if (pile.Count <= 0) return false;
    Group.MoveDisplaySlot(NewDisplaySlot(pile[pile.Count - 1]), this, g);
    UpdateSprite();
    g.UpdateSprite();
    return true;
  }
}
