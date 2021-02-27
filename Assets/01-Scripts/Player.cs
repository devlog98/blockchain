using devlog98.Block;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Object that Player will be able to control on the game
 */ 

namespace devlog98.Actor {
    public enum PlayerDirection { Right, Left, Up, Down } // all possible move directions

    public class Player : MonoBehaviour {
        private List<PlayerBlock> blocks = new List<PlayerBlock>();

        private void Start() {
            blocks.AddRange(GetComponentsInChildren<PlayerBlock>());
            foreach(PlayerBlock block in blocks) {
                Debug.Log("Executing start for... " + block.gameObject.name);
                block.ExecuteStart();
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}