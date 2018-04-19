using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControllor : MonoBehaviour {
    
    public void Pause(int val) {
        Time.timeScale = val;
    }

}
