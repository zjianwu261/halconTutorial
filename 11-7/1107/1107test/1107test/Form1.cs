using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HalconDotNet;

namespace _1107test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        HObject ho_Image;
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //读取文件夹
            OpenFileDialog openfile = new OpenFileDialog();
            string str = "";
            if (openfile.ShowDialog() == DialogResult.OK && openfile.FileName != "")
            {
                str = openfile.FileName;
            }


            HOperatorSet.ReadImage(out ho_Image, str);

            //适应全屏
            HTuple hv_Width, hv_Height;
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            HOperatorSet.SetPart(hWindowControl1.HalconWindow, 0, 0, hv_Height - 1, hv_Width - 1);


            textBox2.Text = hv_Width.ToString();
            textBox3.Text = hv_Height.ToString();

            hWindowControl1.HalconWindow.DispObj(ho_Image);

        }


        /// <summary>
        /// 车牌识别
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            HObject ho_R, ho_G, ho_B;
            HObject ho_H, ho_S, ho_V;
         

            HOperatorSet.Decompose3(ho_Image, out ho_R, out ho_G, out ho_B);
            HOperatorSet.TransFromRgb(ho_R, ho_G, ho_B, out ho_H, out ho_S, out ho_V, "hsv");

            HObject ho_Region, ho_RegionFillUp;

            HOperatorSet.Threshold(ho_S, out ho_Region, 128, 255);
            HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);

            HTuple hv_Row, hv_Column, hv_Phi, hv_Length1, hv_Length2;

            HOperatorSet.SmallestRectangle2(ho_RegionFillUp, out hv_Row, out hv_Column, out hv_Phi,
                out hv_Length1, out hv_Length2);

            HObject ho_Rectangle;
            HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column, hv_Phi, hv_Length1,
                hv_Length2);

            HTuple hv_HomMat2D;
            HOperatorSet.VectorAngleToRigid(hv_Row, hv_Column, hv_Phi, hv_Row, hv_Column,
                0, out hv_HomMat2D);

            HObject ho_RegionAffineTrans, ho_ImageAffineTrans;
            HOperatorSet.AffineTransRegion(ho_Rectangle, out ho_RegionAffineTrans, hv_HomMat2D,
                "nearest_neighbor");
            HOperatorSet.AffineTransImage(ho_R, out ho_ImageAffineTrans, hv_HomMat2D, "constant",
                "false");

            //图像裁剪
            HObject ho_ImageReduced;
            HOperatorSet.ReduceDomain(ho_ImageAffineTrans, ho_RegionAffineTrans, out ho_ImageReduced);

            //提取白色区域
            HObject ho_Region1, ho_ConnectedRegions;
            HOperatorSet.Threshold(ho_ImageReduced, out ho_Region1, 100, 255);
            HOperatorSet.Connection(ho_Region1, out ho_ConnectedRegions);

            HObject ho_SelectedRegions;
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, (new HTuple("rect2_len1")).TupleConcat(
                "rect2_len2"), "and", (new HTuple(45)).TupleConcat(20), (new HTuple(55)).TupleConcat(
                30));

            HTuple hv_Number;
            HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);


            //
            HObject ho_SortedRegions;
            HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "first_point",
                "true", "column");

            //读取ocr
            HTuple hv_OCRHandle;
            HOperatorSet.ReadOcrClassMlp("Industrial_0-9A-Z_NoRej.omc", out hv_OCRHandle);

            //图像反转
            HObject ho_ImageInvert;
            HOperatorSet.InvertImage(ho_ImageReduced, out ho_ImageInvert);

            HTuple hv_Class, hv_Confidence;
            HOperatorSet.DoOcrMultiClassMlp(ho_SortedRegions, ho_ImageInvert, hv_OCRHandle,out hv_Class, out hv_Confidence);

            HTuple hv_FileHandle;
            HOperatorSet.OpenFile("1.txt", "output", out hv_FileHandle);
            HOperatorSet.FwriteString(hv_FileHandle, hv_Class);

            //识别结果显示
            string test = "";
            for (int i = 0; i < hv_Class.TupleLength(); i++)
            {
                test = test + hv_Class.TupleSelect(i);
            }
            textBox1.Text = test;


            //识别结果显示
            string test2 = "";
            for (int i = 0; i < hv_Confidence.TupleLength(); i++)
            {
                test2 = test2 + hv_Confidence.TupleSelect(i);
                test2 = test2 + "\t\n";
            }
            textBox4.Text = test2;


            MessageBox.Show("识别成功");

            //

        }
    }
}
