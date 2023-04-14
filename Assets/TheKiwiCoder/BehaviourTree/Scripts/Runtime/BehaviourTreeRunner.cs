using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
    public class BehaviourTreeRunner : MonoBehaviour {

        /*Can I call functions on the Golem script from the BT?
         - Need to be able to reference a script of an unknown type, and see if it has the listed function name
         - Tree can bind context. Can it also bind the behavior tree runner?
         

         - Not sure if I can just check if a script has a method, and even so it might be inefficient.
           The alternative would be to convert a bunch of enemy functions into new nodes, which could get
           very convoluted if there are too many unique actions for different enemies.
         */

        // The main behaviour tree asset
        public BehaviourTree tree;
        public MonoBehaviour mainEnemyScript;

        // Storage container object to hold game object subsystems
        Context context;

        // Start is called before the first frame update
        void Start() {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
            tree.BindTreeRunner(this); /*We're gonna make the behavior tree able to reference its assigned runner.*/
        }

        /*Custom*/
        void CallFuncOnMainScript(string function)
        {
            
        }

        // Update is called once per frame
        void Update() {
            if (tree) {
                tree.Update();
            }
        }

        Context CreateBehaviourTreeContext() {
            return Context.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            BehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}