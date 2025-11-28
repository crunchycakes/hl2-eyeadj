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
    public float sliderValuenxf;
    public float sliderValuenxm;
    public float sliderValuenxp;
    public float sliderValuenyf;
    public float sliderValuenym;
    public float sliderValuenyp;
    public float sliderValuentf;
    public float sliderValuentm;
    public float sliderValuentp;
    public bool goodeyeisright;
    public int HorizontalWaveType;
    public int VerticalWaveType;
    public int TorsionalWaveType;
    
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
        // TODO: much much cleaner with a dict
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
                else if (sliderId == "slidernxf")
                {
                    sliderValuenxf = val;
                    Debug.Log("slider nxf updated: " + sliderValuenxf);
                }
                else if (sliderId == "slidernxm")
                {
                    sliderValuenxm = val;
                    Debug.Log("slider nxm updated: " + sliderValuenxm);
                }
                else if (sliderId == "slidernxp")
                {
                    sliderValuenxp = val;
                    Debug.Log("slider nxp updated: " + sliderValuenxp);
                }
                else if (sliderId == "slidernyf")
                {
                    sliderValuenyf = val;
                    Debug.Log("slider nyf updated: " + sliderValuenyf);
                }
                else if (sliderId == "slidernym")
                {
                    sliderValuenym = val;
                    Debug.Log("slider nym updated: " + sliderValuenym);
                }
                else if (sliderId == "slidernyp")
                {
                    sliderValuenyp = val;
                    Debug.Log("slider nyp updated: " + sliderValuenyp);
                }
                else if (sliderId == "sliderntf")
                {
                    sliderValuentf = val;
                    Debug.Log("slider ntf updated: " + sliderValuentf);
                }
                else if (sliderId == "sliderntm")
                {
                    sliderValuentm = val;
                    Debug.Log("slider ntm updated: " + sliderValuentm);
                }
                else if (sliderId == "sliderntp")
                {
                    sliderValuentp = val;
                    Debug.Log("slider ntp updated: " + sliderValuentp);
                }
                else if (sliderId == "dropnxw")
                {
                    HorizontalWaveType = Mathf.RoundToInt(val);
                    Debug.Log("dropdown nxw updated: " + HorizontalWaveType);
                }
                else if (sliderId == "dropnyw")
                {
                    VerticalWaveType = Mathf.RoundToInt(val);
                    Debug.Log("dropdown nyw updated: " + VerticalWaveType);
                }
                else if (sliderId == "dropntw")
                {
                    TorsionalWaveType = Mathf.RoundToInt(val);
                    Debug.Log("dropdown ntw updated: " + TorsionalWaveType);
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
        
        /*
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
        */

        // TODO: again this is much in need of refactoring
        // 0 is pendular, 1 is jerk linear, 2 is jerk accel, 3 is jerk decel

        if (HorizontalWaveType == 0)
        {
            xOffset = Mathf.Sin(Time.time * ((sliderValuenxf/100f)*2f*Mathf.PI) + Mathf.Deg2Rad*sliderValuenxp) * dioptersToDist(sliderValuenxm);
        }
        else if (HorizontalWaveType == 1)
        {
            xOffset = (((Time.time * sliderValuenxf/100f) + sliderValuenxp/360f) % 1) * dioptersToDist(sliderValuenxm);
        }
        else if (HorizontalWaveType == 2)
        {
            xOffset = Mathf.Pow(((Time.time * sliderValuenxf/100f) + sliderValuenxp/360f) % 1, 3) * dioptersToDist(sliderValuenxm);
        }
        else if (HorizontalWaveType == 3)
        {
            xOffset = Mathf.Pow(((Time.time * sliderValuenxf/100f) + sliderValuenxp/360f) % 1, 1f/3f) * dioptersToDist(sliderValuenxm);
        }

        if (VerticalWaveType == 0)
        {
            yOffset = Mathf.Sin(Time.time * ((sliderValuenyf/100f)*2f*Mathf.PI) + Mathf.Deg2Rad*sliderValuenyp) * dioptersToDist(sliderValuenym);
        }
        else if (VerticalWaveType == 1)
        {
            yOffset = (((Time.time * sliderValuenyf/100f) + sliderValuenyp/360f) % 1) * dioptersToDist(sliderValuenym);
        }
        else if (VerticalWaveType == 2)
        {
            yOffset = Mathf.Pow(((Time.time * sliderValuenyf/100f) + sliderValuenyp/360f) % 1, 3) * dioptersToDist(sliderValuenym);
        }
        else if (VerticalWaveType == 3)
        {
            yOffset = Mathf.Pow(((Time.time * sliderValuenyf/100f) + sliderValuenyp/360f) % 1, 1f/3f) * dioptersToDist(sliderValuenym);
        }

        if (TorsionalWaveType == 0)
        {
            rotOffset = Mathf.Sin(Time.time * ((sliderValuentf/100f)*2f*Mathf.PI) + Mathf.Deg2Rad*sliderValuentp) * sliderValuentm;
        }
        else if (TorsionalWaveType == 1)
        {
            rotOffset = (((Time.time * sliderValuentf/100f) + sliderValuentp/360f) % 1) * sliderValuentm;
        }
        else if (TorsionalWaveType == 2)
        {
            rotOffset = Mathf.Pow(((Time.time * sliderValuentf/100f) + sliderValuentp/360f) % 1, 3) * sliderValuentm;
        }
        else if (TorsionalWaveType == 3)
        {
            rotOffset = Mathf.Pow(((Time.time * sliderValuentf/100f) + sliderValuentp/360f) % 1, 1f/3f) * sliderValuentm;
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
