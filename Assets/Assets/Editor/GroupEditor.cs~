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
    GUIStyle style = new GUIStyle();
    style.normal.textColor = Color.black;
    style.wordWrap = true;
    style.richText = true;
    
    if (g.group == null || g.group.Count <= 0) 
        EditorGUILayout.LabelField("No Cards");
    else {
      showCards = EditorGUILayout.Foldout(showCards, g.group.Count.ToString() + " Cards");
      if (showCards) {
        foreach (Card c in g.group) {
          EditorGUILayout.LabelField(c.RichText4(), style);
        }
      }
    }
  }
}
