using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Table : Group {
  private List<DisplayCard> _table;
  public List<DisplayCard> table { get { return _table;} } 

  public CardSet sprites;
  public GameObject displayCard;
  public float width; 
  public float height;

	void Start () {
    _group = new List<Card>();
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
    foreach (Card c in group) {
      if (c.cardValue == dc.cardValue) {
        Group.MoveCard(c, this, g);
        UpdateSprite();
        g.UpdateSprite();
        return true;
      }
    }
    return false;
  }

  [RPC]
  private void NetworkDestroyCard(int cardValue) {
    foreach (DisplayCard dc in table) {
      if (dc.cardValue == cardValue) { 
        _table.Remove(dc);
        UnityEngine.Object.Destroy(dc.gameObject);
        return;
      }
    }
  } 

  [RPC]
  private void NetworkClearTable() {
    foreach (DisplayCard dc in table) {
      UnityEngine.Object.Destroy(dc.gameObject);
    }
    table.Clear();
  } 

  [RPC]
  private void NetworkSendCardValues(float x, float y, float rot, int cardValue) {
    GameObject go = Instantiate(displayCard, transform.position + new Vector3(x, y, this.gameObject.transform.localPosition.z), Quaternion.Euler(0, 0, rot)) as GameObject;
    go.layer = this.gameObject.layer;
    go.transform.parent = this.gameObject.transform;
    DisplayCard dc = go.GetComponent<DisplayCard>();
    table.Add(dc);
    dc.Init();
    dc.sprites = sprites;
    dc.DrawCard(cardValue);
  }

  [RPC]
  protected override void NetworkUpdateSprite () {
    foreach (Card c in group) {
      bool drawn = false;
      foreach (DisplayCard dc in table) {
        if (dc.cardValue == c.cardValue) {
          drawn = true;
          break;
        }
      }
      if (!drawn) {
        float x = Random.Range(-width/2, width/2);
        float y = Random.Range(-height/2, height/2);
        float rot = Random.Range(0, 360);
        networkView.RPC("NetworkSendCardValues", RPCMode.All, x, y, rot, c.cardValue);
      }
    }
  }
}
