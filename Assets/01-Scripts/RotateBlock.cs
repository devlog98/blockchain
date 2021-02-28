﻿using devlog98.Actor;
using UnityEngine;

namespace devlog98.Block {
    public class RotateBlock : MonoBehaviour {
        // check collision with Player Blocks
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Block") {
                PlayerBlock block = collision.gameObject.GetComponent<PlayerBlock>();
                Player.instance.SetPivotBlock(block);
                Destroy(this.gameObject);
            }
        }
    }
}