using UnityEngine;
using System.Collections;

public class ImageAnimator : MonoBehaviour {
  public Animator animator;
  public int _cardValue;
  public int cardValue { get { return _cardValue;} }

  public static float moveTime = 0.3f;

  public IEnumerator SmoothMove(Vector3 end, float time) {
    float t = 0;
    Vector3 start = transform.localPosition;
    while (t < time) {
      transform.localPosition = Vector3.Lerp(start, end, t/time);
      t += Time.deltaTime;
      yield return null;
    }
  }

  public void MoveTo(Vector3 pos) {
    StartCoroutine(SmoothMove(pos, moveTime));
  }

  public void MakeBig(bool inHand) {
    animator.SetBool("Big", true);
    if (inHand) animator.SetBool("Hand", inHand);
  }

  public void MakeSmall(bool inHand) {
    animator.SetBool("Big", false);
    if (inHand) animator.SetBool("Hand", inHand);
  }

  public void DrawBlank() {
    animator.SetBool("Back", false);
    animator.SetBool("Blank", true);
  }

  public void DrawBack() {
    animator.SetBool("Blank", false);
    animator.SetBool("Back", true);
  }

  public void Revert() {
    DrawCard(cardValue);
  }

  public void DrawCard(int val) {
    _cardValue = val;
    animator.SetBool("Blank", false);
    animator.SetBool("Back", false);
    animator.SetTrigger("Revert");
    networkView.RPC("NetworkDrawCard",RPCMode.All, networkView.viewID, val);
  }

  [RPC]
  public void NetworkDrawCard(NetworkViewID ID, int idx) {
    GameObject image = NetworkView.Find(ID).observed.gameObject;
    Card card = CardSet.GetCard(idx);
    foreach (TextMesh t in image.GetComponentsInChildren<TextMesh>()) {
      switch (t.text) {
        case "b":
          t.text = card.buyCost.ToString();
          break;
        case "u":
          t.text = card.useCost.ToString();
          break;
        case "Text":
          t.text = card.fullText;
          break;
        case "Name":
          t.text = card.name;
          break;
      }
    }
  }
}
