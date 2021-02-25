# Gesture Based Control of an Unmanned Aerial Vehicle

This document is about Controlling multirotor drone (i.e Mavic Drone in Webots Simulator) using Gesture based. The key criteria is to bring up a **unique** way of conrtolling it and provide a good Human Computer interaction!


### Summary
In the recent years Unmanned aerial Vehicles (i.e drones) are becoming popular for different applications in the industry, research and military areas specially. The drone technology is valued for completing tasks, especially data collection, more cheaply than existing technologies, in remote areas without risking human lives. Moreover because of It’s low cost and high efficiency countries out there have mastered the UAV technologies and the drones have rapidly grown in the market. Some countries like United Arab Emirates have planned on delivering documents using the UAV. One of the company in India has started delivering pizza using UAV, Aamzon Prime and UPS are using multirotor drone to deliver their parcels

### In the Past

With all the tasks a UAV can do their has been a difficult user interface as you need to be specialized in controlling it using aerial remote control, or a software which an highly skilled expert has programmed it (like a mobile app). This manual control of a flying system such as an X4 flyer with a joystick needs several hours of training before being able to succeed.

### What has changed

However in an era of rapidly changing UAV technologies this situation has now been developed with gesture based control. We are now able to control these drones by doing specific body gestures (arm movement etc.) so that drone can take off, land etc.  Normal people can quickly adapt to this method and this resembles as a good human interaction with a computer.

### Real life Examples
* It can help the fire department by the drone being able to be sent into a building that they are unsure if its stable enough to send a person into, as the drone is small and will have little impact on the surroundings and provide a visual feed. The controls mean that it could be operated by any of the members of the crew and would not need to have a specialist to do so, the controls could also be used while wearing gloves so they would not need to take extra time in deploying the device. 

* It could be helpful in search and rescue operations that have been affected by natural disasters as well as those that human inflicted. It could help the crew team to find out missing people with the help of drones on board camera which uses facial verification technology. Although there are still some limitations due to efficiency on the cameras quality and the robustness. 

* A UAV can provide surveillance and assess hazardous situations from the air by its airborne sensors. Or, a group of UAV(s) can fly to remote locations and deploy sensors that can send information to land robots to perform long term surveillance from the ground.


### Ongoing Projects

MIT is using the Dragon flyer RC helicopter in their Swarm Health Management project. Which is focuses on surveillance and monitoring of ground based objects or vehicles. The goal is to continuously monitor using multiple autonomous vehicles in swarms, with distributed intelligent computer control and minimal human supervision.


### What I want to achieve

After reading all these research papers I want to do a full body skeleton system because this will allow me to have many gestures for the control of the drone. The sensor I want to use is  Kinetic because it provides different options such as 1. Object Tracking and Recognition 2. Human Activity Analysis 3. Hand gesture Analysis 4.Indoor 3D mapping. So I want to add these gestures and body postures which will allow me to remote control the drone. It also has the useful libraries OPEN NI (which is available on all platforms) which will help tracking of the user. In the body skeleton I want to add simple gesture like for example you can see in the table below. I want to implement, take off , landing, moving right, moving left, moving front and backwards. I will also identify a gesture by tracking the position of the skeleton and comparing the position made with previous frames if the position has been held for a certain number of frames within an acceptable variance the drone will then be sent the command and will execute said order.

| Gestures                         |Commands      | 
| -------------------------------- |:-------------:|
| Hand fists on respected shoulder      | Initial Position | 
| Left arm on right shoulder      | Take off | 
| Right arm on left shoulder     | Land     |   
| Left arm stretched left | Move Left     |
| Right arm stretched right     | Move right| 
| Left arm above head      | Move forward      |   
| Right arm above head | Move backward      |
| Left arm straight      | Move up | 
| Right arm straight       | Move down| 


#### These are the pose gestures

<p align="center">
    <img width="1000" height="450" src="/images/gestures.png">
</p>

Using this type of sensor will allow normal people benefit from it because a lot of freedom will be provided. Like no one has to be expert in using the joystick to control the drone. Some real world applications like in Natural Disaster or a fire department team or checking pipelines, oil tanks where humans life matters a lot, with the help of these gesture they will be able to control it.

Keeping all this in mind most importantly the reason I chose these gesture and this project specifically is because I want normal people to get effected with this type of system positively. My gestures are very easy and anyone can perform them. It requires less energy and a good Human Interaction with computer, that’s all it matters to me. As an Engineer my job is to get a good client satisfaction. The world is in a fast paced environment and technologies have been arising daily. 


### Risks involved in this project

As we know that there are  many safety risks involved when working with Unmanned Aerial Vehicles. Any small faults( i.e drone falling from air) can lead into a serious injury, because it has sharp blades on its rotor. I will have to be cautious when I am performing my tests making sure I am an indoor environment with one staff present with me. When drone takes off I need to be at least 1.5m away from it. I have to make sure that the batteries are fully charged so that it does not crash/fall out of the sky. I have to think about making the drone hover at a specific place if the kinetic sensor fails to detect my gestures, or making sure it lands smoothly to the ground with low speed through the backup commands sent, so it does not damage from anywhere.


### What changed

Due to the current pandemic I will not be testing on the real hardware. So instead the gestures will be tested on webot software which will be the same as on real hardware. The software contains a mavic Drone 2 and its control code is written in C so, I will have to use that and mix it with my kinect code.

This will remove the safety risks concerned but one should be careful if ther were to test on the real hardware


