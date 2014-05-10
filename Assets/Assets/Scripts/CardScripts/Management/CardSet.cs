using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

/**
 * A set of Cards to be used and referenced by other classes
 */
public static class CardSet {
#region Private Variables and Accessors

  [SerializeField]
  private static List<Card> _cards; // A list of Cards
  public static List<Card> cards { get { return _cards; }}
  
  private static bool _initialized = false; // Whether the set has been loaded
  public static bool initialized { get { return _initialized; }}

#endregion
#region Static Methods

  /**
   * Load Cards in from the file with name FILENAME
   */
  public static void loadCards(string filename) {
    _cards = new List<Card>();
    TextAsset file = (TextAsset) Resources.Load("CardSets/" + filename, typeof(TextAsset));
    if (file == null) {
      Resources.UnloadAsset(file);
      _initialized = false;
      return;
    }
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(file.text);

    
    XmlNodeList xmlCards = xmlDoc.SelectNodes("CardSet/Card");
    foreach (XmlNode c in xmlCards) {
      string name = c.SelectSingleNode("Name").InnerText; 
      int buyCost = Int32.Parse(c.SelectSingleNode("BuyCost").InnerText);
      int useCost = Int32.Parse(c.SelectSingleNode("UseCost").InnerText);
      XmlNodeList xmlEffects = c.SelectSingleNode("Effects").ChildNodes;
      CardEffect[] effects = new CardEffect[xmlEffects.Count];
      for (int i = 0; i < xmlEffects.Count; i++) {
        int val = Int32.Parse(xmlEffects[i].SelectSingleNode("Value").InnerText);
        EffectType type = ParseEnum<EffectType>(xmlEffects[i].SelectSingleNode("Type").InnerText);
        effects[i] = new CardEffect(val, type);
      }
      _cards.Add(new Card(name, buyCost, useCost, cards.Count, effects));
    }
    _initialized = true;
  }

  /**
   * Get the Card with cardValue IDX 
   */
  public static Card GetCard(int idx) {
    if (!initialized) {
      return null;
    }
    return cards[idx];
  }
  
  /**
   * A function to parse a string into an enum
   */
  private static T ParseEnum<T>( string value )
  {
      return (T) Enum.Parse( typeof( T ), value, true );
  }

#endregion 
}
