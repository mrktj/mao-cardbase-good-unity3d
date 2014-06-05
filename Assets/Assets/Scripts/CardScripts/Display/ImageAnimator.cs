using UnityEngine;
using System.Collections;

public class ImageAnimator : MonoBehaviour {
  public Animator animator;
  public GameObject particles;
  public GameObject buyCost;
  public TextMesh[] texts;
  public TextOutline[] outlines;
  public float[] initials;
  public float fontScaling = 2f;
  public float outlineScaling = 1.5f;

  public int _cardValue;
  public int cardValue { get { return _cardValue;} }
  public bool changingFont = false;

  public static float moveTime = 0.3f;
  public static float animTime = 0.5f;
  
  void OnEnable() {
    initials = new float[outlines.Length];
    for (int i = 0; i < outlines.Length; i++) {
      initials[i] = outlines[i].pixelSize;
    }
    StartCoroutine(ShrinkFont(2 * moveTime));
  }


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

  public IEnumerator GrowFont(float time) {
    foreach (TextMesh m in texts) {
      m.characterSize = m.characterSize / fontScaling;
      m.fontSize = (int) (m.fontSize * fontScaling);
    }
    float t0 = Time.time;
    float t = Time.time - t0;
    if (changingFont == true) yield return null;
    changingFont = true;
    while (t < time) {
      t = Time.time - t0;
      for (int i = 0; i < outlines.Length; i++) {
        outlines[i].pixelSize = Mathf.Lerp(initials[i]/outlineScaling, initials[i], t/time);
      }
      yield return null;
    }
    changingFont = false;
  }

  public IEnumerator ShrinkFont(float time) {
    float t0 = Time.time;
    float t = Time.time - t0;
    if (changingFont == true) yield return null;
    changingFont = true;
    while (t < time) {
      t = Time.time - t0;
      for (int i = 0; i < outlines.Length; i++) {
        outlines[i].pixelSize = Mathf.Lerp(initials[i], initials[i]/outlineScaling, t/time);
      }
      yield return null;
    }
    changingFont = false;
    foreach (TextMesh m in texts) {
      m.characterSize = m.characterSize * fontScaling;
      m.fontSize = (int) (m.fontSize / fontScaling);
    }
  }

  public void MakeBig() {
    animator.SetBool("Big", true);
    StartCoroutine(GrowFont(animTime));
  }

  public void MakeSmall() {
    animator.SetBool("Big", false);
    StartCoroutine(ShrinkFont(animTime));
  }

  public void DropText() {
    if (animator.GetBool("Big")) animator.SetBool("Drop", true);
  }

  public void RaiseText() {
    if (animator.GetBool("Big")) animator.SetBool("Drop", false);
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
