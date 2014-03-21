using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CardManager : MonoBehaviour {
  public CardSet cardSet;
  public Hand[] hands;
  public Deck[] decks;
  public Pile[] piles;
  public Table[] tables;
  public Hand handPrefab;

  public int numHands { get { return hands.Length; } }
  public int numDecks { get { return decks.Length; } }
  public int numPiles { get { return piles.Length; } }
  private int numPlayers = 2;
  private bool gameStart = false; 
  private int playerNumber = -1;
  private bool dealt = false;

	void Start () {
	}
	
	void Update () {
    if (gameStart && Input.GetMouseButtonDown(0)) {
      Vector2 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Collider2D clicked = Physics2D.OverlapPoint(click);
      if (clicked) {
        if (clicked.gameObject.layer == LayerMask.NameToLayer("Decks")) {
          //clicked.gameObject.GetComponent<Deck>().DealCard(hands[playerNumber]);
          if (!dealt) {
            for (int i = 0; i < numPlayers; i++) {
              decks[0].DealCard(hands[i]);
              decks[0].DealCard(hands[i]);
              decks[0].DealCard(hands[i]);
              decks[0].DealCard(hands[i]);
            }
            dealt = true;
          }
          bool deal = true;
          for (int i = 0; i < numPlayers - 1; i++) {
            if (!hands[i].isFull) deal = false;
          }
          if (deal) { 
            tables[0].ClearInto(piles[0]);
            clicked.gameObject.GetComponent<Deck>().DealCard(tables[0]);
            clicked.gameObject.GetComponent<Deck>().DealCard(tables[0]);
            clicked.gameObject.GetComponent<Deck>().DealCard(tables[0]);
            clicked.gameObject.GetComponent<Deck>().DealCard(tables[0]);
          }
        }
        if (clicked.gameObject.layer == LayerMask.NameToLayer("Hands")) {
          DisplayCard slot = clicked.gameObject.GetComponent<DisplayCard>();
          if (slot.transform.parent.GetComponent<Hand>() == hands[playerNumber]) {
            Debug.Log("asd");
            //hands[playerNumber].PlayCard(slot, piles[0]);
            hands[playerNumber].PlayCard(slot, tables[0]);
          }
        }
        if (clicked.gameObject.layer == LayerMask.NameToLayer("Piles")) {
          //clicked.gameObject.GetComponent<Pile>().ShuffleInto(decks[0]);
        }
        if (clicked.gameObject.layer == LayerMask.NameToLayer("Tables")) {
          DisplayCard dc = clicked.gameObject.GetComponent<DisplayCard>();
          if (dc != null && dc.transform.parent.GetComponent<Table>() == tables[0]) {
            if (!hands[playerNumber].isFull) {
              tables[0].PickupCard(dc, hands[playerNumber]);
            }
          }
          //clicked.gameObject.GetComponent<Table>().(decks[0]);
        }
      }
    }
	}

  void OnGUI() {
    if (!gameStart && Network.isServer) {
      if (GUI.Button(new Rect(10, 50, 120, 20), "Start Game")) {
        NetworkInit();
      }
    }
  }

  private void NetworkInit() {
    numPlayers = Network.connections.Length + 1; 
    networkView.RPC("SetNumber", RPCMode.All, numPlayers);
    for (int i = 0; i < numPlayers; i++) {
      hands[i] = (Hand) Network.Instantiate(handPrefab, new Vector3(0, 5.5f * Mathf.Pow(-1, i), 0), Quaternion.identity, 0);
      NetworkPlayer player = i == 0 ? Network.player : Network.connections[i - 1];
      //NetworkPlayer player = Network.connections[i];
      hands[i].player = player;
      hands[i].gameObject.transform.parent = this.gameObject.transform;
      networkView.RPC("AddHand", RPCMode.Others, hands[i].networkView.viewID, player);
    }
  }

  [RPC]
  private void SetNumber(int num) {
    gameStart = true;
    playerNumber = (int) Int32.Parse(Network.player.ToString());
    //Debug.Log(playerNumber);
    hands = new Hand[num];
  }

  [RPC]
  private void AddHand(NetworkViewID viewID, NetworkPlayer player) {
    Debug.Log(player);
    int pnum = Int32.Parse(player.ToString());
    hands[pnum] = NetworkView.Find(viewID).gameObject.GetComponent<Hand>();
    hands[pnum].player = player;
    hands[pnum].gameObject.transform.parent = this.gameObject.transform;
    numPlayers = pnum + 1;
  }
}
