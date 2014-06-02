using UnityEngine;
using System.Collections;

public class ImageAnimator : MonoBehaviour {
  public Animator animator;
  public GameObject particles;
  public GameObject buyCost;
  public int _cardValue;
  public int cardValue { get { return _cardValue;} }

  public static float moveTime = 0.3f;

  public IEnumerator SmoothMove(Vector3 end, float time) {
    Vector3 start = transform.localPosition;
    float t0 = Time.time;
    float t = Time.time - t0;
    while (t < time) {
      t = Time.time - t0;
      transform.localPosition = Vector3.Lerp(start, end, t/time);
      yield return null;
    }
  }
  
  public void SetBuyCost(bool b) {
    buyCost.SetActive(b);
  }

  public void SetParticles(bool b) {
    particles.SetActive(b);
  }

  public void MoveTo(Vector3 pos) {
    StartCoroutine(SmoothMove(pos, moveTime));
  }

  public void MakeBig(bool inHand) {
    animator.SetBool("Big", true);
    animator.SetBool("Hand", true);
  }

  public void MakeSmall(bool inHand) {
    animator.SetBool("Big", false);
    animator.SetBool("Hand", true);
  }

  public void DrawBlank() {
    SetBuyCost(false);
    animator.SetBool("Back", false);
    animator.SetBool("Blank", true);
  }

  public void DrawBack() {
    SetBuyCost(false);
    animator.SetBool("Blank", false);
    animator.SetBool("Back", true);
  }

  public void Revert() {
    DrawCard(cardValue);
  }

  public void DrawCard(int val) {
    _cardValue = val;
    SetBuyCost(false);
    animator.SetBool("Blank", false);
    animator.SetBool("Back", false);
    animator.SetTrigger("Revert");
    transform.rotation = Quaternion.identity;
    networkView.RPC("NetworkDrawCard",RPCMode.All, networkView.viewID, val);
  }

  [RPC]
  public void NetworkDrawCard(NetworkViewID ID, int idx) {
    GameObject image = NetworkView.Find(ID).observed.gameObject;
    Card card = CardSet.GetCard(idx);
    foreach (TextMesh t in image.GetComponentsInChildren<TextMesh>(true)) {
      switch (t.gameObject.name) {
        case "BuyCost":
          t.text = card.buyCost.ToString();
          break;
        case "UseCost":
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
