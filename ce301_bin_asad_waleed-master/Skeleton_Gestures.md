## Detecting Skeleton Gesture using Microsoft Kinect V1

So Microsoft Kinect Sensor [1] was used as a gesture tracking device in this investigation. The recognized gestures and body postures are then used to send command to the Quadcoptor. 

After doing lots of research, I found out that the image of the skeleton consists of 20 points (placed in the body joints) that are connected with lines (links). The coordinates of each joint are available along the three axes X, Y, Z. The origin of the coordinate system is located in the image sensor of the color camera (Kinect CMOS color sensor). The joint orientations are preferred as they are more robust and are independent of the position and the height of the gesturer
Given the coordinates of the joints, the distance between them can be determined and the angles between the bones (links) of the skeleton can be determined. 

This was an appropriate approach to it. So for example if i wanted to find an angle between my elbow and forearm, that can be found using the (x,y) coordinates of the joints and a certain number of selected gesture can be analyzed by imposing some certain conditions with respect to paramters of the movement, they are accepted uniqely.

#### Streaming Data out of Kinect Sesnor

Downloading Microsoft windows SDK has allowed me to get all the necessary C# libraries that are needed for the application program. To start off with first step was to connect kinect sesnor because it is compulsory to find out which application program must first determine from which device it will receive information. 

Then for mine I had to check the status of kinect itself -- I used this function which looks through all the sensors and starts the first connected one from the microsoft toolkit. It is compulsory that kinect must be plugged in. 

foreach (var potentiaSensor in Kinect.status.Connected()){
    if (potentialSensor.Status == KinectStatus.Connected){
        this.sensor == potentialSensor;
        break;
    }
}
If the value of the variable sensor is **not null**, the program will execute the method **start** and it enables (SkeletonStream) method which will allow to receive skeleton frames, otherwise an appropriate exception will be throwed. 

Visualization of the information from the Skeleton streams is done with specific events for example *SkeletonFrameReady*. The function **SensorSkeletonFrameReady** is created and is called every time when a ready frame with information appears. This allows to track in real time the changes of the values of coordinates for the selected points. Inside the above function multiple objects of the type Skeleton are created, which contain necessary information about the skeleton. A variable *skeletonFrame* from the type SkeletonFrame is also created, which contains frames with the skeleton. The OpenSkeletonFrame method is used to open these frames, which contains one frame of skeleton data. It is checked whether skeletonFrame is not with a null value. Then the information from these frames is copied into the created array of the type Skeleton. In this way for every frame there exists information about a skeleton with 20 connected points (joints).

#### Finding the Point Coordinates and the Angles Between the Links of the Skeleton

1. Desired angles and coordinates can be accessed inside the function SensorSkeletonFrameReady.
2. Find the coordinates of a desired point it has to be selected from the list of the joints JointType.
3. Create three float variables to store X,Y,Z values
4. So with the help of this coordinates the angles between skeleton links connected with a joint can be determined.

The function Tracker connects the points in order to visualize the built skeleton. The skeleton consists of five parts: torso, left arm, right arm, left leg and right leg. The connection of the points (joints) can be implemented by specifying the couples of points that have to be connected by a link. Kinect is capable to calculate approximately the positions of skeleton parts that are not visible. The forecast is based on the available information about the visible parts of the skeleton.




