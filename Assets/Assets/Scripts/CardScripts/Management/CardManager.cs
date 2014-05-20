using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * The main manager Class
 */
public class CardManager : MonoBehaviour {
  public Player[] players;
  public List<Pile> piles;
  public Pile trash; 
  public Player playerPrefab;
  public GameObject defaultPiles;

  public int numPiles { get { return piles.Count; } }
  private int numPlayers = 2;
  private bool gameStart = false; 
  private int playerNumber = -1;
  private Collider2D hovering = null;
  private float lastHover = 0; 
  private int[] classes = new int[2];
  private int gridInt = 0;


  /**
   * The main loop
   * Nothing should happen if the game hasn't started yet
   */
	void Update () {
    if (gameStart) {
      Vector2 hover = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Collider2D hovered = Physics2D.OverlapPoint(hover);

      // Nothing is being hovered over, revert previous Card to normal size
      if (hovering != null && (hovered == null || hovered != hovering)) { 
        bool inHand = hovering.gameObject.layer == LayerMask.NameToLayer("Players");
        hovering.GetComponent<ImageAnimator>().MakeSmall(inHand);
        hovering = null;
      }
      // If a Card is being hovered over, make it temporarily larger
      if (hovered != hovering && hovered.gameObject.tag == "CardFrame"
          && Time.time - lastHover > 0.2) {
        lastHover = Time.time;
        hovering = hovered;
        bool inHand = hovering.gameObject.layer == LayerMask.NameToLayer("Players");
        hovering.GetComponent<ImageAnimator>().MakeBig(inHand);
      }
    }

    /* If the player has not completed their turn, check to see if the 
     * player clicks things 
     */
    if (gameStart && Input.GetMouseButtonDown(0) && !players[playerNumber].done) {
      Vector2 click = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      Collider2D clicked = Physics2D.OverlapPoint(click);
      if (clicked) {
        // If a DisplaySlot in the Hand is clicked try to play it
        if (clicked.gameObject.layer == LayerMask.NameToLayer("Players")) {
          Hand hand = clicked.transform.parent.GetComponent<Hand>();
          if (hand != null && 
              hand.transform.parent.GetComponent<Player>() == players[playerNumber]) {
            players[playerNumber].TryPlayCard(clicked.gameObject);
          }
        }
        // If a DisplaySlot in a Pile is clicked try to purchase it
        if (clicked.gameObject.layer == LayerMask.NameToLayer("Piles")) {
          Pile p = clicked.transform.parent.GetComponent<Pile>();
          if (p != null) {
            players[playerNumber].TryGainCard(p);
          }
        }
      }
    }
	}


  [RPC]
  public void NetworkGrid(NetworkPlayer p, int val) {
    int pnum =  (int) Int32.Parse(p.ToString());
    classes[pnum] = val;
  }

  void OnGUI() {
    if (!gameStart && (Network.isServer || Network.isClient)) {
      gridInt = GUI.SelectionGrid(new Rect(Screen.width/2, Screen.height/2, 100, 80), 
          gridInt, new string[] {"Rogue", "Mage", "Priest"}, 1);
      networkView.RPC("NetworkGrid", RPCMode.AllBuffered, Network.player, gridInt);
    }
    // Draw Start Game button for the Server
    if (!gameStart && Network.isServer) {
      if (GUI.Button(new Rect(Screen.width - 130, 10, 120, 20), "Start Game")) {
        GameInit();
        foreach (Player p in players) {
          StartCoroutine(p.NewTurn());
        }
      }
    }
    if (gameStart) {
      // Draw End Turn buttons for both players
      if (!players[playerNumber].done && 
          GUI.Button(new Rect(Screen.width - 130, 10, 120, 20), "End Turn")){ 
        players[playerNumber].EndTurn();
      }
    }
    if (gameStart) {

      // Check to see if both players have ended their turn
      bool end = true;
      foreach (Player p in players) {
        if (p.done == false) {
          end = false;
          break;
        }
      }
      // Apply damage and start a new turn 
      if (end == true) {
        if (players.Length == 2) {
        players[0].TakeDamage(players[1].attack - players[0].attack);
        players[1].TakeDamage(players[0].attack - players[1].attack);
        }
        foreach (Player p in players) {
          StartCoroutine(p.NewTurn());
        }
      }
    }
  }

  /**
   * Initialize the Game by instantiating playerPrefabs and initializing values
   * over the network.
   */
  private void GameInit() {
    numPlayers = Network.connections.Length + 1; 
    networkView.RPC("SetNumber", RPCMode.All, numPlayers);
    for (int i = 0; i < numPlayers; i++) {
      Vector3 pos = new Vector3(0, -5.5f * Mathf.Pow(-1, i), 0);
      Quaternion rot = Quaternion.Euler(0,0,180*-i);
      players[i] = (Player) Network.Instantiate(playerPrefab, pos, rot, 0);
      NetworkPlayer player = i == 0 ? Network.player : Network.connections[i - 1];
      //NetworkPlayer player = Network.connections[i];
      players[i].networkPlayer = player;
      players[i].gameObject.transform.parent = this.gameObject.transform;
      players[i].manager = this;
      networkView.RPC("AddPlayer", RPCMode.Others, 
          players[i].networkView.viewID, player, this.networkView.viewID);
    }

    GameObject defaults = (GameObject) 
      Network.Instantiate(defaultPiles, new Vector3(0, 0, 0), Quaternion.identity, 0);
    gameStart = true;

    networkView.RPC("Post", RPCMode.All, defaults.networkView.viewID);
    // Swap the positions of the players so that you're always on the bottom
    networkView.RPC("Swap", RPCMode.Others);
  }

  public Pile GetPileFor(int cardValue) {
    foreach (Pile p in piles) {
      if (p.defaultCard == cardValue) return p;
    }
    return null;
  }

  /**
   * Swap the positions of the player hands
   * Do Other things post initialize
   */
  [RPC]
  private void Swap() {
    Vector3 temp = players[0].transform.position;
    players[0].transform.position = players[1].transform.position;
    players[1].transform.position = temp;
    Quaternion temp2 = players[0].transform.rotation;
    players[0].transform.rotation = players[1].transform.rotation;
    players[1].transform.rotation = temp2;
  }

  [RPC]
  private void Post(NetworkViewID id) {
    players[0].InitHand(classes[0]);
    if (players.Length == 2) {
      players[0].opponent = players[1];
      players[1].opponent = players[0];
      players[1].InitHand(classes[1]);
    }

    Pile[] defaults = NetworkView.Find(id).GetComponentsInChildren<Pile>();
    piles = new List<Pile>(defaults);
    piles.Add(trash);
    gameStart = true;
  }

  /**
   * Set the playerNumber and the list of players
   */
  [RPC]
  private void SetNumber(int num) {
    playerNumber = (int) Int32.Parse(Network.player.ToString());
    players = new Player[num];
  }

  /**
   * Tell everyone about the instantiated players
   */
  [RPC]
  private void AddPlayer(NetworkViewID viewID, NetworkPlayer player, NetworkViewID managerID) {
    int pnum = Int32.Parse(player.ToString());
    players[pnum] = NetworkView.Find(viewID).GetComponent<Player>();
    players[pnum].networkPlayer = player;
    players[pnum].gameObject.transform.parent = this.gameObject.transform;
    players[pnum].manager = NetworkView.Find(managerID).GetComponent<CardManager>();
    numPlayers = pnum + 1;
  }
}
