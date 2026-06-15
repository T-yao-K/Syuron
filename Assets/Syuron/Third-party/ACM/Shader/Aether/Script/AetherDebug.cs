using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class AetherDebug : UdonSharpBehaviour
{
    [SerializeField] TextMeshPro _text;

    void Update()
    {
        System.DateTime dt = Networking.GetNetworkDateTime();
        _text.text = dt.Hour.ToString("00") + ":" + dt.Minute.ToString("00") + " UTC";
    }
}
