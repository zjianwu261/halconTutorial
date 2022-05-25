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

        string str = "";

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



            //string str="C:/Users/haokunlee/Desktop/《halcon机器视觉教程》素材及程序/11-4/素材.png";


            HOperatorSet.ReadImage(out ho_Image,str);
            HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
            HOperatorSet.Threshold(ho_GrayImage, out ho_Region, 128, 255);

            //
            HTuple hv_Width, hv_Height;
            HOperatorSet.GetImageSize(ho_GrayImage, out hv_Width, out hv_Height);
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);


            hWindowControl1.HalconWindow.DispObj(ho_GrayImage);
            hWindowControl1.HalconWindow.DispObj(ho_Region);

            ho_Image.Dispose();
            ho_GrayImage.Dispose();
            ho_Region.Dispose();

           
        }

        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            HObject ho_Image;
            //C#获取文件中图片路径
            OpenFileDialog openfile = new OpenFileDialog();
            
            if (openfile.ShowDialog() == DialogResult.OK && openfile.FileName != "")
            {
                str = openfile.FileName;
            }

            MessageBox.Show("图片读取成功");
            HOperatorSet.ReadImage(out ho_Image, str);

            HTuple hv_Width, hv_Height;
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);

            hWindowControl1.HalconWindow.DispObj(ho_Image);
        }
    }
}


