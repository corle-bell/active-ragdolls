﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActiveRagdoll {
    // Author: Sergio Abreu García | https://sergioabreu.me

    public class SensorsModule : Module {
        [Header("--- FLOOR ---")]
        public float floorDetectionDistance = 0.3f;
        public float maxFloorSlope = 60;

        private bool _isOnFloor = true;
        public bool IsOnFloor { get { return _isOnFloor; } }

        Rigidbody _rightFoot, _leftFoot;



        void Start() {
            _rightFoot = _activeRagdoll.GetPhysicalBone(HumanBodyBones.RightFoot).GetComponent<Rigidbody>();
            _leftFoot = _activeRagdoll.GetPhysicalBone(HumanBodyBones.LeftFoot).GetComponent<Rigidbody>();
        }

        void Update() {
            UpdateOnFloor();
        }

        private void UpdateOnFloor() {
            bool lastIsOnFloor = _isOnFloor;

            Vector3 foo;
            _isOnFloor = CheckRigidbodyOnFloor(_rightFoot, out foo)
                         || CheckRigidbodyOnFloor(_leftFoot, out foo);

            if (_isOnFloor != lastIsOnFloor)
                SendMessage("OnFloorChanged", _isOnFloor, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Checks whether the given rigidbody is on floor
        /// </summary>
        /// <param name="bodyPart">Part of the body to check</param
        /// <returns> True if the Rigidbody is on floor </returns>
        public bool CheckRigidbodyOnFloor(Rigidbody bodyPart, out Vector3 normal) {
            RaycastHit info;

            // Raycast
            Ray ray = new Ray(bodyPart.position, Vector3.down);
            bool onFloor = Physics.Raycast(ray, out info, floorDetectionDistance, ~(1 << bodyPart.gameObject.layer));

            // Additional checks
            onFloor = onFloor && Vector3.Angle(info.normal, Vector3.up) <= maxFloorSlope;

            /*if (onFloor) {
                AFloor floor = info.collider.gameObject.GetComponent<AFloor>();
                if (floor != null) onFloor = floor.isFloor;
            }*/

            normal = info.normal;
            return onFloor;
        }
    }
} // namespace ActiveRagdoll