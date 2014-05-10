using UnityEngine;
using System.Collections;

public class StartupManager : MonoBehaviour {
  public string filename;

	void OnEnable () {
	  CardSet.loadCards(filename);
	}
}
