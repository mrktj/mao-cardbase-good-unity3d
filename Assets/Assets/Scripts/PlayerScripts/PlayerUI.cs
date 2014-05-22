using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {
  public Player player;
  public TextMesh HealthUI;
  public TextMesh EnergyUI;
  public TextMesh AttackUI;
  public TextMesh DeckUI;
  public TextMesh DiscardUI;

	void Update () {
    this.gameObject.transform.rotation = Quaternion.identity;
    HealthUI.text = player.health.ToString();
    EnergyUI.text = player.energy.ToString();
    AttackUI.text = player.attack.ToString();
    DeckUI.text = player._deck.numCards.ToString();
    DiscardUI.text = player._discard.numCards.ToString();
	}
}
