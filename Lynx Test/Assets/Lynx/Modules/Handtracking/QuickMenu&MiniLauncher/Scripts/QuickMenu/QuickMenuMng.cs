using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Leap.Unity;

public class QuickMenuMng : MonoBehaviour
{

    #region INSPECTOR
    public Transform cameraEye;
    [Header("Activation Parameters")]
    public float activationDelay = 1f;
    [Space]
    public float pinchActivateDist = 20f;
    public float pinchDeactivateDist = 20f;
    [Space]
    public bool rightHandUseEnabled = true;
    public bool leftHandUseEnabled = true;

    [Header("QuickMenu Elements")]
    public GameObject quickMenuCanvas;
    public GameObject quickMenuFillIndicator;
    public Color baseColor;
    public Color highlightColor;
    //public List<GameObject> quickMenuButtonGOs;
    [Serializable]
    public class QuickMenuButton
    {
        public GameObject buttonGO;
        public UnityEvent unityEvent;
    }
    public List<QuickMenuButton> quickMenuButtons;

    [Header("QuickMenu Targets")]
    public MiniLauncherMng miniLauncher;
    #endregion

    #region PRIVATE
    //flags
    [HideInInspector] public float pinchRightDist = 10;
    [HideInInspector] public float pinchLeftDist = 10;

    [HideInInspector] public bool pinchLeft = false;
    [HideInInspector] public bool pinchLeftPrev = false;

    [HideInInspector] public bool pinchRight = false;
    [HideInInspector] public bool pinchRightPrev = false;

    [HideInInspector] public bool palmFacingEyeLeft = false;
    [HideInInspector] public bool palmFacingEyeRight = false;

    [HideInInspector] public bool palmOpenLeft = false;
    [HideInInspector] public bool palmOpenRight = false;

    [HideInInspector] public bool quickMenuButtonsActive = false;
    [HideInInspector] public bool quickMenuUsedByHandRight = false;
    [HideInInspector] public bool quickMenuUsedByHandLeft = false;

    //timer stuff
    [HideInInspector] public bool quickMenuTimerActive = false;
    private float timerStartedTime = 0;

    //positions
    private Vector3 quickMenuSpawnPos;
    private Vector3 quickMenuSpawnForward;

    //other
    [HideInInspector] public QuickMenuButton hoveredQuickMenuButton;
    [HideInInspector] public bool toggleTest = false;
    #endregion


    private void OnValidate()
    {
        if (!rightHandUseEnabled && !leftHandUseEnabled) rightHandUseEnabled = true;
        if (pinchDeactivateDist < pinchActivateDist) pinchDeactivateDist = pinchActivateDist;
    }

    private void Awake()
    {
        if (cameraEye == null) cameraEye = Camera.main.gameObject.transform;
    }

    private void Start()
    {
        DeactivateQuickMenu();
    }

    private void Update()
    {
        UpdatePalmFacingFlags(0.0f);
        UpdatePalmOpenFlags();
        UpdatePinchFlags();
        UpdatePinchDistanceFlags();

        //detect pinch start to isolate hand to track for quickmenuupdate
        if (!pinchRightPrev && pinchRight && !quickMenuUsedByHandLeft && rightHandUseEnabled) quickMenuUsedByHandRight = true;
        if (!pinchLeftPrev && pinchLeft && !quickMenuUsedByHandRight && leftHandUseEnabled) quickMenuUsedByHandLeft = true;
        //UpdateQuickMenu will release usedByHandFlags back to false on quickmenurelease conditions
        if (quickMenuUsedByHandRight) UpdateQuickMenu(true);
        if (quickMenuUsedByHandLeft) UpdateQuickMenu(false);
    }


