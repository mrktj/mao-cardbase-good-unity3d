using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : Group {
  private List<DisplayCard> _table;
  public List<DisplayCard> table { get { return _table;} } 
  public List<int> cards { get { return (List<int>) group; } }

  public SpriteSet sprites;
  public GameObject displayCardPrefab;

	void OnEnable () {
    _group = new List<int>();
    _table = new List<DisplayCard>();
	}
	
	void Update () {
	}

  public bool ClearInto(Group g) {
    networkView.RPC("NetworkClearTable", RPCMode.All);
    ShuffleInto(g);
    return true;
  }

  public bool PickupCard(DisplayCard dc, Group g) {
    networkView.RPC("NetworkDestroyCard", RPCMode.All, dc.cardValue);
    foreach (int i in group) {
      if (i == dc.cardValue) {
        Group.MoveCard(i, this, g);
        UpdateSprite();
        g.UpdateSprite();
        return true;
      }
    }
    return false;
  }

  [RPC]
  private void NetworkClearTable() {
    foreach (DisplayCard dc in table) {
      UnityEngine.Object.Destroy(dc.gameObject);
    }
    table.Clear();
  }

  [RPC]
  private void NetworkDestroyDisplayCard(int idx) {
    DisplayCard dc = table[idx];
    _table.RemoveAt(idx);
    UnityEngine.Object.Destroy(dc.gameObject);
  } 

  [RPC]
  private void NetworkNewDisplayCard(float x, float y, int cardValue) {
    GameObject go = Instantiate(displayCardPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    go.layer = this.gameObject.layer;
    go.transform.parent = this.gameObject.transform;
    go.transform.localPosition = new Vector3(x,y,0);
    DisplayCard dc = go.GetComponent<DisplayCard>();
    _table.Add(dc);
    dc.Init();
    dc.sprites = sprites;
  }

  [RPC]
  protected override void NetworkUpdateSprite() {
    if (table.Count < cards.Count) {
      for (int i = table.Count; i < cards.Count; i++) {
        networkView.RPC("NetworkNewDisplayCard", RPCMode.All, -8 + i * 1.5f, 0f, cards[i]);
      }
    }
    if (table.Count > cards.Count) {
      for (int i = cards.Count; i < table.Count; i++) {
        networkView.RPC("NetworkDestroyDisplayCard", RPCMode.All, i);
      }
    }
    
    for (int i = 0; i < table.Count; i++) {
       table[i].DrawCard(cards[i]);
    }
    
  }
}
