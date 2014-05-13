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

  protected override void SendSlot(GameObject obj) {
    return;
  }

  public bool DealCard(Group g) {
    if (group.Count <= 0) return false;
    int randomCard = group.ElementAt(random.Next(group.Count)); 
    Group.MoveDisplaySlot(NewDisplaySlot(randomCard), this, g);
    UpdateSprite();
    g.UpdateSprite();
    return true;
  }
}
