using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using DG.Tweening;
using TMPro;

public class CustomHandSimulator : MonoBehaviour
{
    public Hand handLeft = null;
    Hand handRight = null;
    
    [SerializeField]
    private Chirality HandednessLeft;
    
    [SerializeField]
    public CapsuleHand capsuleHandLeft;

    [SerializeField] TextMeshProUGUI _currentStateText;
    
    public Sequence thumbSequence;
    public Sequence indexSequence;
    public Sequence middleSequence;
    public Sequence ringSequence;
    public Sequence pinkySequence;
    
    #region Rest Vectors
        private Vector3 thumbDistalRest;
        private Vector3 thumbIntermediateRest;
        private Vector3 thumbProximalRest;

        private Vector3 indexDistalRest;
        private Vector3 indexIntermediateRest;
        private Vector3 indexProximalRest;

        private Vector3 middleDistalRest;
        private Vector3 middleIntermediateRest;
        private Vector3 middleProximalRest;

        private Vector3 ringDistalRest;
        private Vector3 ringIntermediateRest;
        private Vector3 ringProximalRest;

        private Vector3 pinkyDistalRest;
        private Vector3 pinkyIntermediateRest;
        private Vector3 pinkyProximalRest;

    #endregion
    
    #region GraspVectors
        private Vector3 thumbDistalGrasp;
        private Vector3 thumbIntermediateGrasp;
        private Vector3 thumbProximalGrasp;

        private Vector3 indexDistalGrasp;
        private Vector3 indexIntermediateGrasp;
        private Vector3 indexProximalGrasp;

        private Vector3 middleDistalGrasp;
        private Vector3 middleIntermediateGrasp;
        private Vector3 middleProximalGrasp;

        private Vector3 ringDistalGrasp;
        private Vector3 ringIntermediateGrasp;
        private Vector3 ringProximalGrasp;

        private Vector3 pinkyDistalGrasp;
        private Vector3 pinkyIntermediateGrasp;
        private Vector3 pinkyProximalGrasp;
    #endregion

    private bool isGrasped = false;
    private bool isPointing = false;
    
