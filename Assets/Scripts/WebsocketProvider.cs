using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Audio;

public class WebsocketProvider : MonoBehaviour
{

    private WebSocket ws;

    public float sliderValuey;
    public float sliderValuex;
    public float sliderValued;
    public float sliderValuedo;
    public float sliderValuea;
    public float sliderValuemm;
    public float sliderValuemd;
    public float sliderValuemx;
    public float sliderValuemy;
    public bool goodeyeisright;
    public bool waveIsPendular;
    
    public GameObject pv;

    public GameObject MainCamera;
    private MembraneDistort mainCameraDistort;
    private Camera mc;
    public GameObject OffCamera;
    private MembraneDistort offCameraDistort;
    private Camera oc;

    // vars for periodically ping to keep alive
    private float time = 0.0f;
    public float pingPeriod = 30.0f;

    void Start()
    {
        mainCameraDistort = MainCamera.GetComponent<MembraneDistort>();
        offCameraDistort = OffCamera.GetComponent<MembraneDistort>();
        mc = MainCamera.GetComponent<Camera>();
        oc = OffCamera.GetComponent<Camera>();

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
                else if (sliderId == "slidermm")
                {
                    sliderValuemm = val;
                    Debug.Log("slider mm updated: " + sliderValuemm);
                }
                else if (sliderId == "slidermd")
                {
                    sliderValuemd = val;
                    Debug.Log("slider md updated: " + sliderValuemd);
                }
                else if (sliderId == "slidermx")
                {
                    sliderValuemx = val;
                    Debug.Log("slider mx updated: " + sliderValuemx);
                }
                else if (sliderId == "slidermy")
                {
                    sliderValuemy = val;
                    Debug.Log("slider my updated: " + sliderValuemy);
                }
                else if (sliderId == "buttonwave")
                {
                    if (val == 1)
                    {
                        waveIsPendular = true;
                    } else
                    {
                        waveIsPendular = false;
                    }
                    Debug.Log("wave type updated: " + waveIsPendular.ToString());
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
        /*
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
        */

        // we still ping regardless; we just do not check ws.IsAlive, as checking it is prohibitively slow
        if (ws != null)
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
        
        float intraOffset;
        float centerOffset;
        if (goodeyeisright)
        {
            mc.stereoTargetEye = StereoTargetEyeMask.Right;
            oc.stereoTargetEye = StereoTargetEyeMask.Left;
            intraOffset = 0.04f;
            centerOffset = 0.01f;
        }
        else
        {
            mc.stereoTargetEye = StereoTargetEyeMask.Left;
            oc.stereoTargetEye = StereoTargetEyeMask.Right;
            intraOffset = -0.04f;
            centerOffset = -0.01f;
        }

        pv.transform.localPosition = new Vector3(centerOffset, -0.0075f, (sliderValuea - 50) * 0.001f + 0.15f);

        // membrane distort
        offCameraDistort.center = new Vector2((sliderValuemx + 100) * 0.005f, (sliderValuemy + 100) * 0.005f);
        offCameraDistort.radius = sliderValuemd * 0.01f;
        offCameraDistort.magnitude = (sliderValuemm + 100) * 0.01f;

        mainCameraDistort.magnitude = 1f;

        //nystagmus
        float yOffset = 0;
        float xOffset = 0;
        float rotOffset = 0;
        
        if (waveIsPendular)
        {
            yOffset = Mathf.Sin(Time.time * sliderValuemd) * sliderValuemy * 0.0005f;
            xOffset = Mathf.Sin(Time.time * sliderValuemd) * sliderValuemx * 0.0005f;
            rotOffset = Mathf.Sin(Time.time * sliderValuemd) * sliderValuemm * 0.5f;
        } else
        {
            yOffset = ((Time.time * sliderValuemd) % 1) * sliderValuemy * 0.0005f;
            xOffset = ((Time.time * sliderValuemd) % 1) * sliderValuemx * 0.0005f;
            rotOffset = ((Time.time * sliderValuemd) % 1) * sliderValuemm * 0.5f;
        }
        

        MainCamera.transform.localPosition = new Vector3(xOffset, yOffset, 0);
        MainCamera.transform.localEulerAngles = new Vector3(0, 0, rotOffset);

        OffCamera.transform.localPosition = new Vector3(intraOffset-dioptersToDist(sliderValuex)+xOffset, -dioptersToDist(sliderValuey)+yOffset, ((sliderValuea - 50) * 0.002f));
        OffCamera.transform.localEulerAngles = new Vector3(0, 0, -sliderValued+rotOffset);
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
