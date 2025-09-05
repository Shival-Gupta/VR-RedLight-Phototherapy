//-----------------------------------------------------------------------
// <copyright file="VrModeController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using Google.XR.Cardboard;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Management;

/// <summary>
/// Singleton VR Manager to handle persistent VR setup and teardown.
/// Handles Cardboard XR initialization, input, and lifecycle across scenes.
/// </summary>
public class VRManager : MonoBehaviour
{
    #region Singleton Implementation

    public static VRManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeScreen();
        StartCoroutine(InitializeVR());
    }

    #endregion

    #region Fields and Properties

    private const float _defaultFieldOfView = 60.0f;
    private Camera _mainCamera;

    public bool EnableTouchToEnterVR = true;

    private bool IsScreenTouched => GetFirstTouchIfExists()?.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began;

    public bool IsVREnabled => XRGeneralSettings.Instance.Manager.isInitializationComplete;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null) Debug.LogWarning("[VRManager] MainCamera not found in scene!");

        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }
    }

    private void Update()
    {
        if (!XRGeneralSettings.Instance || XRGeneralSettings.Instance.Manager == null)
            return;

        if (IsVREnabled)
        {
            HandleVREvents();
            Api.UpdateScreenParams();
        }
        else if (EnableTouchToEnterVR && IsScreenTouched)
        {
            EnterVR();
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isVRScene = GlobalConfig.IsVRScene(scene.name);

        EnableTouchToEnterVR = isVRScene;

        if (isVRScene && !IsVREnabled)
        {
            EnterVR();
        }
        else if (!isVRScene && IsVREnabled)
        {
            ExitVR();
        }

        Debug.Log($"[VRManager] Scene: {scene.name}, VR Mode: {(EnableTouchToEnterVR ? "ON" : "OFF")}");
    }

    #endregion


    #region Initialization

    private void InitializeScreen()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;
    }

    private IEnumerator InitializeVR()
    {
        yield return null; // Allow scene to stabilize

        if (!IsVREnabled)
        {
            yield return StartXR();
        }
    }

    #endregion

    #region XR Control

    public void EnterVR()
    {
        StartCoroutine(StartXR());

        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
    }

    public void ExitVR()
    {
        StopXR();

        string fallbackScene = GlobalConfig.SceneMainMenu;
        if (SceneManager.GetActiveScene().name != fallbackScene)
        {
            SceneManager.LoadScene(fallbackScene);
        }
    }

    private IEnumerator StartXR()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed.");
        }
        else
        {
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("XR Started.");
        }
    }

    private void StopXR()
    {
        Debug.Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        if (_mainCamera != null)
        {
            _mainCamera.ResetAspect();
            _mainCamera.fieldOfView = _defaultFieldOfView;
        }
    }

    #endregion

    #region Event Handlers

    private void HandleVREvents()
    {
        if (Api.IsCloseButtonPressed)
        {
            ExitVR();
        }

        if (Api.IsGearButtonPressed)
        {
            Api.ScanDeviceParams();
        }
    }

    #endregion

    #region Input Helpers

    private static TouchControl GetFirstTouchIfExists()
    {
        Touchscreen touchScreen = Touchscreen.current;

        if (touchScreen == null)
        {
            return null;
        }

        if (!touchScreen.enabled)
        {
            InputSystem.EnableDevice(touchScreen);
        }

        ReadOnlyArray<TouchControl> touches = touchScreen.touches;

        return touches.Count > 0 ? touches[0] : null;
    }

    #endregion
}
