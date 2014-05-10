using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
 * A manager class to generate images of Cards
 * There should always be exactly one instance of this class (no checks)
 */
public class ImageSet : MonoBehaviour {
#region Public Variables

  public GameObject Card;   // Image template of a Card 
  public GameObject Blank;  // Image of the outline of a Card 
  public GameObject Back;   // Image of the back of a Card

#endregion
#region Private Static Variables
  
  /**
   * These static variables should be the same as the public variables
   * They are static so that they can be accessed any time
   */
  private static GameObject CardFrame;
  private static GameObject BlankCard;
  private static GameObject CardBack;

#endregion
#region Unity Methods

  void OnEnable() {
    // Set the static variables 
    CardFrame = Card;
    BlankCard = Blank;
    CardBack = Back;
  }

#endregion
#region Public Static Methods

  /**
   * Return the image of the Card with cardValue IDX as a GameObject
   * The returned GameObject will be the child of PARENT
   */
  public static GameObject GetImage(int idx, GameObject parent) {
    if (!CardSet.initialized) 
      throw new System.InvalidOperationException("CardSet is not initialized");
    GameObject image = 
      GameObject.Instantiate(CardFrame, Vector3.zero, Quaternion.identity) 
      as GameObject;
    image.transform.parent = parent.transform;
    //image.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    image.transform.localPosition = Vector3.zero;
    int parentLayer = parent.layer;
    image.layer = parentLayer;
    foreach (Transform child in image.GetComponentsInChildren<Transform>()) {
      child.gameObject.layer = parentLayer;
    }
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

  /** 
   * Return the image of the outline of a Card as a GameObject
   * The returned GameObject will be the child of PARENT
   */
  public static GameObject GetBlank(GameObject parent) {
    if (!CardSet.initialized) 
      throw new System.InvalidOperationException("CardSet is not initialized");
    GameObject image = 
      GameObject.Instantiate(BlankCard, Vector3.zero, Quaternion.identity) 
      as GameObject;
    image.transform.parent = parent.transform;
    image.transform.localPosition = Vector3.zero;
    int parentLayer = parent.layer;
    image.layer = parentLayer;
    foreach (Transform child in image.GetComponentsInChildren<Transform>()) {
      child.gameObject.layer = parentLayer;
    }
    return image;
  }

  /** 
   * Return the image of the back of a Card as a GameObject
   * The returned GameObject will be the child of PARENT
   */
  public static GameObject GetBack(GameObject parent) {
    if (!CardSet.initialized) 
      throw new System.InvalidOperationException("CardSet is not initialized");
    GameObject image = 
      GameObject.Instantiate(CardBack, Vector3.zero, Quaternion.identity) 
      as GameObject;
    image.transform.parent = parent.transform;
    image.transform.localPosition = Vector3.zero;
    int parentLayer = parent.layer;
    image.layer = parentLayer;
    foreach (Transform child in image.GetComponentsInChildren<Transform>()) {
      child.gameObject.layer = parentLayer;
    }
    return image;
  }

#endregion
}
