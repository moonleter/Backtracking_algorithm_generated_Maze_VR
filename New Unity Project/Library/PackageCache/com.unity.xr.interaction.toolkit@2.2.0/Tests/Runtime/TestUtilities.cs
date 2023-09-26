﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace UnityEngine.XR.Interaction.Toolkit.Tests
{
    internal static class TestUtilities
    {
        internal static void DestroyAllSceneObjects()
        {
            for (var index = 0; index < SceneManager.sceneCount; ++index)
            {
                var scene = SceneManager.GetSceneAt(index);
                foreach (var go in scene.GetRootGameObjects())
                {
                    if (go.name.Contains("tests runner"))
                        continue;
                    Object.DestroyImmediate(go);
                }
            }
        }

        internal static void CreateGOBoxCollider(GameObject go, bool isTrigger = true)
        {
            BoxCollider collider = go.AddComponent<BoxCollider>();
            collider.size = new Vector3(2.0f, 2.0f, 2.0f);
            collider.isTrigger = isTrigger;
        }

        internal static void CreateGOSphereCollider(GameObject go, bool isTrigger = true)
        {
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.radius = 1.0f;
            collider.isTrigger = isTrigger;
        }

        internal static XRInteractionManager CreateInteractionManager()
        {
            GameObject managerGO = new GameObject("Interaction Manager");
            XRInteractionManager manager = managerGO.AddComponent<XRInteractionManager>();
            return manager;
        }

        internal static XRDirectInteractor CreateDirectInteractor()
        {
            GameObject interactorGO = new GameObject("Direct Interactor");
            CreateGOSphereCollider(interactorGO);
            XRController controller = interactorGO.AddComponent<XRController>();
            XRDirectInteractor interactor = interactorGO.AddComponent<XRDirectInteractor>();
            interactor.xrController = controller;
            controller.enableInputTracking = false;
            controller.enableInputActions = false;
            return interactor;
        }

        internal static XROrigin CreateXROrigin()
        {
            var xrOriginGO = new GameObject("XR Origin");
            xrOriginGO.SetActive(false);
            var xrOrigin = xrOriginGO.AddComponent<XROrigin>();
            xrOrigin.Origin = xrOriginGO;

            // Add camera offset
            var cameraOffsetGO = new GameObject("CameraOffset");
            cameraOffsetGO.transform.SetParent(xrOrigin.transform,false);
            xrOrigin.CameraFloorOffsetObject = cameraOffsetGO;

            xrOrigin.transform.position = Vector3.zero;
            xrOrigin.transform.rotation = Quaternion.identity;

            // Add camera
            var cameraGO = new GameObject("Camera");
            var camera = cameraGO.AddComponent<Camera>();

            cameraGO.transform.SetParent(cameraOffsetGO.transform, false);
            xrOrigin.Camera = cameraGO.GetComponent<Camera>();
            xrOriginGO.SetActive(true);

#if ENABLE_VR
            XRDevice.DisableAutoXRCameraTracking(camera, true);
#endif

            return xrOrigin;
        }

        internal static TeleportationAnchor CreateTeleportAnchorPlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "plane";
            TeleportationAnchor teleAnchor = plane.AddComponent<TeleportationAnchor>();
            return teleAnchor;
        }

        internal static XRRayInteractor CreateRayInteractor()
        {
            GameObject interactorGO = new GameObject("Ray Interactor");
            XRController controller = interactorGO.AddComponent<XRController>();
            XRRayInteractor interactor = interactorGO.AddComponent<XRRayInteractor>();
            XRInteractorLineVisual ilv = interactorGO.AddComponent<XRInteractorLineVisual>();
            interactor.xrController = controller;
            controller.enableInputTracking = false;
            interactor.enableUIInteraction = false;
            controller.enableInputActions = false;
            return interactor;
        }

        internal static XRSocketInteractor CreateSocketInteractor()
        {
            GameObject interactorGO = new GameObject("Socket Interactor");
            CreateGOSphereCollider(interactorGO);
            XRSocketInteractor interactor = interactorGO.AddComponent<XRSocketInteractor>();
            return interactor;
        }

        internal static MockInteractor CreateMockInteractor()
        {
            var interactorGO = new GameObject("Mock Interactor");
            interactorGO.transform.localPosition = Vector3.zero;
            interactorGO.transform.localRotation = Quaternion.identity;
            var interactor = interactorGO.AddComponent<MockInteractor>();
            return interactor;
        }

        internal static XRGrabInteractable CreateGrabInteractable()
        {
            GameObject interactableGO = new GameObject("Grab Interactable");
            CreateGOSphereCollider(interactableGO, false);
            XRGrabInteractable interactable = interactableGO.AddComponent<XRGrabInteractable>();
            var rigidBody = interactableGO.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRSimpleInteractable CreateSimpleInteractable()
        {
            GameObject interactableGO = new GameObject("Simple Interactable");
            CreateGOSphereCollider(interactableGO, false);
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRSimpleInteractable CreateSimpleInteractableWithColliders()
        {
            GameObject interactableGO = new GameObject("Simple Interactable with Colliders");
            CreateGOSphereCollider(interactableGO, false);
            CreateGOBoxCollider(interactableGO, false);
            XRSimpleInteractable interactable = interactableGO.AddComponent<XRSimpleInteractable>();
            Rigidbody rigidBody = interactableGO.AddComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
            return interactable;
        }

        internal static XRControllerRecorder CreateControllerRecorder(XRController controller, Action<XRControllerRecording> addRecordingFrames)
        {
            var controllerRecorder = controller.gameObject.AddComponent<XRControllerRecorder>();
            controllerRecorder.xrController = controller;
            controllerRecorder.recording = ScriptableObject.CreateInstance<XRControllerRecording>();

            addRecordingFrames(controllerRecorder.recording);
            return controllerRecorder;
        }

        internal static XRTargetFilter CreateTargetFilter()
        {
            GameObject filterGO = new GameObject("Target Filter");
            return filterGO.AddComponent<XRTargetFilter>();
        }
    }

    class MockInteractor : XRBaseInteractor
    {
        public List<IXRInteractable> validTargets { get; } = new List<IXRInteractable>();

        /// <inheritdoc />
        public override void GetValidTargets(List<IXRInteractable> targets)
        {
            targets.Clear();
            targets.AddRange(validTargets);
        }
    }

    enum TargetFilterCallback
    {
        Link,
        Unlink,
        Process,
    }

    class MockTargetFilter : IXRTargetFilter
    {
        public readonly List<TargetFilterCallback> callbackExecution = new List<TargetFilterCallback>();

        public bool canProcess { get; set; } = true;

        public void Link(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Link);
        }

        public void Unlink(IXRInteractor interactor)
        {
            callbackExecution.Add(TargetFilterCallback.Unlink);
        }

        public void Process(IXRInteractor interactor, List<IXRInteractable> targets, List<IXRInteractable> results)
        {
            callbackExecution.Add(TargetFilterCallback.Process);
            results.AddRange(targets);
        }
    }

    class MockGrabTransformer : IXRGrabTransformer
    {
        public enum MethodTrace
        {
            OnLink,
            OnGrab,
            OnGrabCountChanged,
            ProcessFixed,
            ProcessDynamic,
            ProcessLate,
            ProcessOnBeforeRender,
            OnUnlink,
        }

        public List<MethodTrace> methodTraces { get; } = new List<MethodTrace>();

        public Dictionary<XRInteractionUpdateOrder.UpdatePhase, bool> phasesTraced { get; } = new Dictionary<XRInteractionUpdateOrder.UpdatePhase, bool>
        {
            { XRInteractionUpdateOrder.UpdatePhase.Fixed, false},
            { XRInteractionUpdateOrder.UpdatePhase.Dynamic, true},
            { XRInteractionUpdateOrder.UpdatePhase.Late, false},
            { XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender, false},
        };

        /// <summary>
        /// The <see cref="Pose"/> value to set in <see cref="Process"/>.
        /// Set to <see langword="null"/> to skip using it.
        /// </summary>
        public Pose? targetPoseValue { get; set; }

        /// <summary>
        /// The <see cref="Vector3"/> local scale value to set in <see cref="Process"/>.
        /// Set to <see langword="null"/> to skip using it.
        /// </summary>
        public Vector3? localScaleValue { get; set; }

        /// <inheritdoc />
        public bool canProcess { get; set; } = true;

        /// <inheritdoc />
        public void OnLink(XRGrabInteractable grabInteractable)
        {
            methodTraces.Add(MethodTrace.OnLink);
        }

        /// <inheritdoc />
        public void OnGrab(XRGrabInteractable grabInteractable)
        {
            methodTraces.Add(MethodTrace.OnGrab);
        }

        /// <inheritdoc />
        public void OnGrabCountChanged(XRGrabInteractable grabInteractable, Pose targetPose, Vector3 localScale)
        {
            methodTraces.Add(MethodTrace.OnGrabCountChanged);
        }

        /// <inheritdoc />
        public void Process(XRGrabInteractable grabInteractable, XRInteractionUpdateOrder.UpdatePhase updatePhase, ref Pose targetPose, ref Vector3 localScale)
        {
            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Fixed:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessFixed);
                    break;
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessDynamic);
                    break;
                case XRInteractionUpdateOrder.UpdatePhase.Late:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessLate);
                    break;
                case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:
                    if (phasesTraced[updatePhase])
                        methodTraces.Add(MethodTrace.ProcessOnBeforeRender);
                    break;
                default:
                    Assert.Fail($"Unhandled {nameof(XRInteractionUpdateOrder.UpdatePhase)}={updatePhase}");
                    break;
            }

            if (targetPoseValue.HasValue)
                targetPose = targetPoseValue.Value;

            if (localScaleValue.HasValue)
                localScale = localScaleValue.Value;
        }

        /// <inheritdoc />
        public void OnUnlink(XRGrabInteractable grabInteractable)
        {
            methodTraces.Add(MethodTrace.OnUnlink);
        }
    }
}
