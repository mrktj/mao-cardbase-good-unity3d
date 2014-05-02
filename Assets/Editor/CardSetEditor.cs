using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class CardSetEditor : EditorWindow {
  [SerializeField]
  string filename = "xmlTest";
  [SerializeField]
  List<bool> folds = new List<bool>();
  [SerializeField]
  List<bool> effectfolds = new List<bool>();
  [SerializeField]
  List<string> names = new List<string>();
  [SerializeField]
  List<string> text = new List<string>();
  [SerializeField]
  List<int> buy = new List<int>();
  [SerializeField]
  List<int> use = new List<int>();
  [SerializeField]
  List<List<CardEffect>> effects = new List<List<CardEffect>>();
  [SerializeField]
  Vector2 scroll = new Vector2(0,0);

  [MenuItem("Window/CardSetEditor")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(CardSetEditor));
  }

  void OnEnable() {
    load();
  }

  private void load() {
    CardSet.loadCards(filename);
    if (CardSet.initialized) {
      names.Clear();
      text.Clear();
      buy.Clear();
      use.Clear();
      folds.Clear();
      effectfolds.Clear();
      effects.Clear();
      foreach (Card c in CardSet.cards) {
        folds.Add(false);
        effectfolds.Add(true);
        names.Add(c.name);
        text.Add(c.text);
        buy.Add(c.buyCost);
        use.Add(c.useCost);
        effects.Add(c.effects);
      }
    }
  }

  void OnGUI() {
    
    EditorGUILayout.LabelField("Card Set", EditorStyles.boldLabel);
    filename = EditorGUILayout.TextField("file", filename);
    if (GUILayout.Button("Read")) {
      load();
    }

    if (CardSet.initialized) {
      scroll = EditorGUILayout.BeginScrollView(scroll);
      GUIStyle style = new GUIStyle(GUI.skin.button);
      style.alignment = TextAnchor.MiddleLeft;
      style.fixedWidth = 15;
      style.padding = new RectOffset(3, 0, 0, 2);
      GUILayout.BeginHorizontal();
      if (GUILayout.Button("+", style)) {
        names.Add("New Card"); text.Add(""); buy.Add(0); use.Add(0); folds.Add(false);
        effectfolds.Add(true); effects.Add(new List<CardEffect>());
      }
      style.padding = new RectOffset(5, 0, 0, 2);
      if (GUILayout.Button("-", style) && names.Count > 0) {
        names.RemoveAt(names.Count - 1);
        text.RemoveAt(text.Count - 1);
        buy.RemoveAt(buy.Count - 1);
        use.RemoveAt(use.Count - 1);
        folds.RemoveAt(folds.Count - 1);
        effects.RemoveAt(effects.Count - 1);
        effectfolds.RemoveAt(effectfolds.Count - 1);
      }
      GUILayout.EndHorizontal();
      for (int i = 0; i < names.Count; i++) {
        folds[i] = EditorGUILayout.Foldout(folds[i], names[i] + "(" + buy[i] + "/" + use[i] + ")");
        if (folds[i]) {
          EditorGUI.indentLevel++;
          names[i] = EditorGUILayout.TextField("Name", names[i]);
          text[i] = EditorGUILayout.TextField("Text", text[i]);
          buy[i] = EditorGUILayout.IntField("Buy Cost", buy[i]);
          use[i] = EditorGUILayout.IntField("Use Cost", use[i]);

          GUILayout.BeginHorizontal();
          effectfolds[i] = EditorGUILayout.Foldout(effectfolds[i], "Effects (" + effects[i].Count + ")");
          style.padding = new RectOffset(3, 0, 0, 2);
          if (GUILayout.Button("+", style)) {
            effects[i].Add(new CardEffect(0, EffectType.ENERGY));
          }
          style.padding = new RectOffset(5, 0, 0, 2);
          if (GUILayout.Button("-", style) && effects[i].Count > 0) {
            effects[i].RemoveAt(effects[i].Count - 1);
          }
          GUILayout.EndHorizontal();
          if (effectfolds[i]) {
            EditorGUI.indentLevel++;
            GUIStyle numstyle = new GUIStyle(EditorStyles.numberField);
            numstyle.fixedWidth = 25;
            GUIStyle popstyle = new GUIStyle(EditorStyles.popup);
            popstyle.fixedWidth = 75;
            for (int j = 0; j < effects[i].Count; j++) {
              GUILayout.BeginHorizontal();
              Rect off = new Rect(EditorGUILayout.GetControlRect());
              EditorGUI.LabelField(off, "+");
              off.x += 45;
              effects[i][j].type = (EffectType) EditorGUI.EnumPopup(off, effects[i][j].type, popstyle);
              off.x -= 30;
              effects[i][j].val = EditorGUI.IntField(off, effects[i][j].val, numstyle);
              off.x -= 15;
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
    if (GUILayout.Button("Apply")) {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;
      settings.IndentChars = ("\t");
      settings.OmitXmlDeclaration = true;
      XmlWriter writer = XmlWriter.Create("Assets/Resources/CardSets/" + filename + ".txt", settings);
      writer.WriteStartDocument();
      writer.WriteStartElement("CardSet");

      for (int i = 0; i < names.Count; i++) {
        writer.WriteStartElement("Card");

        writer.WriteElementString("Name", names[i]);
        writer.WriteElementString("Text", text[i]);
        writer.WriteElementString("BuyCost", buy[i].ToString());
        writer.WriteElementString("UseCost", use[i].ToString());
        writer.WriteStartElement("Effects");
        for (int j = 0; j < effects[i].Count; j++) {
          writer.WriteStartElement("Effect");
          writer.WriteElementString("Value", effects[i][j].val.ToString());
          writer.WriteElementString("Type", effects[i][j].type.ToString());
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
