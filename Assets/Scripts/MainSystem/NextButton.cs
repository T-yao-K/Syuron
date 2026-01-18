using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class NextButton : UdonSharpBehaviour
{
    [Tooltip("GameManagerオブジェクトをドラッグ&ドロップで割り当てる")]
    public UdonSharpBehaviour gameManager; // UdonSharpBehaviour型で参照

    public override void Interact()
    {
        if (gameManager != null)
        {
            // SendCustomEventで他のUdonSharpスクリプトのメソッドを呼び出す
            gameManager.SendCustomEvent("GoToNextPhase");
        }
    }
}