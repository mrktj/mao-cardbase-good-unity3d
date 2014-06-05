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

  private static string[] _names;
  public static string[] names { get { return _names; }}
  private static string[] _choiceNames;
  public static string[] choiceNames { get { return _choiceNames; }}
  
  [SerializeField]
  private static bool _initialized = false; // Whether the set has been loaded
  public static bool initialized { get { return _initialized; }}

  public static int[] classCards = new int[] {7, 9, 11};
  public static int[] classTokens = new int[] {6, 8, 10};

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
        EffectType type = ParseEnum<EffectType>(xmlEffects[i].SelectSingleNode("Type").InnerText);
        EffectData data = new EffectData(Int32.Parse(xmlEffects[i].SelectSingleNode("Value").InnerText), 
                                         Boolean.Parse(xmlEffects[i].SelectSingleNode("Opponent").InnerText));
        effects[i] = new CardEffect(data, type);
      }
      _cards.Add(new Card(name, buyCost, useCost, cards.Count, effects));
    }
    _initialized = true;

    _choiceNames = new string[cards.Count + 2];
    _choiceNames[0] = "Hand";
    _choiceNames[1] = "This";
    _names = new string[cards.Count];
    for (int i = 0; i < cards.Count; i ++) {
      _names[i] = cards[i].name;
      _choiceNames[i + 2] = cards[i].name;
    }
  }

  /**
   * Get the Card with cardValue IDX 
   */
  public static Card GetCard(int idx) {
    if (!initialized || idx < 0) {
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
