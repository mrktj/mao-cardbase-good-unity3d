using UnityEngine;
using System.Collections;
 
public class NetworkingConnector : MonoBehaviour {
  public string connectionIP = "127.0.0.1";
  public int connectionPort = 25001;

  void OnGUI() {
    if (Network.peerType == NetworkPeerType.Disconnected) {
      GUI.Label(new Rect(10, 10, 300, 20), "Status: Disconnected");
      connectionIP = GUI.TextField(new Rect(130, 30, 240, 20), connectionIP);
      if (GUI.Button(new Rect(10, 30, 120, 20), "Client Connect")) {
        Network.Connect(connectionIP, connectionPort);
      }
      if (GUI.Button(new Rect(10, 50, 120, 20), "Initialize Server")) {
        Network.InitializeServer(32, connectionPort, false);
      }
    }
    else if (Network.isClient) {
      GUI.Label(new Rect(10, 10, 300, 20), "Status: Client");
      if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect")) {
        Network.Disconnect(200);
      }
    }
    else if (Network.isServer) {
      GUI.Label(new Rect(10, 10, 300, 20), "Status: Server - " + Network.connections.Length + " connected");
      if (GUI.Button(new Rect(10, 30, 120, 20), "Disconnect")) {
          Network.Disconnect(200);
      }
    }
  }
}
