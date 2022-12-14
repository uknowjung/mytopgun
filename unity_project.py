import cv2
from cvzone.HandTrackingModule import HandDetector
import numpy as np
import socket
from thumb_classifier import thumb_classifier

# Thumb classifier
classifier = thumb_classifier()

# Webcam
capture = cv2.VideoCapture(0)

# Hand Detector
detector = HandDetector(maxHands=2, detectionCon=0.8)

# Communication(Python -> Unity)
# Use UDP
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

# Initialize turn and updown
turn = 0
updown = 0
is_velocity_handle_box = 0
velocity = 0
is_attack_mode = 0

while True:

    width = int(capture.get(cv2.CAP_PROP_FRAME_WIDTH))
    height = int(capture.get(cv2.CAP_PROP_FRAME_HEIGHT))
    velocity_box_width = int(width / 4)

    # Get the frame from the webcam
    success, img = capture.read()

    # Hands
    hands, img = detector.findHands(img)

    data = []
    # Landmark values - (x, y, z) * 21 -> make these single list
    if hands:
        if len(hands) == 2:
            print(velocity)
            # Get hands detected
            hand1 = hands[0]
            hand2 = hands[1]

            # Get the landmark list
            lmList1 = hand1['lmList']
            lmList2 = hand2['lmList']
            handType1 = hand1['type']
            handType2 = hand2['type']

            # Draw the line betwen two hands
            length_two_hadns, info, img = detector.findDistance(
                lmList1[0][:2], lmList2[0][:2], img)

            # Decide moving up or down
            # >0 means moving up and <0 means moving down
            center_two_hands = (lmList1[0][1] + lmList2[0][1]) / 2
            updown = int(height - center_two_hands - height * 0.5)

            if handType1 == 'Left' and handType2 == 'Right':
                # Decide turning left or right
                # <0 means turning left and >0 means turning right
                turn = int(lmList2[0][1] - lmList1[0][1])

                # If lm0(Wrist)'s x position is small than velocity_box_width, it's in 'velocity handle box'
                if lmList2[0][0] < velocity_box_width:
                    is_velocity_handle_box = 1
                    velocity = height - lmList2[0][1] # lm0's y position
                else:
                    is_velocity_handle_box = 0

                # Get width and height of box
                bbox1 = hand1['bbox']
                bbox_width = bbox1[2]
                bbox_height = bbox1[3]

                # Find distance between two landmarks, lm4(Thumb tip) and lm10(Middle finger pip)
                length, info, img = detector.findDistance(
                    lmList1[4][:2], lmList1[10][:2], img)

                is_attack_mode = classifier.classify(
                    np.array([bbox_width, bbox_height, length]))

                if is_attack_mode:
                    if is_velocity_handle_box:
                        print('Velocity Handle : ON \tAttack : ON  ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))
                    else:
                        print('Velocity Handle : OFF\tAttack : ON  ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))
                else:
                    if is_velocity_handle_box:
                        print('Velocity Handle : ON \tAttack : OFF ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))
                    else:
                        print('Velocity Handle : OFF\tAttack : OFF ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))

            elif handType1 == 'Right' and handType2 == 'Left':
                # Decide turning left or right
                # <0 means turning left and >0 means turning right
                turn = int(lmList1[0][1] - lmList2[0][1])

                # If lm0(Wrist)'s x position is small than velocity_box_width, it's in 'velocity handle box'
                if lmList1[0][0] < velocity_box_width:
                    is_velocity_handle_box = 1
                    velocity = height - lmList1[0][1] # lm0's y position
                else:
                    is_velocity_handle_box = 0

                # Get width and height of box
                bbox2 = hand2['bbox']
                bbox_width = bbox2[2]
                bbox_height = bbox2[3]

                # Find distance between two landmarks, lm4(Thumb tip) and lm10(Middle finger pip)
                length, info, img = detector.findDistance(
                    lmList2[4][:2], lmList2[10][:2], img)

                is_attack_mode = classifier.classify(
                    np.array([bbox_width, bbox_height, length]))

                if is_attack_mode:
                    if is_velocity_handle_box:
                        print('Velocity Handle : ON \tAttack : ON  ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))
                    else:
                        print('Velocity Handle : OFF\tAttack : ON  ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))
                else:
                    if is_velocity_handle_box:
                        print('Velocity Handle : ON \tAttack : OFF ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))
                    else:
                        print('Velocity Handle : OFF\tAttack : OFF ' +
                              '\tRight : ' + str(turn) + '\tUp : ' + str(updown))

            # Preprocessing data for unity and send it to socket
            if handType1 == 'Right':
                for lm in lmList1:
                    data.extend([lm[0], height - lm[1], lm[2]])
            elif handType2 == 'Right':
                for lm in lmList2:
                    data.extend([lm[0], height - lm[1], lm[2]])
            data.extend([is_velocity_handle_box, is_attack_mode, turn, updown, velocity])

            # [Right hand's 21 landmarks(x,y,z) data | Velocity?(0 or 1) | Attack?(0 or 1) | turn | updown | velocity]
            # Total length 68 -> 21x3 + 1 + 1 + 1 + 1 + 1
            sock.sendto(str.encode(str(data)), serverAddressPort)
    # Draw image
    # Blue box
    img = cv2.line(img, (velocity_box_width+20, 10),
                   (velocity_box_width+20, height-10), (0, 0, 255), 20)
    img = cv2.line(img, (velocity_box_width+20, height-10),
                   (width-20, height-10), (0, 0, 255), 20)
    img = cv2.line(img, (width, 10), (width-20, height-10), (0, 0, 255), 20)
    img = cv2.line(img, (velocity_box_width+20, 10),
                   (width-20, 10), (0, 0, 255), 20)

    # Red box
    img = cv2.line(img, (10, 10), (10, width-10), (255, 0, 0), 20)
    img = cv2.line(img, (10, height-10), (velocity_box_width, height-10), (255, 0, 0), 20)
    img = cv2.line(img, (velocity_box_width, 10),
                   (velocity_box_width, height-10), (255, 0, 0), 20)
    img = cv2.line(img, (10, 10), (velocity_box_width, 10), (255, 0, 0), 20)

    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)
    cv2.waitKey(1)
