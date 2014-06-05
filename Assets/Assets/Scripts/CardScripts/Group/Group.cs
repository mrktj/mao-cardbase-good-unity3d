using UnityEngine;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/**
 * An abstract class that represents a group of Cards
 */
[Serializable]
public abstract class Group : MonoBehaviour {
#region Protected Variables and Accessors

  [SerializeField]
  protected List<int> _group; // The Collection of cardValues
  public List<int> group { 
    get { 
      return _group;
    } 
  }

#endregion 
#region Public Variables

  [HideInInspector]
  public int defaultCard; // The value of the default Card
  [HideInInspector]
  [SerializeField]
  public int numDefault;  // The number of default Cards to start with
  public bool isEmpty   {get { return (group.Count <= 0);}}
  public int numCards  {get { return group.Count;}}

#endregion 
#region Protected Methods

  /* Initialize the group to numDefault defaultCards. */
  protected void Init() {
    for (int i = 0; i < numDefault; i++) {
      networkView.RPC("NetworkAdd", RPCMode.All, defaultCard);
    }
  }

#endregion 
#region Private Methods

  /* Add the Card with cardValue i to the group */
  private void Add(int i) {
    networkView.RPC("NetworkAdd", RPCMode.All, i);
  }

  /* Remove the Card with cardValue i from the group */
  private void Remove(int i) {
    networkView.RPC("NetworkRemove", RPCMode.All, i);
  }

  private void RemoveAt(int idx) {
    networkView.RPC("NetworkRemoveAt", RPCMode.All, idx);
  }

  protected GameObject NewDisplaySlot(int cardValue) {
    return ImageSet.GetNewImage(cardValue, gameObject).gameObject;
  }

  protected void DestroyDisplaySlot(GameObject obj) {
    NetworkViewID id = obj.networkView.viewID;
    Network.Destroy(id);
    //networkView.RPC("NetworkDestroySlot", RPCMode.All, id);
  } 

#endregion 
#region Public Methods

  /* Update the image of the Group */
  public void UpdateSprite() {
    networkView.RPC("NetworkUpdateSprite", RPCMode.All);
  }

  /* Shuffle the Group into Group g */
  public void ShuffleInto(Group g) {
    foreach (int i in group) {
      g.Add(i);
    }
    networkView.RPC("NetworkClear", RPCMode.All);
    UpdateSprite();
    g.UpdateSprite();
  }

#endregion 
#region Static Methods

  /* Move the Card with cardValue i in Group FROM to Group TO */
  public static void MoveCard(int idx, Group from, Group to) {
    int cardVal = from.group[idx];
    from.RemoveAt(idx);
    to.Add(cardVal);
  }
  
  public static void MoveDisplaySlot(int idx, Group from, Group to) {
    GameObject obj = from.SendSlot(idx);
    to.ReceiveSlot(obj);
    Group.MoveCard(idx, from, to);
    from.UpdateSprite();
    to.UpdateSprite();
  }

#endregion 
#region Networking Methods

  [RPC]
  private void NetworkAdd(int cardval) {
    group.Add(cardval);
  }

  [RPC]
  private void NetworkRemove(int cardval) {
    group.Remove(cardval);
  }

  [RPC]
  private void NetworkRemoveAt(int idx) {
    group.RemoveAt(idx);
  }

  [RPC]
  private void NetworkClear() {
    group.Clear();
  }

  [RPC]
  private void NetworkDestroySlot(NetworkViewID id) {
    GameObject obj = NetworkView.Find(id).observed.gameObject;
    UnityEngine.Object.Destroy(obj);
  }

  [RPC]
  private void NetworkTranslateSlot(NetworkViewID id, Vector3 pos) {
    GameObject obj = NetworkView.Find(id).observed.gameObject;
    obj.GetComponent<ImageAnimator>().MoveTo(pos);
  }

  [RPC]
  private IEnumerator NetworkTranslateDestroy(NetworkViewID id, Vector3 pos) {
    GameObject obj = NetworkView.Find(id).observed.gameObject;
    yield return StartCoroutine(obj.GetComponent<ImageAnimator>().SmoothMove(pos, ImageAnimator.moveTime));
    UnityEngine.Object.Destroy(obj);
  }

  [RPC]
  protected virtual void NetworkUpdateSprite() {
  }

#endregion 
#region Virtual Functions
  
  protected virtual GameObject SendSlot(int idx) {
    return NewDisplaySlot(group[idx]);
  }

  protected virtual void ReceiveSlot(GameObject obj) {
  }

#endregion 
}
