using UnityEngine;
using System.Collections;

/**
 * A location where an image of a Card should be displayed
 */
public class DisplaySlot : MonoBehaviour {
#region Private Variables
  
  private int _cardValue; // The cardvalue of the displayed Card

#endregion
#region Public Variables

  public GameObject image; // The image of the Card

#endregion
#region Accessors

  public int cardValue { get { return _cardValue;} }

#endregion
#region Public Methods
  
  /* Draw the Card with cardvalue VAL */
  public void DrawCard(int val) {
    _cardValue = val;
    Destroy(image);
    image = ImageSet.GetImage(val, this.gameObject);
  }

  /* Draw the back of a Card */
  public void DrawBack() {
    Destroy(image);
    image = ImageSet.GetBack(this.gameObject);
  }

  /* Draw nothing */
  public void DrawEmpty() {
    Destroy(image);
  }

  /* Draw the outline of a Card */
  public void DrawBlank() {
    Destroy(image);
    image = ImageSet.GetBlank(this.gameObject);
  }

#endregion
}