    //Update flags methods
    private void UpdatePalmFacingFlags(float threshold)
    {
        Leap.Hand handRight = Hands.Right;
        if (handRight != null)
        {
            Vector3 palmRightForward = handRight.PalmNormal;
            Vector3 palmRightToEye = cameraEye.transform.position - handRight.PalmPosition;
            palmFacingEyeRight = Vector3.Dot(palmRightForward, palmRightToEye) > threshold;
        }
        else palmFacingEyeRight = false;

        Leap.Hand handLeft = Hands.Left;
        if (handLeft != null)
        {
            Vector3 palmLeftForward = handLeft.PalmNormal;
            Vector3 palmLeftToEye = cameraEye.transform.position - handLeft.PalmPosition;
            palmFacingEyeLeft = Vector3.Dot(palmLeftForward, palmLeftToEye) > threshold;
        }
        else palmFacingEyeLeft = false;

    }
    private void UpdatePalmOpenFlags()
    {
        Leap.Hand handRight = Hands.Right;
        if (handRight != null)
        {
            bool indexRightIsExtendedLeap = handRight.GetIndex().IsExtended;
            bool middleRightIsExtendedLeap = handRight.GetMiddle().IsExtended;
            bool ringRightIsExtendedLeap = handRight.GetRing().IsExtended;
            bool pinkyRightIsExtendedLeap = handRight.GetPinky().IsExtended;
            //palmOpenRight = pinkyRightIsExtendedLeap && ringRightIsExtendedLeap;
            palmOpenRight = pinkyRightIsExtendedLeap;
        }
        else palmOpenRight = false;

        Leap.Hand handLeft = Hands.Left;
        if (handLeft != null)
        {
            bool indexLeftIsExtendedLeap = handLeft.GetIndex().IsExtended;
            bool middleLeftIsExtendedLeap = handLeft.GetMiddle().IsExtended;
            bool ringLeftIsExtendedLeap = handLeft.GetRing().IsExtended;
            bool pinkyLeftIsExtendedLeap = handLeft.GetPinky().IsExtended;
            //palmOpenLeft = pinkyLeftIsExtendedLeap && ringLeftIsExtendedLeap;
            palmOpenLeft = pinkyLeftIsExtendedLeap;
        }
        else palmOpenLeft = false;
    }
    private void UpdatePinchFlags()
    {
        Leap.Hand handRight = Hands.Right;
        if (handRight != null)
        {
            if(!pinchRight) pinchRight = handRight.PinchDistance < pinchActivateDist;
            else            pinchRight = handRight.PinchDistance < pinchDeactivateDist;

        }
        else pinchRight = false;

        Leap.Hand handLeft = Hands.Left;
        if (handLeft != null)
        {
            if (!pinchLeft) pinchLeft = handLeft.PinchDistance < pinchActivateDist;
            else            pinchLeft = handLeft.PinchDistance < pinchDeactivateDist;
        }
        else pinchLeft = false;
    }
    private void UpdatePinchDistanceFlags()
    {
        Leap.Hand handRight = Hands.Right;
        if (handRight != null)
        {
            pinchRightDist = handRight.PinchDistance;
        }
        else pinchRightDist = Mathf.Infinity;

        Leap.Hand handLeft = Hands.Left;
        if (handLeft != null)
        {
            pinchLeftDist = handLeft.PinchDistance;
        }
        else pinchLeftDist = Mathf.Infinity;

    }


    //QuickMenu methods
    private void UpdateQuickMenu(bool rightHand)
    {
        bool pinch = rightHand ? pinchRight : pinchLeft;
        bool pinchPrev = rightHand ? pinchRightPrev : pinchLeftPrev;
        bool palmFacingEye = rightHand ? palmFacingEyeRight : palmFacingEyeLeft;
        bool palmOpen = rightHand ? palmOpenRight : palmOpenLeft;

        //pinch start
        if (!pinchPrev && pinch)
        {
            //pinchPrev = true;
            if (rightHand) pinchRightPrev = true;
            else           pinchLeftPrev = true;
            pinchPrev = rightHand ? pinchRightPrev : pinchLeftPrev;

            if (palmFacingEye && palmOpen && !quickMenuButtonsActive && !quickMenuTimerActive)
            {
                quickMenuTimerActive = true;
                timerStartedTime = Time.time;
                ActivateQuickMenu(rightHand);
                ActivateQuickMenuTimer();
            }
        }
        //timer progress interrupt condition
        if (quickMenuTimerActive && (!pinch || !palmFacingEye || !palmOpen))
        {
            quickMenuTimerActive = false;
            DeactivateQuickMenu();
            if (rightHand) pinchRightPrev = false;
            else           pinchLeftPrev = false;
            if (rightHand) quickMenuUsedByHandRight = false;
            else           quickMenuUsedByHandLeft = false;
        }
        //timer progress update & quickmenu buttons activate condition
        if (pinch && palmOpen && !quickMenuButtonsActive && quickMenuTimerActive)
        {
            UpdateQuickMenuPos(rightHand);
            UpdateQuickMenuTimerIndicator();
            float timeElapsed = Time.time - timerStartedTime;
            if (timeElapsed > activationDelay)
            {
                ActivateQuickMenuButtons();
                quickMenuButtonsActive = true;
                quickMenuTimerActive = false;
            }
        }
        //quickmenu buttons update
        if (quickMenuButtonsActive)
        {
            UpdateQuickMenuPos(rightHand);
            UpdateQuickMenuButtons(rightHand);
        }
        //pinch end
        if (pinchPrev && !pinch)
        {
            //pinchPrev = false;
            if (rightHand) pinchRightPrev = false;
            else           pinchLeftPrev = false;
            
            if (quickMenuButtonsActive || quickMenuTimerActive)
            {
                if(quickMenuButtonsActive && hoveredQuickMenuButton != null)
                {
                    CallHoveredButtonEvent();
                    hoveredQuickMenuButton = null;
                }
                DeactivateQuickMenu();
            }
            if (rightHand) quickMenuUsedByHandRight = false;
            else           quickMenuUsedByHandLeft = false;
        }
    }

