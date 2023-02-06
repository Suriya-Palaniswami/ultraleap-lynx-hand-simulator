
//   ==============================================================================
//   | Lynx HandTracking Sample : LynxInterfaces (2022)                           |
//   | Author : GC                                                                |
//   |======================================                                      |
//   | InterfaceMoveGrasp Script                                                  |
//   | Script to move a UI.                                                       |
//   ==============================================================================

using UnityEngine;
using Leap.Unity.Interaction;
using Leap;

public class InterfaceMoveGrasp : MonoBehaviour
{
    public Transform targetInterface = null;
    public bool interfaceLookPlayer = false;

    private InteractionBehaviour interactionBehaviour = null;
    private Vector3 translateReference = Vector3.zero;
    private Vector3 translateUIReeference = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // Setup Move UI operation
        translateUIReeference = targetInterface.position - this.transform.position;

        // Gabbable ball operation
        interactionBehaviour = GetComponent<InteractionBehaviour>();

        interactionBehaviour.OnGraspBegin += () =>
        {
            if (interactionBehaviour.graspingHands.Count > 0)
            {
                Hand leapHand = interactionBehaviour.graspingController.intHand.leapHand;
                translateReference = leapHand.PalmPosition - this.transform.position;
            }
            
        };

        interactionBehaviour.OnGraspStay += () =>
        {
            if (interactionBehaviour.graspingHands.Count > 0)
            {
                Hand leapHand = interactionBehaviour.graspingController.intHand.leapHand;
                this.transform.position =  leapHand.PalmPosition - translateReference;
            }
        };
    }

    void Update()
    {
        // Move UI operation
        targetInterface.position = this.transform.position + translateUIReeference;

        // Interface look player operation
        if (interfaceLookPlayer)
        {
            targetInterface.rotation = Quaternion.LookRotation(targetInterface.position - Camera.main.transform.position);
        }
    }
}
