from sklearn.neighbors import KNeighborsClassifier
import pandas as pd
import numpy as np


class thumb_classifier:
    def __init__(self):
        # Classifier for right thumb
        self.classifier = KNeighborsClassifier(n_neighbors=3)

        df1 = pd.read_csv('training_data_1.csv')
        df1 = df1.transpose()
        df1 = df1.to_numpy()
        df1_y = []
        for i in range(len(df1)):
            df1_y.append(int(df1[i][-1]))
        df1_x = np.delete(df1, 3, axis=1)

        df2 = pd.read_csv('training_data_2.csv')
        df2 = df2.transpose()
        df2 = df2.to_numpy()
        df2_y = []
        for i in range(len(df2)):
            df2_y.append(int(df2[i][-1]))
        df2_x = np.delete(df2, 3, axis=1)

        self.tr_data_y = np.append(df1_y, df2_y)
        self.tr_data_x = np.append(df1_x, df2_x, axis=0)

        self.classifier.fit(self.tr_data_x, self.tr_data_y)

    def classify(self, test_data):
        result = self.classifier.predict(test_data.reshape(1, -1))
        return result[0]