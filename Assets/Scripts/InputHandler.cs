using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private bool isPlayerMoving; // This avoids that there's multiple movement until the previous movement hasn't completed

    GameObject player; // Self reference, in order to use MovePlayer() easily

    private void Start()
    {
        // TODO: add nullcheck. We assign player to the static __player instance, shouldn't be null but check for safety.
        player = Engine.__player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && isPlayerMoving == false)
        {
            isPlayerMoving = true;
            MovePlayer("up");
        }
        else if (Input.GetKeyDown(KeyCode.S) && isPlayerMoving == false)
        {
            isPlayerMoving = true;
            MovePlayer("down");
        }
        else if (Input.GetKeyDown(KeyCode.D) && isPlayerMoving == false)
        {
            isPlayerMoving = true;
            MovePlayer("right");
        }
        else if (Input.GetKeyDown(KeyCode.A) && isPlayerMoving == false)
        {
            isPlayerMoving = true;
            MovePlayer("left");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // TODO: Will implement game menu screen here in the future.
        }

    }

    public void MovePlayer(string direction)
    {
        switch (direction)
        {
            case "up":
                float yPositive = player.transform.localPosition.y + 1.0f;
                player.transform.localPosition = new Vector3(player.transform.localPosition.x, yPositive, 0);
                break;
            case "down":
                float yNegative = player.transform.localPosition.y - 1.0f;
                player.transform.localPosition = new Vector3(player.transform.localPosition.x, yNegative, 0);
                break;
            case "right":
                float xPositive = player.transform.localPosition.x + 1.0f;
                player.transform.localPosition = new Vector3(xPositive, player.transform.localPosition.y, 0);
                break;
            case "left":
                float xNegative = player.transform.localPosition.x - 1.0f;
                player.transform.localPosition = new Vector3(xNegative, player.transform.localPosition.y, 0);
                break;

        }
        isPlayerMoving = false;
    }
}
