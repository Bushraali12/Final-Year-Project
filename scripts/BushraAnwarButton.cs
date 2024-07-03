using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType { Play, Pause, Stop, Record }
public class BushraAnwarButton : MonoBehaviour
{
    public ButtonType buttonType;
    public BA_VideoManager videoManager;
    private void OnMouseDown()
    {
        switch (buttonType)
        {
            case ButtonType.Play:
                videoManager.OnPlay();
                break;
            case ButtonType.Pause:
                videoManager.OnPause();
                break;
            case ButtonType.Stop:
                videoManager.OnStop();
                break;
            case ButtonType.Record:

                break;
        }
    }
   
}
