using com.rfilkov.kinect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractionFramework.Runtime
{
    public class MyGestureListener : MonoBehaviour, GestureListenerInterface
    {
        [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
        public int playerIndex = 0;

        public ulong playerID = 0;

        [Tooltip("List of the gestures to detect.")]
        public List<GestureType> detectGestures;

        // 单例
        public static MyGestureListener Instance;
        private void Awake()
        {
            Instance = this;
            detectGestures = new List<GestureType>()
            {
                GestureType.RaiseRightHand,
                GestureType.RaiseLeftHand
            };

        }

        /// <summary>
        /// 动作完成回调
        /// </summary>
        public Action<GestureType> GestureCompletedCallback;
        /// <summary>
        /// 是否有人
        /// </summary>
        public Action<bool> HasHuman;
        /// <summary>
        /// 人的位置
        /// </summary>
        public Action<Vector3> HumanPos;

        public bool GestureCancelled(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint)
        {
            return true;
        }

        public bool GestureCompleted(ulong userId, int userIndex, GestureType gesture, KinectInterop.JointType joint, Vector3 screenPos)
        {
            if (playerID != userId)
                return false;
            //有消息直接扔出去即可
            GestureCompletedCallback?.Invoke(gesture);
            //GestureCompletedCallback(gesture);
            return true;
        }

        public void GestureInProgress(ulong userId, int userIndex, GestureType gesture, float progress, KinectInterop.JointType joint, Vector3 screenPos)
        {

        }

        public void UserDetected(ulong userId, int userIndex)
        {
            //这里如果当前没有追踪用户就添加
            if (playerID == 0)
            {
                //playerID = userId;
                AddUser(userId);
            }
        }
        private void AddUser(ulong userId)
        {
            playerID = userId;
            // as an example - detect these user specific gestures 例如，检测这些特定于用户的手势
            KinectGestureManager gestureManager = KinectManager.Instance.gestureManager;
            // 识别列表中注册的所有姿势
            foreach (GestureType gesture in detectGestures)
            {
                gestureManager.DetectGesture(userId, gesture);
            }
        }
        public void UserLost(ulong userId, int userIndex)
        {
            if (userId != playerID)
                return;

            playerID = 0;

            //用户丢失后将自动移除所有手势监听，所以此处不用管

            //判断是否还有人，如果还有，就把那个人变成持有者
            if (KinectManager.Instance.GetAllUserIds().Count >= 1)
            {
                userId = KinectManager.Instance.GetAllUserIds()[0];
                AddUser(userId);
            }
        }

        private void Update()
        {
            if (playerID == 0)
            {
                return;
            }
            KinectManager kinectManager = KinectManager.Instance;
            if (kinectManager.IsInitialized() && kinectManager.IsUserDetected(0))
            {

                // 遍历得到的当前用户id
                foreach (var item in kinectManager.GetAllUserIds())
                {
                    if (item == playerID)
                    {
                        if (kinectManager.IsJointTracked(item, 0))
                        {
                            Rect backgroundRect = Camera.main.pixelRect;
                            //Vector3 posJoint = kinectManager.GetJointPosColorOverlay(item, KinectInterop.JointType.SpineChest, 0,Camera.main, backgroundRect);

                            Vector3 posJoint = kinectManager.GetJointPosition(item, KinectInterop.JointType.SpineChest);

                            //Vector3 SpineBasePos = kinectManager.GetJointKinectPosition(item, KinectInterop.JointType.SpineChest,true);
                            //Debug.Log(posJoint.x);
                            HumanPos?.Invoke(posJoint);
                        }
                    }
                }
            }
        }
    }
}