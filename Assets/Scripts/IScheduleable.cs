using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScheduleable 
{
    // An interface in C# contains only the declaration of the methods, properties, and events, but not the implementation. It is left to the class that implements the interface by providing implementation for all the members of the interface. 
    int Time { get; }
}
