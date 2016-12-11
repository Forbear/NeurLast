using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace NeurLast {
    class neurFirst {
        const int xCount = 30;
        const int inLayerCount = 5;
        const int hidenLayerCount = xCount * 2 / 3;
        const double E = 0.00001;
        public double correntE = 0;
        double[,] hidenWeightMass = new double[inLayerCount, hidenLayerCount];
        double[] exitWeightMass = new double[hidenLayerCount];
        double[] hidenThoughtMass = new double[hidenLayerCount];
        double[] xMass = new double[xCount];
        double[] weightedSumMass = new double[hidenLayerCount];
        double[] hidenYMass = new double[hidenLayerCount];
        double[] hidenShiftYMass = new double[hidenLayerCount];
        double[] exitYMass = new double[xCount - inLayerCount];
        public double exitStepLearn = 0;
        public double hidenStepLearn = 0;
        double exitT = 0;
        public double[] getXMass() {
            return xMass;
        }
        public double[] getYMass() {
            return exitYMass;
        }
        public double[] getExitYMass() {
            return exitYMass;
        }
        public double getE() {
            return correntE;
        }
        public int getInLayerCount() {
            return inLayerCount;
        }
        public int getHidenLayerCount() {
            return hidenLayerCount;
        }
        public int getXCount() {
            return xCount;
        }
        public void readAllParams() {
            StreamReader sr = new StreamReader("hidenWeights.txt");
            for(int i = 0; i < inLayerCount; i++)
                for(int j = 0; j < hidenLayerCount; j++) hidenWeightMass[i, j] = Convert.ToDouble(sr.ReadLine());
            sr.Close();
            sr = new StreamReader("exitWeights.txt");
            StreamReader srT = new StreamReader("hidenTs.txt");
            StreamReader srY = new StreamReader("exitY.txt");
            for(int i = 0; i < hidenLayerCount; i++) {
                exitWeightMass[i] = Convert.ToDouble(sr.ReadLine());
                hidenThoughtMass[i] = Convert.ToDouble(srT.ReadLine());
                exitYMass[i] = Convert.ToDouble(srY.ReadLine());
            }
            sr.Close();
            srT.Close();
            srY.Close();
            srT = new StreamReader("exitT.txt");
            exitT = Convert.ToDouble(srT.ReadLine());
            srT.Close();
            sr = new StreamReader("inParams.txt");
            for(int i = 0; i < xCount; i++) {
                xMass[i] = Convert.ToDouble(sr.ReadLine());
            }
            sr.Close();
        }
        public void writeAllWeights() {
            StreamWriter sw = new StreamWriter("hidenWeights.txt");
            for (int i = 0; i < inLayerCount; i++)
                for (int j = 0; j < hidenLayerCount; j++) sw.WriteLine(hidenWeightMass[i, j].ToString());
            sw.Close();
            sw = new StreamWriter("exitWeights.txt");
            StreamWriter swT = new StreamWriter("hidenTs.txt");
            for (int i = 0; i < hidenLayerCount; i++) {
                sw.WriteLine(exitWeightMass[i].ToString());
                swT.WriteLine(hidenThoughtMass[i].ToString());
            }
            sw.Close();
            swT.Close();
            sw = new StreamWriter("exitY.txt");
            for (int i = 0; i < exitYMass.Length; i++) sw.WriteLine(exitYMass[i].ToString());
            sw.Close();
            swT = new StreamWriter("exitT.txt");
            swT.WriteLine(exitT.ToString());
            swT.Close();
        }
        public void randomStartParams() {
            Random r = new Random();
            StreamWriter sw = new StreamWriter("hidenWeights.txt");
            for (int i = 0; i < inLayerCount; i++) 
                for (int j = 0; j < hidenLayerCount; j++) sw.WriteLine((r.NextDouble() / 10 - 0.05).ToString());
            sw.Close();
            sw = new StreamWriter("exitWeights.txt");
            StreamWriter swT = new StreamWriter("hidenTs.txt");
            for(int i =0; i < hidenLayerCount; i++) {
                sw.WriteLine((r.NextDouble() / 10 - 0.05).ToString());
                swT.WriteLine("1");
            }
            sw.Close();
            swT.Close();
            swT = new StreamWriter("exitT.txt");
            swT.WriteLine("1");
            swT.Close();
        }
        public void weightedSum(int step) {
            double sum = 0;
            for (int i = 0; i < hidenLayerCount; i++) {
                for (int j = 0; j < inLayerCount; j++) sum += hidenWeightMass[j, i] * xMass[j + step];
                weightedSumMass[i] = sum - hidenThoughtMass[i];
            }
        }
        public void hidenYDet() {
            for(int i = 0; i < hidenLayerCount; i++) hidenYMass[i] = Math.Log(weightedSumMass[i] + Math.Sqrt(weightedSumMass[i] * weightedSumMass[i] + 1));
        }
        public void hidenShiftYDet() {
            for(int i = 0; i < hidenLayerCount; i++)  hidenShiftYMass[i] = 1 / (Math.Sqrt(1 + weightedSumMass[i] * weightedSumMass[i]));
        }
        public void exitYDet(int step) {
            double sum = 0;
            for (int i = 0; i < hidenLayerCount; i++) sum += hidenYMass[i] * exitWeightMass[i];
            exitYMass[step] =  sum - exitT;
        }
        public void exitStepLearnDet(){
            double sum = 0;
            for (int i = 0; i < hidenLayerCount; i++) sum += hidenYMass[i] * hidenYMass[i];
            exitStepLearn = 1 / (1 + sum);
        }
        public void exitThoughtDet(int step) {
            exitT += exitStepLearn * (exitYMass[step] - xMass[step + inLayerCount]);
        }
        public void exitWeightDet(int step) {
            for (int i = 0; i < hidenLayerCount; i++) exitWeightMass[i] -= exitStepLearn * (exitYMass[step] - xMass[step + inLayerCount]) * hidenYMass[i];
        }
        public void hidenStepLearnDet(int step) {
            double sumOne = 0;
            double sumDuo = 0;
            double sumTre = 0;
            for(int i = 0; i < hidenLayerCount; i++) {
                sumOne += exitWeightMass[i] * hidenShiftYMass[i];
                sumTre += exitWeightMass[i] * exitWeightMass[i] * hidenShiftYMass[i] * hidenShiftYMass[i];
            }
            for (int i = 0; i < inLayerCount; i++) sumDuo += xMass[step + i];
            hidenStepLearn = sumOne / (sumDuo * sumTre); 
        }
        public void hidenThoughtDet(int step) {
            for (int i = 0; i < hidenLayerCount; i++)
                hidenThoughtMass[i] += hidenStepLearn * (exitYMass[step] - xMass[step + inLayerCount]) * hidenYMass[i] * exitWeightMass[i] * hidenShiftYMass[i];
        }
        public void hidenWeightDet(int step) {
            for (int i = 0; i < inLayerCount; i++)
                for (int j = 0; j < hidenLayerCount; j++)
                    hidenWeightMass[i, j] -= hidenStepLearn * (exitYMass[step] - xMass[step + inLayerCount]) * hidenYMass[j] * exitWeightMass[j] * xMass[step + i + inLayerCount] * hidenShiftYMass[j];
        }
        public bool failDet(int step) {
            double rez = 0.5 * (xMass[step + inLayerCount] - exitYMass[step]) * (xMass[step + inLayerCount] - exitYMass[step]);
            if (rez < E) return true;
            else return false;
        }
        public bool allFailDet() {
            double sum = 0;
            for (int i = 0; i < 20; i++) sum += (exitYMass[i] - xMass[i + inLayerCount]) * (exitYMass[i] - xMass[i + inLayerCount]);
            correntE = sum /= 2;
            if (sum < E) return true;
            else return false;
        }
    }
}
