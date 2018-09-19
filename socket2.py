import time
import socket

UDP_IP = "10.10.20.41"
UDP_PORT = 5065

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

while True:
    sock.sendto("{ \"type\": \"Dummy\" }", (UDP_IP, UDP_PORT))
    time.sleep(1)
