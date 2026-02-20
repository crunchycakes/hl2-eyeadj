using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class PVAtEyeGaze : MonoBehaviour
{

    [SerializeField] private GameObject _PV;
    [SerializeField] private GameObject _eyePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // var inputSystem = MixedRealityToolkit.Instance.GetService<IMixedRealityInputSystem>();
        // var gazeProvider = inputSystem.GazeProvider;

        _PV.transform.position =
            CoreServices.InputSystem.EyeGazeProvider.GazeOrigin + 
            CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized * 0.15f;
        
        _PV.transform.LookAt(_eyePos.transform.position);
    }
}
