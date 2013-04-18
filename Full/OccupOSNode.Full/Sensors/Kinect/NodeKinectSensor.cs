// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeKinectSensor.cs" company="OccupOS">
//   This file is part of OccupOS.
//   OccupOS is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//   OccupOS is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
//   You should have received a copy of the GNU General Public License along with OccupOS.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("OccupOSNode.Full.Tests")]

namespace OccupOSNode.Sensors.Kinect
{
    using System;
    using System.Collections;

    using Microsoft.Kinect;

    using OccupOS.CommonLibrary.Sensors;

    internal class NodeKinectSensor : Sensor, ISoundSensor, IEntityPositionSensor, IEntityCountSensor, IDynamicSensor
    {
        private const int MAX_AUTO_CONNECTION_ATTEMPTS = 10;
        private const int MAX_TIME_DIFFERENCE = 200;
        private const int QUEUE_MAX_LENGTH = 6;

        private KinectSensor ksensor;

        public NodeKinectSensor(int id)
            : base(id)
        {
        }

        public void Connect()
        {
            bool connected = false;
            for (int k = 0; k < MAX_AUTO_CONNECTION_ATTEMPTS; k++)
            {
                connected = this.FindKinectSensor();
                if (connected)
                {
                    break;
                }
            }
        }

        public void Disconnect()
        {
            if (this.ksensor != null)
            {
                this.StopSensor(this.ksensor);
            }
        }

        public bool FindKinectSensor()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                if (this.ID > KinectSensor.KinectSensors.Count - 1)
                {
                    this.ksensor = KinectSensor.KinectSensors[KinectSensor.KinectSensors.Count - 1];
                }
                else
                {
                    this.ksensor = KinectSensor.KinectSensors[this.ID];
                }

                if (this.ksensor != null && this.ksensor.Status == KinectStatus.Connected)
                {
                    this.ksensor.ColorStream.Enable();
                    this.ksensor.DepthStream.Enable();
                    var tsparams = new TransformSmoothParameters
                                       {
                                           Smoothing = 0.2f, 
                                           Correction = 0.6f, 
                                           Prediction = 0.0f, 
                                           JitterRadius = 0.5f, 
                                           MaxDeviationRadius = 0.05f
                                       };
                    this.ksensor.SkeletonStream.Enable(tsparams);
                    this.ksensor.Start();
                    return true;
                }
            }