    private void ActivateQuickMenu(bool rightHand)
    {
        Vector3 indexTipPos = (rightHand == true) ? Hands.Right.GetIndex().TipPosition 
                                                  : Hands.Left.GetIndex().TipPosition;

        quickMenuSpawnPos = indexTipPos;
        quickMenuSpawnForward = cameraEye.TransformDirection(Vector3.forward);
        quickMenuCanvas.SetActive(true);
    }
    private void UpdateQuickMenuPos(bool rightHand)
    {
        int v = 2;

        //place at index & oriente towards eye
        if (v == 1)
        {
            transform.position = quickMenuSpawnPos;
            transform.LookAt(cameraEye);
        }

        //orient along eye forward at spawn & keep near hand along eye forward
        if (v == 2)
        {
            Vector3 indexTipPos = (rightHand == true) ? Hands.Right.GetIndex().TipPosition
                                                      : Hands.Left.GetIndex().TipPosition;

            transform.position = quickMenuSpawnPos;
            //Vector3 eyeForward = cameraEye.TransformDirection(Vector3.forward);
            Vector3 eyeForward = quickMenuSpawnForward;
            float indexTipUIDist = 0.02f;
            transform.rotation = Quaternion.LookRotation(eyeForward);

            float offsetFromIndexAlongEyeFrowardVector = transform.InverseTransformPoint(indexTipPos).z;
            transform.position += eyeForward * (offsetFromIndexAlongEyeFrowardVector - indexTipUIDist);
            //transform.position = quickMenuSpawnPos + Vector3.Normalize(spawnPosToEyeVector) * indexTipUIDist;
        }
    }
    private void DeactivateQuickMenu()
    {
        quickMenuCanvas.SetActive(false);
        quickMenuButtonsActive = false;
        quickMenuTimerActive = false;
    }

    private void ActivateQuickMenuTimer()
    {
        quickMenuFillIndicator.SetActive(true);
        foreach (QuickMenuButton quickMenuButton in quickMenuButtons) quickMenuButton.buttonGO.SetActive(false);
    }
    private void UpdateQuickMenuTimerIndicator()
    {
        float timeElapsed = Time.time - timerStartedTime;
        float timerProgress = Mathf.InverseLerp(0, activationDelay, timeElapsed);
        quickMenuFillIndicator.GetComponent<Image>().fillAmount = timerProgress;
    }

    private void ActivateQuickMenuButtons()
    {
        quickMenuFillIndicator.SetActive(false);
        foreach (QuickMenuButton quickMenuButton in quickMenuButtons) quickMenuButton.buttonGO.SetActive(true);
    }
    private void UpdateQuickMenuButtons(bool rightHand)
    {
        Vector3 indexTipPos = (rightHand == true) ? Hands.Right.GetIndex().TipPosition
                                                  : Hands.Left.GetIndex().TipPosition;

        float closestButtonDist = Mathf.Infinity;
        QuickMenuButton clostestButton = quickMenuButtons[0];
        foreach (QuickMenuButton button in quickMenuButtons)
        {
            float buttonDist = Vector3.Distance(indexTipPos, button.buttonGO.transform.position);
            if(buttonDist < closestButtonDist)
            {
                clostestButton = button;
                closestButtonDist = buttonDist;
            }
        }
        foreach (QuickMenuButton button in quickMenuButtons)
        {
            button.buttonGO.GetComponent<Image>().color = baseColor;
        }
        clostestButton.buttonGO.GetComponent<Image>().color = highlightColor;
        hoveredQuickMenuButton = clostestButton;
    }


    //QuickMenu public button methods
    public void ToggleMiniLauncher()
    {
        if (miniLauncher.active) miniLauncher.DeactivateMiniLauncher();
        else                     miniLauncher.ActivateMiniLauncher();
    }
    public void CallHoveredButtonEvent()
    {
        if(hoveredQuickMenuButton.unityEvent != null) 
            hoveredQuickMenuButton.unityEvent.Invoke();
    }

}
