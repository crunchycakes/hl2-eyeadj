using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;

public class WebsocketProvider : MonoBehaviour
{

    private WebSocket ws;

    public float sliderValue;

    void Start()
    {
        string serverUrl = "wss://hl2-torsion-71297cc1c5dc.herokuapp.com/ws";

        ws = new WebSocket(serverUrl);
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        ws.SslConfiguration.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

        // Called when connected
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("!!!!! connected to websocket");
            ws.Send("unity hello");
        };

        // Called when receiving messages
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("!!!!! websocket received: " + e.Data);

            JObject msg = JObject.Parse(e.Data);
            if ((string)msg["type"] == "slider")
            {
                sliderValue = (float)msg["value"];
                Debug.Log("slider value recieved: " + sliderValue);
            }
        };

        // Called on errors
        ws.OnError += (sender, e) =>
        {
            Debug.LogError("!!!!! websocket error: " + e.Message);
        };

        // Called when closed
        ws.OnClose += (sender, e) =>
        {
            Debug.LogWarning("!!!!! websocket closed: " + e.Reason);
        };

        ws.ConnectAsync();  // Non-blocking connect
    }

    //void Update()
    //{
    //    // Optional: Ping server every 30s to keep alive (Heroku idle timeout)
    //    if (ws != null && ws.IsAlive)
    //    {
    //        // Keep-alive logic if needed
    //    }
    //}

    void OnApplicationQuit()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
