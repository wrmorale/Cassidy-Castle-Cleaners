using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.

    // Aggro: Keep enemies aggro after the player leaves detection range
    [System.Serializable]
    public class Blackboard {

        public Vector3 moveToPosition;
        public bool aggro = false;
        public float specialCooldownEnds = 0.0f; //Cooldown used for special abilities, like Golem's Dash
    }
}