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
    public float sliderValuedo;
    public float sliderValuea;
    public bool goodeyeisright;
    
    public GameObject pv;

    public GameObject MainCamera;
    public GameObject OffCamera;

    // vars for periodically ping to keep alive
    private float time = 0.0f;
    public float pingPeriod = 30.0f;

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
                else if (sliderId == "sliderdo")
                {
                    sliderValuedo = val;
                    Debug.Log("slider do updated: " + sliderValuedo);
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
            else if ((string)msg["type"] == "goodeyeisright")
            {
                goodeyeisright = (bool)msg["value"];
                Debug.Log("goodeyeisright changed: " + goodeyeisright);
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

    void Update()
    {
        // Optional: Ping server every 30s to keep alive (Heroku idle timeout)
        if (ws != null && ws.IsAlive)
        {
            time += Time.deltaTime;
            if (time >= pingPeriod)
            {
                time = 0.0f;
                ws.Send("unity keepalive");
                Debug.Log("pinged to keep alive");
            }
        }

        // IMPROVE THESE MAGIC NUMBERS!!
        //pv.transform.localPosition = new Vector3((sliderValuex - 50) * 0.0020f, (sliderValuey - 50) * 0.00075f, (sliderValuea - 50) * 0.001f + 0.15f);
        pv.transform.localPosition = new Vector3(0, 0, (sliderValuea - 50) * 0.001f + 0.15f);
        if (goodeyeisright)
        {
            MainCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;
            OffCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
        }
        else
        {
            MainCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Left;
            OffCamera.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.Right;
        }

        OffCamera.transform.localPosition = new Vector3(-dioptersToDist(sliderValuex), -dioptersToDist(sliderValuey), 0);
        OffCamera.transform.eulerAngles = new Vector3(0, 0, sliderValued-90);
        //pv.transform.eulerAngles = new Vector3(sliderValued-180, 90, -90);
    }

    private float dioptersToDist(float diopters)
    {
        // TODO: FIX MAGIC NUMBER!!!
        return (diopters / 100f) * 0.15f;
    }

    void OnApplicationQuit()
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }
}