            return false;
        }

        public ConnectionStatus GetConnectionStatus()
        {
            if (this.ksensor != null)
            {
                switch (this.ksensor.Status)
                {
                    case KinectStatus.Connected:
                        return ConnectionStatus.Connected;
                    case KinectStatus.Disconnected:
                        return ConnectionStatus.Disconnected;
                    case KinectStatus.Initializing:
                        return ConnectionStatus.Connecting;
                    default:
                        return ConnectionStatus.Error;
                }
            }
            else
            {
                return ConnectionStatus.Disconnected;
            }
        }

        public override SensorData GetData()
        {
            var sensorData = new SensorData
                                 {
                                     SensorType = this,
                                     ReadTime = DateTime.Now,
                                     EntityCount = this.GetEntityCount(), 
                                     EntityPositions = this.GetEntityPositions()
                                 };
            return sensorData;
        }

        public int GetDeviceCount()
        {
            return KinectSensor.KinectSensors.Count;
        }

        public int GetEntityCount()
        {
            int count = 0;
            if (this.ksensor != null && this.ksensor.Status == KinectStatus.Connected)
            {
                using (SkeletonFrame skeletonFrame = this.ksensor.SkeletonStream.OpenNextFrame(1000))
                {
                    if (skeletonFrame != null)
                    {
                        count = this.CountSkeletons(skeletonFrame);
                    }
                }
            }
            else
            {
                throw new SensorNotFoundException("Kinect sensor not found");
            }

            return count;
        }

        public Position[] GetEntityPositions()
        {
            if (this.ksensor != null && this.ksensor.Status == KinectStatus.Connected)
            {
                SynchedFrames frames = this.PollSynchronizedFrames();
                if (frames.s_frame != null && frames.d_frame != null)
                {
                    int[] playerData = this.CalculatePlayerPositions(frames.s_frame, frames.d_frame);
                    Position[] entityPositions = new Position[frames.s_frame.SkeletonArrayLength];
                    for (int k = 0; k < entityPositions.Length; k++)
                    {
                        entityPositions[k].X = playerData[k * 3];
                        entityPositions[k].Y = playerData[(k * 3) + 1];
                        entityPositions[k].Depth = playerData[(k * 3) + 2];
                    }

                    return entityPositions;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                throw new SensorNotFoundException("Kinect sensor not found");
            }
        }

        public int GetMaxSensors()
        {
            return -1; // infinite
        }

        public void StopSensor(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
            }
        }

        private Joint BestTrackedJoint(Skeleton skeleton)
        {
            Joint joint = skeleton.Joints[JointType.Head];
            if (joint.TrackingState != JointTrackingState.Tracked)
            {
                joint = skeleton.Joints[JointType.ShoulderCenter];
                if (joint.TrackingState != JointTrackingState.Tracked)
                {
                    joint = skeleton.Joints[JointType.Spine];
                    if (joint.TrackingState != JointTrackingState.Tracked)
                    {
                        joint = skeleton.Joints[JointType.HipCenter];
                        if (joint.TrackingState != JointTrackingState.Tracked)
                        {
                            joint = skeleton.Joints[JointType.HandRight];
                            if (joint.TrackingState != JointTrackingState.Tracked)
                            {
                                joint = skeleton.Joints[JointType.HandLeft];
                                if (joint.TrackingState != JointTrackingState.Tracked)
                                {
                                    joint = skeleton.Joints[JointType.FootRight];
                                    if (joint.TrackingState != JointTrackingState.Tracked)
                                    {
                                        joint = skeleton.Joints[JointType.FootLeft];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return joint;
        }

        private int[] CalculatePlayerPositions(SkeletonFrame skeletonFrame, DepthImageFrame depthFrame)
        {
            Skeleton[] allskeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            int[] pointdata = new int[skeletonFrame.SkeletonArrayLength * 3];
            skeletonFrame.CopySkeletonDataTo(allskeletons);
            int k = 0;

            foreach (Skeleton skeleton in allskeletons)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Joint jointpoint = this.BestTrackedJoint(skeleton);
                    if (jointpoint.TrackingState == JointTrackingState.Tracked)
                    {
                        var depthImagePoint = depthFrame.MapFromSkeletonPoint(jointpoint.Position);
                        pointdata[k] = depthImagePoint.Depth;
                        pointdata[k + 1] = depthImagePoint.X;
                        pointdata[k + 2] = depthImagePoint.Y;
                    }
                    else
                    {
                        pointdata[k] = 0;
                        pointdata[k + 1] = 0;
                        pointdata[k + 2] = 0;
                    }
                }
                else
                {
                    pointdata[k] = 0;
                    pointdata[k + 1] = 0;
                    pointdata[k + 2] = 0;
                }

                k += 3;
            }

            return pointdata;
        }

        private int CountSkeletons(SkeletonFrame skeletonFrame)
        {
            int count = 0;
            Skeleton[] allskeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo(allskeletons);
            foreach (Skeleton skeleton in allskeletons)
            {
                if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                {
                    count++;
                }
            }

            return count;
        }

        private SynchedFrames PollSynchronizedFrames()
        {
            SynchedFrames frames = new SynchedFrames();
            Queue skeletonQueue = new Queue();
            Queue depthQueue = new Queue();
            long lowest = MAX_TIME_DIFFERENCE;
            for (int k = 0; k < QUEUE_MAX_LENGTH; k++)
            {
                skeletonQueue.Enqueue(this.ksensor.SkeletonStream.OpenNextFrame(100));
                depthQueue.Enqueue(this.ksensor.DepthStream.OpenNextFrame(100));
            }

            SkeletonFrame skeletonFrame = (SkeletonFrame)skeletonQueue.Dequeue();
            frames.s_frame = skeletonFrame;
            DepthImageFrame depthFrame = (DepthImageFrame)depthQueue.Dequeue();
            frames.d_frame = depthFrame;
            for (int l = 0; l < QUEUE_MAX_LENGTH - 1; l++)
            {
                if (skeletonFrame != null && depthFrame != null)
                {
                    long diff = Math.Abs(skeletonFrame.Timestamp - depthFrame.Timestamp);
                    if (diff == 0)
                    {
                        break;
                    }

                    if (diff < lowest)
                    {
                        lowest = diff;
                        frames.s_frame = skeletonFrame;
                        frames.d_frame = depthFrame;
                        if (skeletonFrame.Timestamp < depthFrame.Timestamp)
                        {
                            skeletonFrame = (SkeletonFrame)skeletonQueue.Dequeue();
                        }
                        else
                        {
                            depthFrame = (DepthImageFrame)depthQueue.Dequeue();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (skeletonFrame == null)
                    {
                        skeletonFrame = (SkeletonFrame)skeletonQueue.Dequeue();
                    }

                    if (depthFrame == null)
                    {
                        depthFrame = (DepthImageFrame)depthQueue.Dequeue();
                    }
                }
            }

            return frames;
        }

        private struct SynchedFrames
        {
            public DepthImageFrame d_frame;
            public SkeletonFrame s_frame;
        }
    }
}