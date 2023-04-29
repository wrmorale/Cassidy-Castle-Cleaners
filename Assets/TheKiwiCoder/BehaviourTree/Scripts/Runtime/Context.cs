using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TheKiwiCoder {

    // The context is a shared object every node has access to.
    // Commonly used components and subsytems should be stored here
    // It will be somewhat specfic to your game exactly what to add here.
    // Feel free to extend this class 

    /*Santi: Does adding more things to this class make it slower? I'll
     * have to focus on just getting this done first and then consider
     * performance later.
     */
    public class Context {
        public GameObject gameObject;
        public Transform transform;
        public Animator animator; //Will try to make it target the animator in the children.
        public Rigidbody rigidbody;
        public NavMeshAgent agent;
        //public SphereCollider sphereCollider;
        //public BoxCollider boxCollider;
        //public CapsuleCollider capsuleCollider;
        public CharacterController characterController;
        // Add other game specific systems here
        public Enemy enemy;
        public AttackManager attackManager;
        public MirrorBossMain mirrorBossScript;

        public static Context CreateFromGameObject(GameObject gameObject) {
            // Fetch all commonly used components
            Context context = new Context();
            context.gameObject = gameObject;
            context.transform = gameObject.transform;
            context.animator = gameObject.GetComponentInChildren<Animator>(); //Enemies have their animator in their model, a child
            context.rigidbody = gameObject.GetComponent<Rigidbody>();
            context.agent = gameObject.GetComponent<NavMeshAgent>();
            //context.sphereCollider = gameObject.GetComponent<SphereCollider>();
            //context.boxCollider = gameObject.GetComponent<BoxCollider>();
            //context.capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
            context.characterController = gameObject.GetComponent<CharacterController>();

            // Add whatever else you need here...
            context.enemy = gameObject.GetComponent<Enemy>();
            context.attackManager = gameObject.GetComponent<AttackManager>();
            context.mirrorBossScript = gameObject.GetComponentInParent<MirrorBossMain>();

            return context;
        }
    }
}