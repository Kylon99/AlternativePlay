using AlternativePlay.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlternativePlay
{
    public static class Utilities
    {
        /// <summary>
        /// Checks to see if the tracker data used should cause score submissions to be disabled.
        /// If the position of the saber is anything other than (0, 0, 0) then score
        /// submission will be disabled.  Rotation can be any value.
        /// </summary>
        public static void CheckAndDisableForTrackerTransforms(TrackerConfigData trackerConfigData)
        {
            if (String.IsNullOrWhiteSpace(trackerConfigData?.Serial)) return;

            if (trackerConfigData.Position != Vector3.zero)
            {
                // Disable scoring due to transforms
                AlternativePlay.Logger.Info($"Position: {trackerConfigData.Position}");
                AlternativePlay.Logger.Info("Disabling score submission on tracker with non-default position");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);
            }
        }

        /// <summary>
        /// Calculates the <see cref="Pose"/> based on the given <see cref="TrackerConfigData"/>
        /// given the current tracker's rotation and position.
        /// </summary>
        public static Pose CalculatePoseFromTrackerData(TrackerConfigData trackerConfigData, Pose tracker)
        {
            Pose result = Pose.identity;

            // Calculate and apply rotation first
            Quaternion extraRotation = Quaternion.Euler(trackerConfigData.EulerAngles);
            Quaternion finalRotation = tracker.rotation * extraRotation;
            result.rotation = finalRotation;

            // Rotate position and add 
            Vector3 rotatedPosition = tracker.rotation * trackerConfigData.Position;
            result.position = tracker.position + rotatedPosition;

            return result;
        }

        /// <summary>
        /// Rotates the pose around 180 degrees in both the Y and Z direction effectively
        /// reversing a saber direction.
        /// </summary>
        public static Pose Reverse(this Pose pose)
        {
            Pose result = pose;
            result.rotation *= Quaternion.Euler(0.0f, 180.0f, 180.0f);
            return result;
        }

        /// <summary>
        /// Creates a chain link to be used with Nunchaku and Flail behaviors. If head is true
        /// then it creates the head chain link with no configurable joints.  These links will have
        /// other links attaching to it instead.
        /// </summary>
        public static GameObject CreateLink(string name, float mass, float angularDrag, bool head = false)
        {
            var link = new GameObject(name);

            // Add the Rigidbody component
            var chainLinkRigid = link.AddComponent<Rigidbody>();
            chainLinkRigid.mass = mass;
            chainLinkRigid.useGravity = false;
            chainLinkRigid.isKinematic = head;
            chainLinkRigid.detectCollisions = false;
            chainLinkRigid.angularDrag = angularDrag;

            if (!head)
            {
                // Add the Configurable Joint component
                var joint = link.AddComponent<ConfigurableJoint>();
                joint.anchor = new Vector3(0.5f, 0.0f, 0.0f);
                joint.axis = new Vector3(0.0f, 0.0f, 1.0f);
                joint.secondaryAxis = new Vector3(0.0f, 1.0f, 0.0f);
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;
                joint.projectionMode = JointProjectionMode.PositionAndRotation;
                joint.projectionAngle = 0.0f;
                joint.projectionDistance = 0.0f;
            }

            return link;
        }

        /// <summary>
        /// Places the links in the proper position and then connects the joints of a given 
        /// chain to its previous link. Makes the assumption that the head of the chain needs 
        /// not be connected and that every link after has a <see cref="ConfigurableJoint"/>. 
        /// Every link must have a <see cref="Rigidbody"/> though.
        /// </summary>
        public static void ConnectChain(List<GameObject> chain, float length)
        {
            // Cannot connect a chain with less than 2 links
            if (chain == null || chain.Count < 2) { return; }

            // Sets the position of every chain link first
            float halfLength = length / 2.0f;
            float linkSep = length / (chain.Count - 1);
            Func<int, float> linkPosition = (int i) => (halfLength - (i * linkSep)) * 10.0f; // Scale up by 10 due to broken unity physics

            for (int i = 0; i < chain.Count; i++)
            {
                chain[i].transform.position = new Vector3(linkPosition(i), 0.0f, 0.0f);
                chain[i].transform.rotation = Quaternion.identity;

                // Reset the motion of the rigid body
                var rigid = chain[i].GetComponent<Rigidbody>();
                if (rigid != null)
                {
                    rigid.velocity = new Vector3();
                    rigid.angularVelocity = new Vector3();
                }
            }

            // Connect joints to rigid bodies
            for (int i = 1; i < chain.Count; i++)
            {
                var joint = chain[i].GetComponent<ConfigurableJoint>();
                var previousRigidbody = chain[i - 1].GetComponent<Rigidbody>();

                if (joint != null && previousRigidbody != null)
                {
                    // Connect the joint to the previous body if both are found
                    joint.connectedBody = chain[i - 1].GetComponent<Rigidbody>();
                }
            }
        }

        /// <summary>
        /// Move the link meshes to the same positions of the chain
        /// </summary>
        /// 
        public static void MoveLinks(List<GameObject> linkMeshes, List<GameObject> chain)
        {
            for (int i = 1; i < chain.Count; i++)
            {
                linkMeshes[i].transform.position = chain[i].transform.position / 10.0f;
                linkMeshes[i].transform.rotation = chain[i].transform.rotation;
            }
        }


        /// <summary>
        /// Creates the same number of link mesh instances as the number of links
        /// in the chain
        /// </summary>
        public static List<GameObject> CreateLinkMeshes(int linkCount)
        {
            var result = new List<GameObject>();
            for (int i = 0; i < linkCount; i++)
            {
                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gameObject.transform.localScale = new Vector3(0.07f, 0.03f, 0.03f);
                result.Add(gameObject);
            }

            return result;
        }
    }
}
