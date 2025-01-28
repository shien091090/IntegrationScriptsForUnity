using System;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(EventTrigger))]
    public class OperableUI : MonoBehaviour
    {
        private const string DEBUGGER_KEY = "OperableUI";

        [SerializeField] private bool autoInitWhenStart;
        [SerializeField] private bool autoUpdate;
        [SerializeField] private bool isShowDebugLog;
        [SerializeField] private bool enableClick;
        [SerializeField] private bool enableDoubleClick;
        [SerializeField] private bool enableDrag;
        [SerializeField] private float checkDoubleClickTime;
        [SerializeField] private float checkDoubleClickCoolDownTime;

        private float waitDoubleClickTimer;
        private float doubleClickCoolDownTimer;
        private bool isClicked;
        private bool isDrag;
        private bool isWaitDoubleClick;
        private bool isDoubleClickCoolDown;
        private bool isWaitForDoubleClickUp;

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);
        private EventTrigger eventTrigger;

        public event Action OnClickEvent;
        public event Action OnDoubleClickEvent;
        public event Action OnStartDragEvent;
        public event Action OnDragOverEvent;

        private void Start()
        {
            if (autoInitWhenStart)
                Init();
        }

        private void Update()
        {
            if (autoUpdate)
                UpdatePerFrame(Time.deltaTime);
        }

        public void Init()
        {
            InitEventTrigger();
            waitDoubleClickTimer = 0;
            doubleClickCoolDownTimer = 0;
            isClicked = false;
            isDrag = false;
            isWaitDoubleClick = false;
            isDoubleClickCoolDown = false;
            isWaitDoubleClick = false;

            ShowLog("Init");
        }

        private void InitEventTrigger()
        {
            eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            EventTrigger.Entry dragEntry = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            eventTrigger.triggers.Remove(pointerDownEntry);
            eventTrigger.triggers.Remove(pointerUpEntry);
            eventTrigger.triggers.Remove(dragEntry);

            if (enableClick)
            {
                pointerDownEntry.callback.AddListener((data) =>
                {
                    OnClickDown();
                });

                pointerUpEntry.callback.AddListener((data) =>
                {
                    OnClickUp();
                });

                eventTrigger.triggers.Add(pointerDownEntry);
                eventTrigger.triggers.Add(pointerUpEntry);
            }

            if (enableDrag)
            {
                dragEntry.callback.AddListener((data) =>
                {
                    OnDrag();
                });

                eventTrigger.triggers.Add(dragEntry);
            }
        }

        public void UpdatePerFrame(float deltaTime)
        {
            if (isWaitDoubleClick)
                waitDoubleClickTimer += deltaTime;

            if (isDoubleClickCoolDown)
            {
                doubleClickCoolDownTimer += deltaTime;
                if (doubleClickCoolDownTimer >= checkDoubleClickCoolDownTime)
                    isDoubleClickCoolDown = false;
            }
        }

        private void MoveFollowMouse()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPositionInCamera = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = new Vector3(worldPositionInCamera.x, worldPositionInCamera.y, transform.position.z);
        }

        private void ShowLog(string log)
        {
            if (isShowDebugLog)
                debugger.ShowLog(log);
        }

        private void OnClickDown()
        {
            if (enableDoubleClick)
            {
                if (enableDoubleClick &&
                    waitDoubleClickTimer <= checkDoubleClickTime &&
                    isWaitDoubleClick)
                {
                    ShowLog("OnDoubleClickEvent");
                    isDoubleClickCoolDown = true;
                    isWaitForDoubleClickUp = true;
                    doubleClickCoolDownTimer = 0;

                    OnDoubleClickEvent?.Invoke();
                }
                else
                {
                    // ShowLog(isDoubleClickCoolDown ?
                    // "OnClickDown(DoubleClickCoolDown)" :
                    // "OnClickDown");
                }
            }

            waitDoubleClickTimer = 0;
            isClicked = true;
            isWaitDoubleClick = false;
        }

        private void OnClickUp()
        {
            isClicked = false;

            if (isDrag)
            {
                ShowLog("OnDragOverEvent");
                isDrag = false;
                OnDragOverEvent?.Invoke();
            }
            else
            {
                if (isDoubleClickCoolDown == false)
                    isWaitDoubleClick = true;

                if (isWaitForDoubleClickUp)
                    isWaitForDoubleClickUp = false;
                else
                {
                    ShowLog("OnClickEvent");
                    OnClickEvent?.Invoke();
                }
            }
        }

        private void OnDrag()
        {
            if (isClicked == false)
                return;

            if (isDrag == false)
            {
                ShowLog("OnStartDragEvent");
                isDrag = true;
                OnStartDragEvent?.Invoke();
            }

            MoveFollowMouse();
        }
    }
}