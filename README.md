# Radify
## Radify is a program for windows, developed with C#, that allows you to work with category 10 and category 21 messages, version 0.23, 0.26 and 2.1 of the eurocontrol Asterix system.

The program allows you to load all the .ast files you want, and sort them by time. You can see the data of all the uploaded files organized by tables, search in those tables, sort them, and export them.

You can also see the messages loaded on a map, ordered by the radar that detected them. The program is developed for the Spanish airspace, so in category 10 messages it automatically detects which airport it is by looking at an internal list that contains the SICs of the country's radar systems, and makes the conversion from Cartesian to geocentric coordinates correctly , using the GeoUtils library.

On the map you can see the vehicles, see the information of each one of them by clicking on it, measure distances between vehicles or between a vehicle and a point, move from a vehicle on the map to the message in the tables to see all the information, export the Vehicle trajectories to KML to view them in Google Earth, view the history of vehicle positions, and play with the simulation time.

### ScreenShots

#### Intro of the program
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Intro.PNG)

#### Load page. In load we can choose which categories to load, and the day of the file
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Load.PNG)

#### Tables view. We can see all the messages information, see messages on map, order by columns, search for messages, or export the tables to csv.
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Tables.PNG)

#### Map view. We can see the vehicles of different color depending on the radar that detected them, and with different shape depending on whether they are surface vehicles, airplanes, or indeterminate. We can also filter which vehicles we want to see. In addition, each vehicle is identified with its label
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Map1.PNG)

#### By clicking on a marker we can see its information. We can also see your position history 
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Map2.PNG)

#### We can also see the position history of all vehicles
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Map3.PNG)

#### We can measure distances between vehicles, or vehicles and points on the map
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Map4.PNG)

#### We can also change from map to satellite view
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Map5.PNG)

#### Trajectories exported to kml with the program. Each trajectory contains a description of the vehicle. In addition, they are differentiated between those detected by SMR, those detected by MLAT and those detected by ADS-B, are painted in different colors, and are organized by folders to be able to filter them also in google earth
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Google%20Earth2.PNG)

![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/GoogleEarth.PNG)

#### Help page. Clicking on each of the controls appears an explanation of what that control is for
![ScreenSHot](https://github.com/SergiBago/Radify/blob/master/Images/Help.PNG)

