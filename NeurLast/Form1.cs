using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace NeurLast {
    public partial class Form1 : Form {
        neurFirst neurObj = new neurFirst();
        
        public Form1() {
            InitializeComponent();
        }
        void neurAlgorithm() {
            int count = 0;
            int hidenLayerCount = neurObj.getHidenLayerCount();
            int xCount = neurObj.getXCount();
            int inLayerCount = neurObj.getInLayerCount();
            double e = 0;
            neurObj.readAllParams();
            do {
                for (int i = 0; i < hidenLayerCount; i++) {
                    do {
                        neurObj.weightedSum(i);
                        neurObj.hidenYDet();
                        neurObj.hidenShiftYDet();
                        neurObj.exitYDet(i);
                        if (neurObj.failDet(i)) break;
                        neurObj.exitStepLearnDet();
                        neurObj.exitThoughtDet(i);
                        neurObj.exitWeightDet(i);
                        neurObj.hidenStepLearnDet(i);
                        neurObj.hidenThoughtDet(i);
                        neurObj.hidenWeightDet(i);
                        e = neurObj.getE();
                        count++;
                        if (label1.InvokeRequired) label1.Invoke(new MethodInvoker(delegate { label1.Text = count.ToString(); }));
                        if (label2.InvokeRequired) label2.Invoke(new MethodInvoker(delegate { label2.Text = e.ToString(); }));
                    } while (true);
                }
                if (neurObj.allFailDet()) break;
            } while (true);
            if (label2.InvokeRequired) label2.Invoke(new MethodInvoker(delegate { label2.Text = e.ToString(); }));
            neurObj.writeAllWeights();
            for(int i = hidenLayerCount; i < hidenLayerCount + inLayerCount; i++) {
                neurObj.weightedSum(i);
                neurObj.hidenYDet();
                neurObj.hidenShiftYDet();
                neurObj.exitYDet(i);
            }
        }
        void createChart() {
            
        }
        private void button1_Click(object sender, EventArgs e) {
            Thread neurAlgorithmThread = new Thread(neurAlgorithm);
            neurAlgorithmThread.Start();
        }

        private void button2_Click(object sender, EventArgs e) {
            neurObj.randomStartParams();
        }

        private void button3_Click(object sender, EventArgs e) {
            double[] massX = neurObj.getXMass();
            double[] massY = neurObj.getYMass();
            double min = Double.MaxValue;
            double max = Double.MinValue;
            for (int i = 0; i < massX.Length; i++) {
                chart1.Series["TrueValues"].Points.AddXY(i, massX[i]);
                if (!(i > massY.Length - 1)) {
                    if (min > massY[i]) min = massY[i];
                    if (max < massY[i]) max = massY[i];
                    chart1.Series["ProphetValues"].Points.AddXY(i + 5, massY[i]);
                }
                if (min > massX[i]) min = massX[i];
                if (max < massX[i]) max = massX[i];                
            }
            chart1.ChartAreas[0].AxisY.Maximum = max + max / 1000;
            chart1.ChartAreas[0].AxisY.Minimum = min - min / 1000;
        }
    }
}
