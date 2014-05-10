using UnityEngine;
using System.Collections;

public class PlayerUI : MonoBehaviour {
  public Player player;
  public TextMesh HealthUI;
  public TextMesh EnergyUI;
  public TextMesh AttackUI;
  public TextMesh DefenseUI;

	void Update () {
    this.gameObject.transform.rotation = Quaternion.identity;
    HealthUI.text = player.health.ToString();
    EnergyUI.text = player.energy.ToString();
    AttackUI.text = player.attack.ToString();
    DefenseUI.text = player.defense.ToString();
	}
}
