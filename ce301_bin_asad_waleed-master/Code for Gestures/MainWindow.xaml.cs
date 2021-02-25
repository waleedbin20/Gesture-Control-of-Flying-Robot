//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using System.Numerics;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Width of output drawing
        /// </summary>
        private const float RenderWidth = 640.0f;

        /// <summary>
        /// Height of our output drawing
        /// </summary>
        private const float RenderHeight = 480.0f;

        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;

        /// <summary>
        /// Thickness of body center ellipse
        /// </summary>
        private const double BodyCenterThickness = 10;

        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;

        /// <summary>
        /// Brush used to draw skeleton center point
        /// </summary>
        private readonly Brush centerPointBrush = Brushes.Blue;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;


        /// <summary>
        /// Pen used for drawing bones that are currently tracked
        /// </summary>
        private readonly Pen trackedBonePen = new Pen(Brushes.Blue, 6);
        private readonly Pen matchBonePen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Drawing group for skeleton rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            sensor.Stop();
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            Image.Source = this.imageSource;

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                this.sensor.ColorFrameReady += Sensor_ColorFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                //this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }
        private static readonly int Bgr32BytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private byte[] pixelData;
        private WriteableBitmap CameraSource;
        private void Sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            Console.WriteLine("camera");
            bool receivedData = false;
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                if (imageFrame != null)
                {

                    if (pixelData == null)
                    {
                        this.pixelData = new byte[imageFrame.PixelDataLength];
                    }

                    imageFrame.CopyPixelDataTo(this.pixelData);
                    receivedData = true;

                    // A WriteableBitmap is a WPF construct that enables resetting the Bits of the image.
                    // This is more efficient than creating a new Bitmap every frame.
                    if (receivedData)
                    {
                        this.CameraSource = new WriteableBitmap(imageFrame.Width, imageFrame.Height, 96, 96, PixelFormats.Bgr32, null);

                        this.CameraSource.WritePixels(
                            new Int32Rect(0, 0, imageFrame.Width, imageFrame.Height),
                            this.pixelData,
                            imageFrame.Width * Bgr32BytesPerPixel,
                            0);
                        displayStream.Source = CameraSource;
                    }

                }
            }
        }




        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                if (skeletons.Length != 0)
                {
                    foreach (Skeleton skel in skeletons)
                    {
                        RenderClippedEdges(skel, dc);

                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }

                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            BodyCenterThickness,
                            BodyCenterThickness);
                        }
                    }
                }

                // prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }
        private static double GetDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            Console.WriteLine("angle for left arm strech left ->  " + AngleBetweenJoints(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.HipLeft]));
            Console.WriteLine("angle for left arm strech left Z axis ->  " + AngleBetweenJointsZ(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.HipLeft]));
            Console.WriteLine(skeleton.Joints[JointType.WristRight].Position.Z);
            double angleLeftStrech = AngleBetweenJoints(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.HipLeft]);
            double angleLeftStrechZ = AngleBetweenJointsZ(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.HipLeft]);
            double angleRightStrech = AngleBetweenJoints(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.HipRight]);
            double angleRightStrechZ = AngleBetweenJointsZ(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.HipRight]);
            double angleRightAnkle = AngleBetweenJoints(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.ShoulderRight]);
            double angleLeftAnkle = AngleBetweenJoints(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.ShoulderLeft]);
            double distanceR = GetDistance(skeleton.Joints[JointType.WristRight].Position.X, skeleton.Joints[JointType.WristRight].Position.Y, skeleton.Joints[JointType.ShoulderLeft].Position.X, skeleton.Joints[JointType.ShoulderLeft].Position.Y);
            double distanceL = GetDistance(skeleton.Joints[JointType.WristLeft].Position.X, skeleton.Joints[JointType.WristLeft].Position.Y, skeleton.Joints[JointType.ShoulderRight].Position.X, skeleton.Joints[JointType.ShoulderRight].Position.Y);
            double angleRightElbow = AngleBetweenJointsZ(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.ShoulderRight]);
            double angleLeftElbow = AngleBetweenJointsZ(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.ShoulderLeft]);
            Console.WriteLine("Ankle " + angleRightElbow);
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter, trackedBonePen);
           
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight, trackedBonePen);
            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft, trackedBonePen);
            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight, trackedBonePen);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight, trackedBonePen);
            if ((angleRightAnkle > 25 && angleRightAnkle < 60) && (angleLeftAnkle > 25 && angleLeftAnkle < 60))
            {
                this.lbl.Content = "Intial Start";



                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);

                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft,matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft,matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft,matchBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight,matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight,matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight,matchBonePen);

                

                


            }
            else if (angleRightAnkle > 65 && angleRightAnkle < 120 && distanceR < 0.19)
            {
                this.lbl.Content = "Right on shoulder";
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, matchBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, trackedBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, matchBonePen);
            }
            else if (angleLeftAnkle > 65 && angleLeftAnkle < 120 && distanceL < 0.19)
            {


                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, matchBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, trackedBonePen);

                this.lbl.Content = "left on shoulder";
            }
            else if (angleLeftStrech > 85 && angleLeftStrech < 110 && angleLeftStrechZ > 60)
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, matchBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, trackedBonePen);
                this.lbl.Content = "Left Arm Streched Left";
            }
            else if (angleLeftStrech > 75 && angleLeftStrech < 95 && angleLeftStrechZ < 60 && angleLeftElbow>110)
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, matchBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, trackedBonePen);
                this.lbl.Content = "Left Arm straight";
            }
            else if (angleLeftStrech > 135 && angleLeftStrech < 160)
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, matchBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, trackedBonePen);
                this.lbl.Content = "Left Up";
            }
            else if (angleRightStrech > 85 && angleRightStrech < 110 && angleRightStrechZ > 60)
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, matchBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, trackedBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, matchBonePen);
                this.lbl.Content = "Right Arm Streched Right";
            }
            else if (angleRightStrech > 75 && angleRightStrech < 95 && angleRightStrechZ < 60 && angleRightElbow>110)
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, trackedBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, matchBonePen);
                this.lbl.Content = "Right Arm straight";
            }
            else if (angleRightStrech > 135 && angleRightStrech < 160)
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, trackedBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, matchBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, matchBonePen);
                this.lbl.Content = "Right Up";
            }
            
            else
            {
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight, trackedBonePen);
                // Left Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft, trackedBonePen);

                // Right Arm
                this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight, trackedBonePen);
                this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight, trackedBonePen);
                this.lbl.Content = "";
            }



            //// Render Torso
            //this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            //this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            //this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            //this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            //this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            //this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            //this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            //// Left Arm
            //this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            //this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            //this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            //// Right Arm
            //this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            //this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            //this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            //// Left Leg
            //this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            //this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            //this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            //// Right Leg
            //this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            //this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            //this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }
        public static double AngleBetweenJoints(Joint j1, Joint j2, Joint j3)
        {
            double Angulo = 0;
            double shrhX = j1.Position.X - j2.Position.X;
            double shrhY = j1.Position.Y - j2.Position.Y;
            double shrhZ = j1.Position.Z - j2.Position.Z;


            double hsl = vectorNorm(shrhX, shrhY, shrhZ);
            double unrhX = j3.Position.X - j2.Position.X;
            double unrhY = j3.Position.Y - j2.Position.Y;
            double unrhZ = j3.Position.Z - j2.Position.Z;
            double hul = vectorNorm(unrhX, unrhY, unrhZ);
            double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;


            }
            else
                Angulo = 0;


            return Angulo;

        }

        public static double AngleBetweenJointsZ(Joint j1, Joint j2, Joint j3)
        {
            double Angulo = 0;
            double shrhX = j1.Position.X - j2.Position.X;

            double shrhZ = j1.Position.Z - j2.Position.Z;


            double hsl = vectorNormZ(shrhX, shrhZ);
            double unrhX = j3.Position.X - j2.Position.X;

            double unrhZ = j3.Position.Z - j2.Position.Z;
            double hul = vectorNormZ(unrhX, unrhZ);
            double mhshu = shrhX * unrhX + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;


            }
            else
                Angulo = 0;


            return Angulo;

        }

        private static double vectorNorm(double x, double y, double z)
        {

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

        }
        private static double vectorNormZ(double x, double z)
        {

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));

        }
        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1, Pen drawPen)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
           

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }


    }
}