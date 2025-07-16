using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System;

public class WebsocketProvider : MonoBehaviour
{

    private WebSocket ws;

    public float sliderValuey;
    public float sliderValuex;
    public float sliderValued;
    public float sliderValuea;
    public GameObject pv;

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
                string sliderId = (string)msg["id"];
                float val = (float)msg["value"];

                if (sliderId == "sliderx")
                {
                    sliderValuex = val;
                    Debug.Log("slider x updated: " + sliderValuex);
                }
                else if (sliderId == "slidery")
                {
                    sliderValuey = val;
                    Debug.Log("slider y updated: " + sliderValuey);
                }
                else if (sliderId == "sliderd")
                {
                    sliderValued = val;
                    Debug.Log("slider d updated: " + sliderValued);
                }
                else if (sliderId == "slidera")
                {
                    sliderValuea = val;
                    Debug.Log("slider a updated: " + sliderValuea);
                }

                // GET RID OF THESE MAGIC NUMS!!!
                //try
                //{
                //    pv.transform.localPosition = new Vector3((sliderValuex - 50) * 0.0025f, (sliderValuey - 50) * 0.0005f, 0.15f);
                //}
                //catch (Exception ere)
                //{
                //    Debug.LogError(ere.Message);
                //}
                //Debug.Log("Transform success!?");
            }
        };

        // Called on errors
        ws.OnError += (sender, e) =>
        {
            Debug.LogError("!!!!! websocket error: " + e.Message);
            Debug.Log(sliderValuex);
            Debug.Log(sliderValuey);
            Debug.Log(sliderValued);
        };

        // Called when closed
        ws.OnClose += (sender, e) =>
        {
            Debug.LogWarning("!!!!! websocket closed: " + e.Reason);
        };

        ws.ConnectAsync();  // Non-blocking connect
    }

    void Update()
    {
        // Optional: Ping server every 30s to keep alive (Heroku idle timeout)
        //    if (ws != null && ws.IsAlive)
        //    {
        //        // Keep-alive logic if needed
        //    }

        // IMPROVE THESE MAGIC NUMBERS!!
        pv.transform.localPosition = new Vector3((sliderValuex - 50) * 0.0020f, (sliderValuey - 50) * 0.00075f, (sliderValuea - 50) * 0.001f + 0.15f);
        pv.transform.eulerAngles = new Vector3(sliderValued-180, 90, -90);
    }

    void OnApplicationQuit()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
