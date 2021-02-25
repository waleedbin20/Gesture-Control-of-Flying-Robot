# I used JIRA to manage my (research) project during the CHALLENGE WEEK

All my evidences for each task will be uploaded here with snapshots!

#### How drones are being used in the industry for delivery purposes

There has been significant advancements in drone technology for parcel delivery. Depending on Some being remote while half of them autonomously. As you see in [2] for this the first group contains the coordinated use of trucks and UAVs. That is when together trucks and drones perform delivery services in a coordinated way. This was the first study during 2015 by Muray and Chu and they proposed two different methods of drone-assisted parcel delivery.

The second ones the works where drones are assisted by other vehicles to perform their services. In this case, drones are the only vehicles that bring packages to final deliveries, and the other vehicles are used as a launching platform, or to help the drones reach the desired targets. This was a recent study in 2018 where one drone can do multiple tasks and the truck could be the main service station for the launching pad of the drone.

The third group focuses on the works where drones work alone or with the only support of fixed infrastructure, like distribution centers or depots.  This study is being done by Chauhan et al (2018). There are various other examples that can be found in [3].

1. This is the evidence related to it
<p align="center">
    <img width="1000" height="350" src="/images/t1.png">
</p>

2. This evidence is also related to parcel delivery drone -  References are added realted to it 
<p align="center">
    <img width="1000" height="350" src="/images/t2.png">
</p>

#### Researched on how kinect sensor play an important role

Kinetic sensor was developed by Microsoft in their Xbox gaming console in 2012. By its low cost and high efficient rate we can now capture 3D motion without the need of controller. There has been 4 possible ways of using this sensor – 
* Object Tracking and Recognition 
* Human Activity Analysis 
* Hand gesture Analysis 
*Indoor 3D mapping.

More is talked about in [6][7]

3. This is the evidence for that
<p align="center">
    <img width="1000" height="350" src="/images/t4.png">
</p>

<p align="center">
    <img width="1000" height="350" src="/images/t6.PNG">
</p>

So I researched more into kinect sensor and found that there are twp versions currently in the market. V2 and V1 -- I went on and resarched more about the difference between them and they were seven turorials on microsoft which talks in depth the sensor. It's functionalities, how to program them (i.e outputting the images from the sensor to the monitor screenn). The body skeleton joints have been improved 25 joints are used to represent two people infront of them camera.

For my project i will be using Microsoft SDK 1.8 Xbox 360 kinect sensor to capture the skeleton gestures.


#### Different Gesture based projects and how computer vision plays an important role
There have been many different proposals on this.

In [4] they proposed a system which removed the use of gloves or remote controllers to control the drone. With the help of kinetic sensor and computer vision techniques they used specific parts of the body to extract gestures that are fed to the drone. The kinetic sensor used is because of its low cost and high efficient rate we can now capture 3D motion without the need of controller. The use of **computer vision** techniques rely on the ability of the drone’s camera to capture surrounding images and use pattern recognition to translate these images to meaningful commands. So the sensor is acting as gesture tracking device, which recognizes the gestures  and you can then control the quadrotor. The commands they tried to implement in [4] were: moving down, moving backward, left, right, take off and landing. Different approaches [5] have been taken like the use of skin color segmentation and using face two hands for gesture based control.

More about computer vision techniques used with Kinetic sensor is available in [6][7]. It talks about 3D model based and skeleton based for hand pose Estimation in [7]. In [8] they proposed a way of using Finger Earth’s movers distance for building robust part-based hand gesture recognition system using kinetic sensor. Because it has been a challenging part of entire human body, hand is a smaller object with a lot of complexity and segmentation errors 

In [9] a different technique was implemented a full skeleton based. This consists of 20points that are placed in body joints. Each joint was represented through X,Y,Z coordinates, so based on these coordinates of the joints the distance is found and the angles between the bones of the skeleton are determined. So for instance angle between the forearm and shoulder bone can be calculated on the basis of the determined coordinates of a number of selected gestures can be analyzed and by imposing certain conditions with respect to the parameters of the movement, they are accepted uniquely. They used kinetic sensor for capturing these gestures. So with these postures and gestures with specific commands can be sent to the quadrotor and it will execute as we say. They were able to make it fly right,  left forward, backward, upward and downward. Some other works that are related to this are in [10][11][12]. 


4. This is the evidence for Gesture based and a subtask for computer vision
<p align="center">
    <img width="1000" height="350" src="/images/t5.png">
