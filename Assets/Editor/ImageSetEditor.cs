using UnityEngine;
using UnityEditor;
using System.Collections;

public class ImageSetEditor : EditorWindow {
  [SerializeField]
  GameObject CardFrame;

  [MenuItem("Window/ImageSetEditor")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(ImageSetEditor));
  }

  void OnEnable() {
    ImageSet.CardFrame = CardFrame;
  }

  void OnGUI() {
    if (CardSet.initialized) {
      CardFrame = (GameObject) EditorGUILayout.ObjectField("CardFrame", CardFrame, typeof(GameObject), false);
      ImageSet.CardFrame = CardFrame;
    }
  }
}