    private void Awake()
    {
        handLeft = TestHandFactory.MakeTestHand(HandednessLeft == Chirality.Left);

        #region Rest Vectors Initialization
        thumbDistalRest = handLeft.Finger(0).bones[3].NextJoint;
        thumbIntermediateRest = handLeft.Finger(0).bones[3].NextJoint;
        thumbProximalRest = handLeft.Finger(0).bones[3].NextJoint;

        indexDistalRest = handLeft.Finger(1).bones[3].NextJoint;
        indexIntermediateRest = handLeft.Finger(1).bones[3].NextJoint;
        indexProximalRest = handLeft.Finger(1).bones[3].NextJoint;

        middleDistalRest = handLeft.Finger(2).bones[3].NextJoint;
        middleIntermediateRest = handLeft.Finger(2).bones[3].NextJoint;
        middleProximalRest = handLeft.Finger(2).bones[3].NextJoint;

        ringDistalRest = handLeft.Finger(3).bones[3].NextJoint;
        ringIntermediateRest = handLeft.Finger(3).bones[3].NextJoint;
        ringProximalRest = handLeft.Finger(3).bones[3].NextJoint;

        pinkyDistalRest = handLeft.Finger(4).bones[3].NextJoint;
        pinkyIntermediateRest = handLeft.Finger(4).bones[3].NextJoint;
        pinkyProximalRest = handLeft.Finger(4).bones[3].NextJoint;
        #endregion

        #region Grasp Vectors Initialization
        thumbDistalGrasp = new Vector3(-0.01f, -0.02f, -0.04f);
        thumbIntermediateGrasp = new Vector3(-0.02f, -0.02f, -0.05f);
        thumbProximalGrasp = new Vector3(-0.04f, -0.018f, -0.04f);

        indexDistalGrasp = new Vector3(-0.02f, -0.04f, -0.01f);
        indexIntermediateGrasp = new Vector3(-0.0335026f, -0.01f, -0.03f);
        indexProximalGrasp = new Vector3(-0.029786529f, 0.02f, -0.02f);

        middleDistalGrasp = new Vector3(-0.01f, -0.04f, -0.01f);
        middleIntermediateGrasp = new Vector3(-0.01f, -0.01f, -0.03f);
        middleProximalGrasp = new Vector3(-0.01f, 0.02f, -0.02f);

        ringDistalGrasp = new Vector3(0.0f, -0.04f, -0.01f);
        ringIntermediateGrasp = new Vector3(0.01f, -0.01f, -0.03f);
        ringProximalGrasp = new Vector3(0.01f, 0.02f, -0.02f);

        pinkyDistalGrasp = new Vector3(0.015f, -0.04f, -0.01f);
        pinkyIntermediateGrasp = new Vector3(0.03f, -0.01f, -0.03f);
        pinkyProximalGrasp = new Vector3(0.03f, 0.02f, -0.02f); 

        #endregion
    }

    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            DoHandGraspTween();
            OnUpdateHand(handLeft);
            HandleUI("a");
        }
        else if (Input.GetKeyDown("q"))
        {
            RevertGraspToRest();
            OnUpdateHand(handLeft);
            HandleUI("q");

        }
        else if (Input.GetKeyDown("e"))
        {
            PointIndex();
            OnUpdateHand(handLeft);
            HandleUI("e");
        }
        else if (Input.GetKeyDown("y"))
        {
            DOYolo();
            OnUpdateHand(handLeft);
            HandleUI("y");
        }
        else
        {
            capsuleHandLeft.SetLeapHand(handLeft);
            capsuleHandLeft.UpdateHandWithEvent();
        }
    }
    
    void OnUpdateHand(Hand _hand)
        {
            Debug.Log("On Update Called");

            capsuleHandLeft.SetLeapHand(handLeft);

            capsuleHandLeft.UpdateHandWithEvent();
        }
    
    private void DoHandGraspTween()
    {
        isGrasped = true;
        #region Thumb
        thumbSequence = DOTween.Sequence();
        //distal
        thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[3].NextJoint, x => handLeft.Finger(0).bones[3].NextJoint = x, thumbDistalGrasp, 0.2f));
        //intermediate
        thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[2].NextJoint, x => handLeft.Finger(0).bones[2].NextJoint = x, thumbIntermediateGrasp,0.2f));
        //proximal
        thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[1].NextJoint, x => handLeft.Finger(0).bones[1].NextJoint = x, thumbProximalGrasp, 0.2f));
        thumbSequence.Play();
        #endregion

        #region Index
        indexSequence = DOTween.Sequence();
        //distal
        indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[3].NextJoint, x => handLeft.Finger(1).bones[3].NextJoint = x, middleDistalGrasp, 0.1f));
        //intermediate
        indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[2].NextJoint, x => handLeft.Finger(1).bones[2].NextJoint = x, middleIntermediateGrasp, 0.1f));
        //proximal
        indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[1].NextJoint, x => handLeft.Finger(1).bones[1].NextJoint = x, middleProximalGrasp, 0.1f));
        indexSequence.Play();
        #endregion

        #region Middle
        middleSequence = DOTween.Sequence();
        //distal
        middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[3].NextJoint, x => handLeft.Finger(2).bones[3].NextJoint = x, middleDistalGrasp, 0.1f));
        //intermediate
        middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[2].NextJoint, x => handLeft.Finger(2).bones[2].NextJoint = x, middleIntermediateGrasp, 0.1f));
        //proximal
        middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[1].NextJoint, x => handLeft.Finger(2).bones[1].NextJoint = x, middleProximalGrasp, 0.1f));
        middleSequence.Play();

        #endregion

        #region Ring

        ringSequence = DOTween.Sequence();
        //distal
        ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[3].NextJoint, x => handLeft.Finger(3).bones[3].NextJoint = x, ringDistalGrasp, 0.1f));
        //intermediate
        ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[2].NextJoint, x => handLeft.Finger(3).bones[2].NextJoint = x, ringIntermediateGrasp, 0.1f));
        //proximal
        ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[1].NextJoint, x => handLeft.Finger(3).bones[1].NextJoint = x, ringProximalGrasp, 0.1f));
        ringSequence.Play();
        
        #endregion

        #region Pinky
        pinkySequence = DOTween.Sequence();
        //distal
        pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[3].NextJoint, x => handLeft.Finger(4).bones[3].NextJoint = x, pinkyDistalGrasp, 0.1f));
        //intermediate
        pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[2].NextJoint, x => handLeft.Finger(4).bones[2].NextJoint = x, pinkyIntermediateGrasp, 0.1f));
        //proximal
        pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[1].NextJoint, x => handLeft.Finger(4).bones[1].NextJoint = x, pinkyProximalGrasp, 0.1f));
        pinkySequence.Play();

        
        #endregion
    }
    
    private void RevertGraspToRest()
    {
        isGrasped = false;

        #region Thumb
        thumbSequence.Kill();
        thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[3].NextJoint, x => handLeft.Finger(0).bones[3].NextJoint = x, thumbDistalRest, 0.2f));
        //intermediate
        thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[2].NextJoint, x => handLeft.Finger(0).bones[2].NextJoint = x, thumbIntermediateRest, 0.2f));
        //proximal
        thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[1].NextJoint, x => handLeft.Finger(0).bones[1].NextJoint = x, thumbProximalRest, 0.2f));
        thumbSequence.Play();
        #endregion

        #region Index
        indexSequence.Kill();
        indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[3].NextJoint, x => handLeft.Finger(1).bones[3].NextJoint = x, indexDistalRest, 0.1f));
        //intermediate
        indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[2].NextJoint, x => handLeft.Finger(1).bones[2].NextJoint = x, indexIntermediateRest, 0.1f));
        //proximal
        indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[1].NextJoint, x => handLeft.Finger(1).bones[1].NextJoint = x, indexProximalRest, 0.1f));
        indexSequence.Play();
        #endregion

        #region Middle
        middleSequence.Kill();
        middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[3].NextJoint, x => handLeft.Finger(2).bones[3].NextJoint = x, middleDistalRest, 0.1f));
        //intermediate
        middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[2].NextJoint, x => handLeft.Finger(2).bones[2].NextJoint = x, middleIntermediateRest, 0.1f));
        //proximal
        middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[1].NextJoint, x => handLeft.Finger(2).bones[1].NextJoint = x, middleProximalRest, 0.1f));
        middleSequence.Play();
        #endregion

        #region Ring
        ringSequence.Kill();
        ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[3].NextJoint, x => handLeft.Finger(3).bones[3].NextJoint = x, ringDistalRest, 0.1f));
        //intermediate
        ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[2].NextJoint, x => handLeft.Finger(3).bones[2].NextJoint = x, ringIntermediateRest, 0.1f));
        //proximal
        ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[1].NextJoint, x => handLeft.Finger(3).bones[1].NextJoint = x, ringProximalRest, 0.1f));
        ringSequence.Play();
        #endregion

        #region Pinky
        pinkySequence.Kill();
        pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[3].NextJoint, x => handLeft.Finger(4).bones[3].NextJoint = x, pinkyDistalRest, 0.1f));
        //intermediate
        pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[2].NextJoint, x => handLeft.Finger(4).bones[2].NextJoint = x, pinkyIntermediateRest, 0.1f));
        //proximal
        pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[1].NextJoint, x => handLeft.Finger(4).bones[1].NextJoint = x, pinkyProximalRest, 0.1f));
        pinkySequence.Play();
        #endregion
    }

    private void PointIndex()
    {
        if (isGrasped)
        {
            #region Index
            indexSequence.Kill();
            indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[3].NextJoint, x => handLeft.Finger(1).bones[3].NextJoint = x, indexDistalRest, 0.1f));
            //intermediate
            indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[2].NextJoint, x => handLeft.Finger(1).bones[2].NextJoint = x, indexIntermediateRest, 0.1f));
            //proximal
            indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[1].NextJoint, x => handLeft.Finger(1).bones[1].NextJoint = x, indexProximalRest, 0.1f));
            indexSequence.Play();
            #endregion
        }
        else
        {
            #region Thumb
            thumbSequence = DOTween.Sequence();
            //distal
            thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[3].NextJoint, x => handLeft.Finger(0).bones[3].NextJoint = x, thumbDistalGrasp, 0.2f));
            //intermediate
            thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[2].NextJoint, x => handLeft.Finger(0).bones[2].NextJoint = x, thumbIntermediateGrasp, 0.2f));
            //proximal
            thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[1].NextJoint, x => handLeft.Finger(0).bones[1].NextJoint = x, thumbProximalGrasp, 0.2f));
            thumbSequence.Play();
            #endregion

            #region Middle
            middleSequence = DOTween.Sequence();
            //distal
            middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[3].NextJoint, x => handLeft.Finger(2).bones[3].NextJoint = x, middleDistalGrasp, 0.1f));
            //intermediate
            middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[2].NextJoint, x => handLeft.Finger(2).bones[2].NextJoint = x, middleIntermediateGrasp, 0.1f));
            //proximal
            middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[1].NextJoint, x => handLeft.Finger(2).bones[1].NextJoint = x, middleProximalGrasp, 0.1f));
            middleSequence.Play();

            #endregion

            #region Ring

            ringSequence = DOTween.Sequence();
            //distal
            ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[3].NextJoint, x => handLeft.Finger(3).bones[3].NextJoint = x, ringDistalGrasp, 0.1f));
            //intermediate
            ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[2].NextJoint, x => handLeft.Finger(3).bones[2].NextJoint = x, ringIntermediateGrasp, 0.1f));
            //proximal
            ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[1].NextJoint, x => handLeft.Finger(3).bones[1].NextJoint = x, ringProximalGrasp, 0.1f));
            ringSequence.Play();

            #endregion

            #region Pinky
            pinkySequence = DOTween.Sequence();
            //distal
            pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[3].NextJoint, x => handLeft.Finger(4).bones[3].NextJoint = x, pinkyDistalGrasp, 0.1f));
            //intermediate
            pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[2].NextJoint, x => handLeft.Finger(4).bones[2].NextJoint = x, pinkyIntermediateGrasp, 0.1f));
            //proximal
            pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[1].NextJoint, x => handLeft.Finger(4).bones[1].NextJoint = x, pinkyProximalGrasp, 0.1f));
            pinkySequence.Play();


            #endregion
        }
    }

    private void DOYolo()
    {
        if (isGrasped)
        {
            #region Index
            indexSequence.Kill();
            indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[3].NextJoint, x => handLeft.Finger(1).bones[3].NextJoint = x, indexDistalRest, 0.1f));
            //intermediate
            indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[2].NextJoint, x => handLeft.Finger(1).bones[2].NextJoint = x, indexIntermediateRest, 0.1f));
            //proximal
            indexSequence.Append(DOTween.To(() => handLeft.Finger(1).bones[1].NextJoint, x => handLeft.Finger(1).bones[1].NextJoint = x, indexProximalRest, 0.1f));
            indexSequence.Play();
            #endregion

            #region Middle
            middleSequence.Kill();
            middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[3].NextJoint, x => handLeft.Finger(2).bones[3].NextJoint = x, middleDistalRest, 0.1f));
            //intermediate
            middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[2].NextJoint, x => handLeft.Finger(2).bones[2].NextJoint = x, middleIntermediateRest, 0.1f));
            //proximal
            middleSequence.Append(DOTween.To(() => handLeft.Finger(2).bones[1].NextJoint, x => handLeft.Finger(2).bones[1].NextJoint = x, middleProximalRest, 0.1f));
            middleSequence.Play();
            #endregion
        }
        else
        {
            #region Thumb
            thumbSequence = DOTween.Sequence();
            //distal
            thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[3].NextJoint, x => handLeft.Finger(0).bones[3].NextJoint = x, thumbDistalGrasp, 0.2f));
            //intermediate
            thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[2].NextJoint, x => handLeft.Finger(0).bones[2].NextJoint = x, thumbIntermediateGrasp, 0.2f));
            //proximal
            thumbSequence.Append(DOTween.To(() => handLeft.Finger(0).bones[1].NextJoint, x => handLeft.Finger(0).bones[1].NextJoint = x, thumbProximalGrasp, 0.2f));
            thumbSequence.Play();
            #endregion

            #region Ring

            ringSequence = DOTween.Sequence();
            //distal
            ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[3].NextJoint, x => handLeft.Finger(3).bones[3].NextJoint = x, ringDistalGrasp, 0.1f));
            //intermediate
            ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[2].NextJoint, x => handLeft.Finger(3).bones[2].NextJoint = x, ringIntermediateGrasp, 0.1f));
            //proximal
            ringSequence.Append(DOTween.To(() => handLeft.Finger(3).bones[1].NextJoint, x => handLeft.Finger(3).bones[1].NextJoint = x, ringProximalGrasp, 0.1f));
            ringSequence.Play();

            #endregion

            #region Pinky
            pinkySequence = DOTween.Sequence();
            //distal
            pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[3].NextJoint, x => handLeft.Finger(4).bones[3].NextJoint = x, pinkyDistalGrasp, 0.1f));
            //intermediate
            pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[2].NextJoint, x => handLeft.Finger(4).bones[2].NextJoint = x, pinkyIntermediateGrasp, 0.1f));
            //proximal
            pinkySequence.Append(DOTween.To(() => handLeft.Finger(4).bones[1].NextJoint, x => handLeft.Finger(4).bones[1].NextJoint = x, pinkyProximalGrasp, 0.1f));
            pinkySequence.Play();


            #endregion
        }
    }

    private void HandleUI(string currentState)
    {
        switch (currentState)
        {
            case "a": _currentStateText.text = "Current State : GRASPED";break;
            case "q": _currentStateText.text = "Current State : REST";break;
            case "e": _currentStateText.text = "Current State : POINTING";break;
            case "y": _currentStateText.text = "Current State : YOLO"; break;
        }
    }
}
