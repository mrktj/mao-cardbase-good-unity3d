using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Group), true)]
public class GroupEditor : Editor {
  bool showCards = false;

  public override void OnInspectorGUI(){
    DrawDefaultInspector();
    Group g = (Group) target;

    g.numDefault = EditorGUILayout.IntSlider("Num Default", g.numDefault, 0, 20);
    if (CardSet.initialized && g.numDefault > 0) {

      string[] names = new string[CardSet.cards.Count];
      int[] options = new int[CardSet.cards.Count];
      for (int i = 0; i < CardSet.cards.Count; i++) {
        names[i] = CardSet.GetCard(i).name;
        options[i] = i;
      }
      
      g.defaultCard = EditorGUILayout.IntPopup("Default Card", g.defaultCard, names, options);
    }
    else {
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Default Card \t\t\t None");
      EditorGUILayout.EndHorizontal();
    }
    
    if (g.group == null || g.group.Count <= 0) 
        EditorGUILayout.LabelField("No Cards");
    else {
      showCards = EditorGUILayout.Foldout(showCards, g.group.Count.ToString() + " Cards");
      if (showCards) {
        foreach (int i in g.group) {
          EditorGUILayout.LabelField(CardSet.GetCard(i).ToString());
        }
      }
    }

    if(GUI.changed){ EditorUtility.SetDirty(g);}
  }
}
