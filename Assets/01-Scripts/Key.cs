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

            List<PlayerBlock> checkBlocks = blocks;

            // false if Player has different amount of blocks
            if (checkBlocks.Count != keyBlocks.Count) {
                check = false;
                return check;
            }
            
            foreach (PlayerBlock keyBlock in keyBlocks) {
                // find block with same properties
                PlayerBlock block = checkBlocks.Find(x =>
                    (x.RightBlock == null) == (keyBlock.RightBlock == null) &&
                    (x.LeftBlock == null) == (keyBlock.LeftBlock == null) &&
                    (x.UpBlock == null) == (keyBlock.UpBlock == null) &&
                    (x.DownBlock == null) == (keyBlock.DownBlock == null)
                );

                // remove block from original list
                if (block != null) {
                    checkBlocks.Remove(block);
                }
            }

            // if there are blocks left, Player composition is wrong
            if (checkBlocks.Count > 0) {
                check = false;
            }

            return check;
        }
    }
}