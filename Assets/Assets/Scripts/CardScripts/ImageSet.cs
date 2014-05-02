using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class ImageSet {
  [SerializeField]
  public static GameObject CardFrame;

  public static GameObject GetImage(int idx, GameObject parent) {
    if (!CardSet.initialized) throw new System.InvalidOperationException("CardSet is not initialized");
    GameObject image = GameObject.Instantiate(CardFrame, Vector3.zero, Quaternion.identity) as GameObject;
    image.transform.parent = parent.transform;
    image.transform.localPosition = Vector3.zero;
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
    return image;
  }

}
