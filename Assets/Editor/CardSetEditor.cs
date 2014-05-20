using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

/**
 * Editor to edit the CardSet files in a Unity Editor Window
 */
public class CardSetEditor : EditorWindow {
  [SerializeField]
  string filename = "BasicSet";
  [SerializeField]
  List<bool> folds = new List<bool>();        // List of fold states
  [SerializeField]
  List<bool> effectfolds = new List<bool>();  // List of effect fold states
  [SerializeField]
  List<string> names = new List<string>();    // List of names
  [SerializeField]
  List<int> buy = new List<int>();            // List of buy costs
  [SerializeField]
  List<int> use = new List<int>();            // List of use costs
  [SerializeField]
  Vector2 scroll = new Vector2(0,0);          // Scroll State
  
  // List of Effects
  [SerializeField]
  List<List<CardEffect>> effects = new List<List<CardEffect>>(); 

  [MenuItem("Window/CardSetEditor")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(CardSetEditor));
  }

  /**
   * Load the CardSet in file filename
   */
  private void load() {
    CardSet.loadCards(filename);
    if (CardSet.initialized) {
      names.Clear();
      buy.Clear();
      use.Clear();
      folds.Clear();
      effectfolds.Clear();
      effects.Clear();
      foreach (Card c in CardSet.cards) {
        folds.Add(false);
        effectfolds.Add(true);
        names.Add(c.name);
        buy.Add(c.buyCost);
        use.Add(c.useCost);
        effects.Add(c.effects);
      }
    }
  }

  void OnGUI() {
    
    /* Allow user to input filename and load the CardSet */
    EditorGUILayout.LabelField("Card Set", EditorStyles.boldLabel);
    filename = EditorGUILayout.TextField("file", filename);
    if (GUILayout.Button("Read")) {
      load();
    }

    /* If CardSet has been loaded */
    if (CardSet.initialized) {
      scroll = EditorGUILayout.BeginScrollView(scroll);

      // Button GUIStyle
      GUIStyle style = new GUIStyle(GUI.skin.button);
      style.alignment = TextAnchor.MiddleLeft;
      style.fixedWidth = 15;
      style.padding = new RectOffset(3, 0, 0, 2);

      // Add buttons for adding and removing Cards
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("+", style)) {
        names.Add("New Card"); buy.Add(0); use.Add(0); folds.Add(false);
        effectfolds.Add(true); effects.Add(new List<CardEffect>());
      }
      style.padding = new RectOffset(5, 0, 0, 2);
      if (GUILayout.Button("-", style) && names.Count > 0) {
        names.RemoveAt(names.Count - 1);
        buy.RemoveAt(buy.Count - 1);
        use.RemoveAt(use.Count - 1);
        folds.RemoveAt(folds.Count - 1);
        effects.RemoveAt(effects.Count - 1);
        effectfolds.RemoveAt(effectfolds.Count - 1);
      }
      GUILayout.EndHorizontal();

      // Display all Cards and their information
      for (int i = 0; i < names.Count; i++) {
        folds[i] = EditorGUILayout.Foldout(folds[i], names[i] + "(" + buy[i] + "/" + use[i] + ")");
        if (folds[i]) {
          EditorGUI.indentLevel++;
          names[i] = EditorGUILayout.TextField("Name", names[i]);
          buy[i] = EditorGUILayout.IntField("Buy Cost", buy[i]);
          use[i] = EditorGUILayout.IntField("Use Cost", use[i]);
          // Display Card Text
          string fulltext = "";
          if (i < CardSet.cards.Count) fulltext = CardSet.GetCard(i).fullText;
          EditorGUILayout.LabelField(fulltext, GUILayout.Height(fulltext.Split('\n').Length * 16.0f));

          // Display buttons for adding and removing effects
          GUILayout.BeginHorizontal();
          effectfolds[i] = EditorGUILayout.Foldout(effectfolds[i], 
              "Effects (" + effects[i].Count + ")");
          style.padding = new RectOffset(3, 0, 0, 2);
          if (GUILayout.Button("+", style)) {
            effects[i].Add(new CardEffect(0, EffectType.ENERGY));
          }
          style.padding = new RectOffset(5, 0, 0, 2);
          if (GUILayout.Button("-", style) && effects[i].Count > 0) {
            effects[i].RemoveAt(effects[i].Count - 1);
          }
          GUILayout.EndHorizontal();

          // Display all effects on the Card and allow changing their properties
          if (effectfolds[i]) {
            EditorGUI.indentLevel++;

            // Some Custom GUIStyles for formatting
            GUIStyle numstyle = new GUIStyle(EditorStyles.numberField);
            numstyle.fixedWidth = 25;
            GUIStyle popstyle = new GUIStyle(EditorStyles.popup);
            popstyle.fixedWidth = 75;

            for (int j = 0; j < effects[i].Count; j++) {
              GUILayout.BeginHorizontal();
              Rect off = new Rect(EditorGUILayout.GetControlRect());
              off.width = 110;
              effects[i][j].type = (EffectType) 
                EditorGUI.EnumPopup(off, effects[i][j].type, popstyle);
              if (effects[i][j].generalType == GeneralType.BASIC) {
                off.x += 80;
                effects[i][j].data.num = 
                  EditorGUI.IntField(off, effects[i][j].data.num, numstyle);
              }
              else if (effects[i][j].generalType == GeneralType.CARDMOD) {
                off.x += 80;
                effects[i][j].data.cardValue = 
                  EditorGUI.IntPopup(off, effects[i][j].data.num, 
                      CardSet.choiceNames, 
                      Enumerable.Range(-1, CardSet.cards.Count + 1).ToArray());
              }
              GUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
          }
          EditorGUI.indentLevel--;
        }
      }
      EditorGUILayout.EndScrollView();
    }

    EditorGUILayout.Space();

    // Apply the changes by writing them back to the file
    if (GUILayout.Button("Apply")) {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.IndentChars = ("\t");
      settings.OmitXmlDeclaration = true;
      XmlWriter writer = XmlWriter.Create("Assets/Resources/CardSets/"
          + filename + ".txt", settings);
      writer.WriteStartDocument();
      writer.WriteStartElement("CardSet");

      for (int i = 0; i < names.Count; i++) {
        writer.WriteStartElement("Card");

        writer.WriteElementString("Name", names[i]);
        writer.WriteElementString("BuyCost", buy[i].ToString());
        writer.WriteElementString("UseCost", use[i].ToString());
        writer.WriteStartElement("Effects");
        for (int j = 0; j < effects[i].Count; j++) {
          writer.WriteStartElement("Effect");
          writer.WriteElementString("Type", effects[i][j].type.ToString());
          writer.WriteElementString("Value", effects[i][j].data.num.ToString());
          writer.WriteEndElement();
        }
        writer.WriteEndElement();

        writer.WriteEndElement();
      }

      writer.WriteEndElement();
      writer.WriteEndDocument();
      writer.Close();

      AssetDatabase.Refresh();
    }
  }
}
