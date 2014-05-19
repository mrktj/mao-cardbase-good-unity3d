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

  public GameObject Instance;
  public GameObject Card;   // Image template of a Card 

#endregion
#region Private Static Variables
  
  /**
   * These static variables should be the same as the public variables
   * They are static so that they can be accessed any time
   */
  private static GameObject CardFrame;
  private static GameObject SetInstance;

#endregion
#region Unity Methods

  void OnEnable() {
    // Set the static variables 
    CardFrame = Card;
    SetInstance = Instance;
  }

#endregion
#region Public Static Methods

  /**
   * Return the image of the Card with cardValue IDX as a GameObject
   * The returned GameObject will be the child of PARENT
   */
  public static GameObject GetNewImage(int idx, GameObject parent) {
    if (!CardSet.initialized) 
      throw new System.InvalidOperationException("CardSet is not initialized");
    GameObject image = 
      Network.Instantiate(CardFrame, Vector3.zero, Quaternion.identity, 0)
      as GameObject;
    SetInstance.networkView.RPC("NetworkInitCard", RPCMode.All, image.networkView.viewID,
      parent.networkView.viewID, parent.layer, idx);
    image.GetComponent<ImageAnimator>().DrawCard(idx);
    return image;
  }

  [RPC]
  private void NetworkInitCard(NetworkViewID ID, NetworkViewID parentID, int layer, int idx) {
    GameObject image = NetworkView.Find(ID).observed.gameObject;
    image.transform.parent = NetworkView.Find(parentID).observed.gameObject.transform;
    image.transform.localPosition = Vector3.zero;
    image.transform.rotation = Quaternion.identity;
    image.layer = layer;
    foreach (Transform child in image.GetComponentsInChildren<Transform>()) {
      child.gameObject.layer = layer;
    }
  }
  
  /** 
   * Return the image of the outline of a Card as a GameObject
   * The returned GameObject will be the child of PARENT
   */
  public static ImageAnimator GetNewBlank(GameObject parent) {
    ImageAnimator obj = GetNewImage(0, parent).GetComponent<ImageAnimator>();
    obj.DrawBlank();
    return obj;
  }

  /** 
   * Return the image of the back of a Card as a GameObject
   * The returned GameObject will be the child of PARENT
   */
  public static ImageAnimator GetNewBack(GameObject parent) {
    ImageAnimator obj = GetNewImage(0, parent).GetComponent<ImageAnimator>();
    obj.DrawBack();
    return obj;
  }

#endregion
}
