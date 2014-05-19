using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  public Deck _deck;
  public Hand _hand;
  public Pile _discard;
  public Hand _table;
  private int _energy;
  private int _health;
  private int _attack;
  private int _defense;
  private bool _done;
  public bool done { get { return _done; } }
  public int energy { get { return _energy; } }
  public int health { get { return _health; } }
  public int attack { get { return _attack; } }
  public int defense { get { return _defense; } }
  
  public int DefaultHandSize = 4;
  public int DefaultHealth = 20;
  public NetworkPlayer networkPlayer;
  public Player opponent;

	void OnEnable () {
    _health = DefaultHealth;
    _done = false;
	}
	
	void Update () {
  }
  
  public void EndTurn() {
    networkView.RPC("NetworkEndTurn", RPCMode.All);
  }

  public IEnumerator NewTurn() {
    networkView.RPC("NetworkNewTurn", RPCMode.All);
    _table.ClearInto(_discard);
    _hand.ClearInto(_discard);
    yield return new WaitForSeconds(ImageAnimator.moveTime * 2);
    Draw(DefaultHandSize);
  }

  public void TryPlayCard(GameObject dc) {
    Card played = CardSet.GetCard(dc.GetComponent<ImageAnimator>().cardValue);
    if (played.useCost > energy) return;
    UseEnergy(played.useCost);
    _hand.PlayCard(dc, _table);
    played.ApplyCardEffects(this, opponent);
  }

  public void GainEnergy(int val) {
    if (val <= 0) throw new System.ArgumentException("Parameter must be positive", "val");
    networkView.RPC("NetworkGainEnergy", RPCMode.All, val);
  }

  public void UseEnergy(int val) {
    if (val < 0) throw new System.ArgumentException("Parameter must be positive", "val");
    networkView.RPC("NetworkUseEnergy", RPCMode.All, val);
  }

  public void Attack(int val) {
    if (val <= 0) throw new System.ArgumentException("Parameter must be positive", "val");
    networkView.RPC("NetworkAttack", RPCMode.All, val);
  }

  public void Defend(int val) {
    if (val <= 0) throw new System.ArgumentException("Parameter must be positive", "val");
    networkView.RPC("NetworkDefend", RPCMode.All, val);
  }

  public void TakeDamage(int val) {
    if (val <= 0) return;
    networkView.RPC("NetworkTakeDamage", RPCMode.All, val);
  }

  public void Heal(int val) {
    if (val <= 0) throw new System.ArgumentException("Parameter must be positive", "val");
    networkView.RPC("NetworkHeal", RPCMode.All, val);
  }

  public void Draw(int val) {
    if (val <= 0) throw new System.ArgumentException("Parameter must be positive", "val");
    for (int i = 0; i < val; i++) {
      DrawCard();
    }
  }

  public void DrawCard() {
    if (_deck.group.Count <= 0) {
      _discard.ShuffleInto(_deck);
    }
    _deck.DealCard(_hand);
  }
  
  public void TryGainCard(Pile p) {
    if (p.numDefault <= 0) return;
    Card pileCard = CardSet.GetCard(p.defaultCard);
    if (pileCard.buyCost > energy) return;
    UseEnergy(pileCard.buyCost);
    GainCard(p);
  }

  public void GainCard(Pile p) {
    p.DealCard(_discard);
  }

  [RPC]
  private void NetworkGainEnergy(int val) {
    _energy += val;
  }

  [RPC]
  private void NetworkUseEnergy(int val) {
    _energy -= val;
  }

  [RPC]
  private void NetworkTakeDamage(int val) {
    _health -= val;
  }

  [RPC]
  private void NetworkHeal(int val) {
    _health += val;
  }

  [RPC]
  private void NetworkDefend(int val) {
    _defense += val;
  }

  [RPC]
  private void NetworkAttack(int val) {
    _attack += val;
  }

  [RPC]
  private void NetworkEndTurn() {
    _done = true;
  }

  [RPC]
  private void NetworkNewTurn() {
    _done = false;
    _energy = 0;
    _attack = 0;
    _defense = 0;
  }

}
