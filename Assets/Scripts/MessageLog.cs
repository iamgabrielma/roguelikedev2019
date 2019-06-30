using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Represents a queue of messages that can be added to
// the oldest message will be the first one removed from the history.

public class MessageLog: MonoBehaviour
{
    //private static readonly int __maxNumberOfMessagesInTheLog = 10;
    public string _foo;

    // _lines will keep track of the lines of text via a Queue
    private readonly Queue<string> _lines;

    private Text _textToDisplay;

    private void Start()
    {
        //_foo = this.gameObject.GetComponent<Text>().text; // This is getting "--/ Message Log /--" text
    }
    public MessageLog()
    {
        _lines = new Queue<string>();
    }

    public void Add(string message) {

        _lines.Enqueue(message);

        //if (_lines.Count > __maxNumberOfMessagesInTheLog)
        //{
        //    _lines.Dequeue(); // Removes the oldest one
        //}
    }

    public string OutputLogs()
    {

        _foo = "";
        foreach (string st in _lines)
        {
            _foo = _foo + st + "\n";
        }

        return _foo;
        //List<string> _foo = new List<string>();
        //_foo.Add(_lines.ToArray);

        //_foo = "";

        ////string[] lines = _lines.ToArray(); // Set the queue lines into an array
        //string[] lines = _lines.ToArray(); // Set the queue lines into an array

        //for (int i = 0; i < lines.Length; i++)
        //{
        //    _foo += lines[i];
        //    Debug.Log(i);
        //    Debug.Log(lines[i]);

        //}
        ////return lines;
        //DisplayMessage(lines); // at this point _foo is just strings, not array, try this way
    }

    public string[] DisplayMessage(string[] _foo) {

        return _foo;
    }


}
