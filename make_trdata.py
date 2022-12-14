import cv2
import numpy as np
import pandas as pd
from cvzone.HandTrackingModule import HandDetector
import socket

# Parameters
width, height = 1280, 720

# Webcam
capture = cv2.VideoCapture(0)
capture.set(3, width)
capture.set(4, height)

# Hand Detector
detector = HandDetector(maxHands=1, detectionCon=0.8)

# Communication(Python -> Unity)
# Use UDP
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

length_bootstrap = False
length_threshold = 0
df_index = -100
df = pd.DataFrame()

while True:
    # Get the frame from the webcam
    success, img = capture.read()

    # Hands
    hands, img = detector.findHands(img)

    data = []
    # Landmark values - (x, y, z) * 21 -> make these single list
    if hands:
        # Get the first hand detected
        hand1 = hands[0]

        # Get the landmark list
        lmList1 = hand1['lmList']
        handType1 = hand1['type']

        # If lm0(Wrist)'s x position is smaller than 600, it's in 'velocity handle box'
        if handType1 == 'Right':
            bbox1 = hand1['bbox']
            bbox_width = bbox1[2]
            bbox_height = bbox1[3]

            if lmList1[0][0] < 600:
                is_velocity_handle_box = True
            else:
                is_velocity_handle_box = False
            # print(is_velocity_handle_box)

            # Find distance between two landmarks, lm4(Thumb tip) and lm10(Middle finger pip)
            length, info, img = detector.findDistance(
                lmList1[4][:2], lmList1[10][:2], img)
            if length_bootstrap is False:
                length_bootstrap = True
                length_threshold = length
            if length > length_threshold:
                is_attack_mode = True
            else:
                is_attack_mode = False
            # print(length_threshold, length, is_attack_mode)
            # print(
            #     f'Velocity handle box : {is_velocity_handle_box}, Attack mode : {is_attack_mode}')
        # Make 1D list of 21 landmarks(only one hand)
        # print(lmList1)
        for lm in lmList1:
            data.extend([lm[0], height - lm[1], lm[2]])

        sock.sendto(str.encode(str(data)), serverAddressPort)

        # # Make data for training classifier
        tr_data = np.array([bbox_width, bbox_height, length, is_attack_mode])
        # print(tr_data)
        if df_index >= 0:
            df[df_index] = tr_data
        df_index += 1
        print(df)
        if df_index == 500:
            df.to_csv("training_data_up.csv", index=False)
    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)
    cv2.waitKey(1)
