using devlog98.Block;
using System.Collections.Generic;
using UnityEngine;

/*
 * Key composition that unlocks specific Locks
 * Dependent on Player Blocks composition
 */

namespace devlog98.Level {
    public class Key : MonoBehaviour {
        private List<PlayerBlock> keyBlocks = new List<PlayerBlock>();

        private void Start() {
            keyBlocks.AddRange(GetComponentsInChildren<PlayerBlock>());
            foreach (PlayerBlock keyBlock in keyBlocks) {
                keyBlock.CheckBlockNeighbours();
            }
        }

        // check Player Blocks composition for specific Lock
        public bool CheckAgainstKeyBlocks(List<PlayerBlock> blocks) {
            bool check = true;

            // false if Player has different amount of blocks
            if (blocks.Count != keyBlocks.Count) {
                check = false;
                return check;
            }
            
            // false if a key block is not detecting another block
            foreach (PlayerBlock keyBlock in keyBlocks) {
                if (!keyBlock.CheckCollisionWithBlock()) {
                    check = false;
                    return check;
                }
            }

            return check;
        }
    }
}