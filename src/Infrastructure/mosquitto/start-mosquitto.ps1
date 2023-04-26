# First we build a custom Mosquitto docker image (based on the official eclipse image) that 
# contains the custom configuration file we need for Dapr traffic-control (see mosquitto.conf).

docker build -t dapr-trafficcontrol/mosquitto:1.0 .
docker run -d -p 1323:1323 -p 3121:3121 --name plc-mosquitto dapr-trafficcontrol/mosquitto:1.0
