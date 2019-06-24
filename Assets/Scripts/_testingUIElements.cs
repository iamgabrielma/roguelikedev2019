using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _testingUIElements : MonoBehaviour
{
    //[Range(0, 1)]
    private float _outlinePercentage;

    public Transform statusLogBackgroundOutline;
    public Transform messageLogBackgroundOutline;

    void Start()
    {
        CreateOutline();
    }

    void CreateOutline() {

        _outlinePercentage = 0.02f;
        statusLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
        messageLogBackgroundOutline.localScale = Vector3.one * (1 - _outlinePercentage);
    }

}
