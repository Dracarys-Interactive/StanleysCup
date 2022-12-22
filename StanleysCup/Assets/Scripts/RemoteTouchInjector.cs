/* 
 *    
        DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
                    Version 2, December 2004

 Copyright (C) 2004 Sam Hocevar <sam@hocevar.net>

 Everyone is permitted to copy and distribute verbatim or modified
 copies of this license document, and changing it is allowed as long
 as the name is changed.

            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE
   TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION

  0. You just DO WHAT THE FUCK YOU WANT TO.


 * Written by Mitch because he was upset at people being upset
 * 
 * INSTRUCTIONS
 * Ensure Input mode is set to "Both"  (Input and InputSystem) in Unity project settings
 * Use Unity Remote as normal.
 * 
 * NOTES
 * TapCount is broken in 1.0.2 (verified) but repaired in 1.1preview.   See notes below for specifics in how to back-patch the fix into 1.0.2 if needed.
 * Input ordering is *not* guaranteed anywhere here, but at least its better than literally nothing.
 */


using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class RemoteTouchInjector : MonoBehaviour
{
    public bool showDebugGUI = false;

    Touchscreen touchScreen;
    TouchState primaryTouchState = default;
    TouchState[] touchStates = new TouchState[10];

    private void OnEnable()
    {
        touchScreen = InputSystem.AddDevice<Touchscreen>("Injected TouchScreen");
        InputSystem.onBeforeUpdate += Inject;
    }

    private void OnDisable()
    {
        InputSystem.onBeforeUpdate -= Inject;
        if (touchScreen != null)
            InputSystem.RemoveDevice(touchScreen);
    }

    unsafe private void Inject()
    {
        //InputState.currentTime
        for (int i = 0; i < 10; i++)
        {
            Touch t = default;

            if (i < Input.touchCount)
            {
                t = Input.GetTouch(i);
                var ts = touchStates[t.fingerId];
                ts.position = t.position;
                ts.delta = t.deltaPosition;
                ts.pressure = t.pressure;
                ts.touchId = t.fingerId;
                ts.radius = Vector2.one * t.radius;

                //Tap Count is done inside TouchScreen controldevice implementation with new InputSystem and is kinda funky.
                //Typically this is exposed as an Accessibility Setting on the end device itself (ie: the iPhone settings menus)

                //IMPORTANT NOTE:  If you want TapCount to actually function, you need to use the latest  Input System Preview Package (1.1.0+)
                //OR!  If you continue to use 1.0.2 (verified stable) Modify InputState.cs
                //WRONG        public static double currentTime => InputRuntime.s_Instance.currentTime + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
                //RIGHT        public static double currentTime => InputRuntime.s_Instance.currentTime - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
                //                                                                                      ^ zigged when supposed to zag

                switch (t.phase)
                {
                    case UnityEngine.TouchPhase.Began:
                        ts.startPosition = t.position;
                        ts.startTime = InputState.currentTime;
                        ts.phase = UnityEngine.InputSystem.TouchPhase.Began;
                        break;
                    case UnityEngine.TouchPhase.Moved:
                        ts.phase = UnityEngine.InputSystem.TouchPhase.Moved;
                        break;
                    case UnityEngine.TouchPhase.Stationary:
                        ts.phase = UnityEngine.InputSystem.TouchPhase.Stationary;
                        break;
                    case UnityEngine.TouchPhase.Ended:
                        ts.phase = UnityEngine.InputSystem.TouchPhase.Ended;
                        break;
                    case UnityEngine.TouchPhase.Canceled:
                        ts.phase = UnityEngine.InputSystem.TouchPhase.Canceled;
                        break;
                }

                touchStates[t.fingerId] = ts;
            }
        }

        foreach (var ts in touchStates)
        {
            if (ts.phase != UnityEngine.InputSystem.TouchPhase.None)
                InputSystem.QueueStateEvent(touchScreen, ts);
        }

        for (int i = 0; i < touchStates.Length; i++)
        {
            if (touchStates[i].phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                touchStates[i].phase = UnityEngine.InputSystem.TouchPhase.None;
                InputSystem.QueueStateEvent(touchScreen, touchStates[i]);
            }
        }
    }

    private void OnGUI()
    {
        if (!showDebugGUI)
            return;

        foreach (var t in Input.touches)
        {
            var pos = new Rect(t.position, Vector2.one * 300);
            pos.y = Screen.height - pos.y;
            GUI.Label(pos, $"OG\t{t.fingerId}\t{t.phase}\t{t.tapCount}");
        }

        foreach (var touchControl in touchScreen.touches)
        {
            var touchState = touchControl.ReadValue();
            if (touchState.phase == UnityEngine.InputSystem.TouchPhase.None)
                continue;

            var pos = new Rect(touchState.position, Vector2.one * 300);
            pos.y = Screen.height - pos.y - 20;
            GUI.Label(pos, $"IS\t{touchState.touchId}\t{touchState.phase}\t{touchState.tapCount}\t{touchState.isPrimaryTouch}");
            GUILayout.Label(touchState.touchId.ToString() + "  " + touchState.phase + "  " + touchState.isTap);
        }
    }
}
