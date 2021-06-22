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

            for (int i = 0; i < chain.Count; i++)
            {
                chain[i].transform.position = new Vector3((halfLength - (i * linkSep)) * 10.0f, 0.0f, 0.0f); // Scale up by 10 due to broken unity physics
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
            float anchorPosition = linkSep / 2.0f;
            for (int i = 1; i < chain.Count; i++)
            {
                var joint = chain[i].GetComponent<ConfigurableJoint>();
                var previousRigidbody = chain[i - 1].GetComponent<Rigidbody>();

                if (joint != null && previousRigidbody != null)
                {
                    // Connect the joint to the previous body if both are found
                    joint.anchor = new Vector3(anchorPosition, 0.0f, 0.0f);
                    joint.connectedBody = chain[i - 1].GetComponent<Rigidbody>();
                }
            }
        }

        /// <summary>
        /// Move the link meshes to the same positions of the chain
        /// </summary>
        public static void MoveLinkMeshes(List<GameObject> linkMeshes, List<GameObject> chain, float chainLength)
        {
            if (chain.Count < 2) { return; } // Do not use links for chain less than 2

            // Calculate required numbers first
            int chainSegments = chain.Count - 1;
            float shortChainLength = chainLength / chainSegments * (chainSegments - 1);  // Exclude the first segment from having links

            float chainSegmentLength = chainLength / chainSegments;
            float linkMeshSeparation = shortChainLength / linkMeshes.Count;

            for (int i = 0; i < linkMeshes.Count; i++)
            {
                // Determine positions on the chain length of the current link mesh
                float leftLinkMeshPosition = linkMeshSeparation * i + chainSegmentLength;
                float rightLinkMeshPosition = leftLinkMeshPosition + linkMeshSeparation;

                // Determine the chain links to calculate position from
                int leftChainIndex = (int)Math.Floor(leftLinkMeshPosition / chainSegmentLength);
                int rightChainIndex = (int)Math.Ceiling(rightLinkMeshPosition / chainSegmentLength);
                float leftFractionalPosition = (leftLinkMeshPosition - (leftChainIndex * chainSegmentLength)) / chainSegmentLength;
                float rightFractionalPosition = (rightLinkMeshPosition - ((rightChainIndex - 1) * chainSegmentLength)) / chainSegmentLength;

                // Find the left and right positions and rotations
                Vector3 leftPosition = ((chain[leftChainIndex + 1].transform.position - chain[leftChainIndex].transform.position) / 10.0f * leftFractionalPosition) + (chain[leftChainIndex].transform.position / 10.0f);
                Vector3 rightPosition = ((chain[rightChainIndex].transform.position - chain[rightChainIndex - 1].transform.position) / 10.0f * rightFractionalPosition) + (chain[rightChainIndex - 1].transform.position / 10.0f);

                Quaternion leftQuaternion = Quaternion.Lerp(chain[leftChainIndex].transform.rotation, chain[leftChainIndex + 1].transform.rotation, leftFractionalPosition);
                Quaternion rightQuaternion = Quaternion.Lerp(chain[rightChainIndex - 1].transform.rotation, chain[rightChainIndex].transform.rotation, rightFractionalPosition);
                Quaternion linkTwist = i % 2 != 0 ? Quaternion.Euler(90.0f, 0.0f, 0.0f) : Quaternion.identity;

                // Final interpolation from the left and right points
                linkMeshes[i].transform.position = ((rightPosition - leftPosition) / 2.0f) + leftPosition;
                linkMeshes[i].transform.rotation = Quaternion.Lerp(leftQuaternion, rightQuaternion, 0.5f) * linkTwist;
                linkMeshes[i].transform.rotation.Normalize();
            }
        }

        /// <summary>
        /// Creates the same number of link mesh instances as the number of links
        /// in the chain
        /// </summary>
        public static List<GameObject> CreateLinkMeshes(int chainCount, float chainLength)
        {
            const float linkMeshOverlap = 0.03f;
            const float linkMeshLength = 0.1f;

            if (chainCount < 2) return new List<GameObject>(); // Create no links if there is less than 2 links
            int chainSegments = chainCount - 1;
            float shortChainLength = chainLength / chainSegments * (chainSegments - 1);  // Exclude the first segment from having links

            // Calculate the number of links required with an overlap buffer
            int count = (int)Math.Round((shortChainLength - linkMeshOverlap) / (linkMeshLength - linkMeshOverlap));

            var result = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var link = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.LinkPrefab);
                result.Add(link);
            }

            return result;
        }
    }
}
