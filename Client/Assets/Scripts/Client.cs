using System.Net;
using UnityEngine;

namespace StrategicGame.Client {
    public class Client : MonoBehaviour {
        RemoteServer _remoteServer;

        void Start() {    
            var endPoint = new IPEndPoint(IPAddress.Loopback, 4040);
            _remoteServer = new RemoteServer(endPoint);
        }
    }
}
