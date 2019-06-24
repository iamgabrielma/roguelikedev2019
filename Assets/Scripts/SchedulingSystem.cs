using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchedulingSystem
{
    private int _time;
    private readonly SortedDictionary<int, List<IScheduleable>> _scheduleables;
    // https://roguesharp.wordpress.com/2016/08/27/roguesharp-v3-tutorial-scheduling-system/
    // https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.sorteddictionary-2?redirectedfrom=MSDN&view=netframework-4.8
    /*
    - Represents a collection of key/value pairs that are sorted on the key
    - "Key" in the dictionary is an integer that represents the time.
    - The "Value" of each dictionary entry is a List<IScheduleable>
    - IScheduleable: custom interface with a single property "Time". we can put anything on the schedule as long as it has a Time which represents how many turns pass until its time comes up again on the schedule.  
    - we will give each Actor a Speed statistic which will represent how often they can take actions. 
    - our scheduling system is that it will keep track of the time or number of turns that have passed as well as the current time.
    - The Speed of the actor will determine when they get inserted into the timeline. 
    - having a lower speed is better.
    - why do we have a List<> of them for each key in the dictionary? more than one Actor can act on the same time interval  
    */

    // constructor?
    public SchedulingSystem()
    {
        _time = 0;
        _scheduleables = new SortedDictionary<int, List<IScheduleable>>();
    }

    // Adds to the schedule
    public void Add( IScheduleable scheduleable ) {

        int key = _time + scheduleable.Time;
        Debug.Log(scheduleable + "added to the schedule | key: " + key);
    }

    // Removes from the schedule
    public void Remove( IScheduleable scheduleable )
    {

    }

    // Get the current time (turn) for the schedule
    public int GetTime() {

        return _time;
    }

    // Reset the time (turn) and clear the schedule
    public void ClearSchedule() {

        _time = 0;
        _scheduleables.Clear();
    }


}