</p>






5. Evidence for finding references

<p align="center">
    <img width="1000" height="350" src="/images/t3.png">
</p>




6. Programming control code for my gesture

<p align="center">
    <img width="700" height="350" src="/images/t7.png">
</p>


<p align="center">
    <img width="700" height="350" src="/images/t8.png">
</p>


7. Another Research fpr webots system done

<p align="center">
    <img width="700" height="350" src="/images/t9.png">
</p>


| References    |
| ------------- |

[2]	Riu - R, Monica, Mireia; Menendez, Monica (2019,  May) . “Logistic Deliveries With Drones State Of The Art Of Practice And Research”. [online] Research-collection.ethz.ch. Available at: https://www.research-collection.ethz.ch/bitstream/handle/20.500.11850/342823/RocaRiu_Menendez.pdf?sequence=1&isAllowed=y

[3]	Eitan Frachtengerg “Practical Drone Delivery - IEEE Journals & Magazine” Vol.52, 1558-0814, (Dec 2019)  [online]  Ieeexplore.ieee.org, Available at: https://ieeexplore.ieee.org/document/8909916

[4]	Andrea Sanna, Fabrizio Lamberti, Gianluca Paravati, Eduardo Andres Henao Ramirez, and Federico Manuri, “A kinetic based Natural Interface for Quadrotor Control”. In: Institue of Computer Science, Social Informatic and Telecommunication Engineering, vol.78, (2012) Available at: https://link.springer.com/chapter/10.1007/978-3-642-30214-5_6

[5]	Bhuiyan, M.A., Ampornaramveth, V., Muto, S., Ueno, H.: “Realtime vision based Gesture Recognition for Human Robot Interaction”. In: The IEEE International Conference on Robotics and Biometrics, pp. 413–418 (Aug 2004),  Available at: https://ieeexplore.ieee.org/abstract/document/1521814

[6]	Zhengyou Zhang “Microsoft Kinetic Sensor and its effect – IEEE Multimedia” Vol.19, 1941-0166, (Feb 2012)  [online]  Ieeexplore.ieee.org, Available at: https://ieeexplore.ieee.org/document/6190806/authors#authors

[7]	Dong Xu, Jamie Shotton, Ling Shao, Jungong Han “Enhanced Computer Vision with Microsoft Kinetic Sensor: A Review – IEEE Transactions on Cybernetics” Vol.43, 1941-0166, (Oct 2013)  [online]  Ieeexplore.ieee.org, Available at: https://ieeexplore.ieee.org/abstract/document/6547194/authors#authors

[8]	Zhou Ren, Junsong Yuan, Jingjing meng, Zhengyou zhang “Robust part-based hand gesture recognition system using kinetic Sensor: A Review – IEEE Transactions and Multimedia” Vol.15, 1941-0077, (Feb 2013)  [online]  Ieeexplore.ieee.org, Available at: https://ieeexplore.ieee.org/abstract/document/6470686/authors#authors

[9]	Vasil L popov, Kostadin B Shiev, Andon V Topalov, Nikola G Shakev, Sevil A Ahmed “Control of the flight of a small quadrotor using gesture interface” (Sep 2016)   – IEEE 8th International Conference on intelligent System [online]  Ieeexplore.ieee.org, Available at: https://ieeexplore.ieee.org/abstract/document/7737492/authors#authors

[10] Ahmed Mashood, Hassan Noura, Imad Jawhar, Nader Mohamed, “A gesture based kinetic for quadrotor control” (July 2015) – 2015 International Conference on Information and Communication Technology Research [online]  Ieeexplore.ieee.org, Available at: https://ieeexplore.ieee.org/document/7156481

[11] Sanders B., Vincenzi D., Shen Y. (2018) “Investigation of Gesture Based UAV Control”. In: Chen J. (eds) Advances in Human Factors in Robots and Unmanned Systems. AHFE 2017. Advances in Intelligent Systems and Computing, vol 595. Springer, Cham. https://doi.org/10.1007/978-3-319-60384-1_20

[12] Jon Bolin, Chad Crawford, William Macke, Jon Hoffman, Sam Beckmann, Sandip. Sen “Gesture based control of Autonomouse UAV” (May 2017) – Internationa Conference of Autonomous agent and Multiagent systems, Available at: http://www.ens.utulsa.edu/~sandip/research/web/papers/17Bolin.pdf



