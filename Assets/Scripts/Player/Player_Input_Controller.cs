using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Input_Controller : MonoBehaviour
{
    public Player_Input playerInputAction;

    private void Awake() {
        playerInputAction = new Player_Input();
    }
}
