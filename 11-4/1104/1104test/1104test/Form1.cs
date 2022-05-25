using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

//添加引用
using HalconDotNet;

namespace _1104test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 图像处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            HObject ho_Image, ho_GrayImage, ho_Region;
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Region);



            HOperatorSet.ReadImage(out ho_Image, "C:/Users/haokunlee/Desktop/《halcon机器视觉教程》素材及程序/11-4/素材.png");
            HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
            HOperatorSet.Threshold(ho_GrayImage, out ho_Region, 128, 255);


            hWindowControl1.HalconWindow.DispObj(ho_GrayImage);
            hWindowControl1.HalconWindow.DispObj(ho_Region);

            ho_Image.Dispose();
            ho_GrayImage.Dispose();
            ho_Region.Dispose();

        }
    }
}
