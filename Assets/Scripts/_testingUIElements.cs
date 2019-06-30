using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _testingUIElements : MonoBehaviour
{
    //[Range(0, 1)]
    private float _outlinePercentage;

    public Transform statusLogBackgroundOutline;
    public Transform messageLogBackgroundOutline;

    //private string _guiText;
    //public Text gt;

    //public static MessageLog MessageLog { get; private set; }

    void Start()
    {
        // UI styles
        CreateOutline();

        // Message logs
        //MessageLog messageLog = Engine.MessageLog; // We assign the static var to a local var                                                  //_guiText = messageLog.OutputLogs();
        //MessageLog messageLog = new MessageLog();
        //messageLog.Add("This is another message to test the queue...");
        //messageLog.OutputLogs();
        //_guiText = messageLog._foo;
        //messageLog._foo = gt.ToString();

    }

    void CreateOutline() {

        _outlinePercentage = 0.02f;
        statusLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
        messageLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
    }

}
