using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
  public int playerClass;
  public Deck _deck;
  public Hand _hand;
  public Pile _discard;
  public Hand _table;
  private int _energy;
  private int _health;
  private int _attack;
  private bool _done;
  public bool done { get { return _done; } }
  public int energy { get { return _energy; } }
  public int health { get { return _health; } }
  public int attack { get { return _attack; } }
  
  public int DefaultHandSize = 4;
  public int DefaultHealth = 20;
  public NetworkPlayer networkPlayer;
  public Player opponent;
  public CardManager manager;

	void OnEnable () {
    _health = DefaultHealth;
    _done = false;
	}
  
  public void InitHand(int Class) {
    playerClass = Class;
    _hand.defaultCard = CardSet.classCards[playerClass];
    _hand.numDefault = 1;
    _hand.Setup();
  }
	
	void Update () {
  }
  
  public void EndTurn() {
    networkView.RPC("NetworkEndTurn", RPCMode.All);
  }


  public void ResetTurn() {
    networkView.RPC("NetworkNewTurn", RPCMode.All);
  }

  public IEnumerator NewTurn() {
    _table.ClearInto(_discard);
    _hand.ClearInto(_discard);
    /*
    int count = _table.slots.Count;
    for (int i = 0; i < count; i++) {
      GameObject discarded = _table.slots[0];
      Card discardedCard = CardSet.GetCard(discarded.GetComponent<ImageAnimator>().cardValue);
      Group.MoveDisplaySlot(discarded, _table, _discard);
      discardedCard.OnDiscardEffects(this, opponent);
    }
    
    count = _hand.slots.Count;
    for (int i = 0; i < count; i++) {
      GameObject discarded = _hand.slots[0];
      Card discardedCard = CardSet.GetCard(discarded.GetComponent<ImageAnimator>().cardValue);
      Group.MoveDisplaySlot(discarded, _hand, _discard);
      discardedCard.OnDiscardEffects(this, opponent);
    }
    */
    _discard.ReturnTo(_hand);
    yield return new WaitForSeconds(ImageAnimator.moveTime * 2);
    Draw(DefaultHandSize);
  }

  public void TrashCard(int cardValue) {
    if (cardValue < 0) {
      int count = _table.slots.Count;
      GameObject trash = _table.slots[count - 1];
      _table.PlayCard(trash, manager.trash);
    }
  }

  public void GetNew(int cardValue) {
    Pile p = manager.GetPileFor(cardValue, this);
    if (p.isEmpty) {
      return;
    }
    p.DealCard(_discard);
  }

  public void GiveNew(int cardValue) {
    Pile p = manager.GetPileFor(cardValue, this);
    if (p.isEmpty) {
      return;
    }
    p.DealCard(opponent._discard);
  }

  public void TryPlayCard(GameObject dc) {
    Card played = CardSet.GetCard(dc.GetComponent<ImageAnimator>().cardValue);
    if (played.useCost > energy) return;
    UseEnergy(played.useCost);
    _hand.PlayCard(dc, _table);
    played.OnPlayEffects(this, opponent);
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
    if (_deck.isEmpty) {
      _discard.ShuffleInto(_deck);
    }
    if (_deck.isEmpty) {
      Debug.Log("empty after shuffle");
    }
    _deck.DealCard(_hand);
  }
  
  public void TryGainCard(Pile p) {
    if (p.isEmpty) return;
    Card pileCard = CardSet.GetCard(p.defaultCard);
    if (pileCard.buyCost > energy) return;
    UseEnergy(pileCard.buyCost);
    GainCard(p);
  }

  public void GainCard(Pile p) {
    if (p.isEmpty) {
      return;
    }
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
  }
}
