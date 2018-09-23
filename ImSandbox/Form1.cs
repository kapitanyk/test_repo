using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using static OpenCvSharp.ML.StatModel;

namespace ImSandbox
{
    public partial class Form1 : Form
    {
        Image target_image;
        int width;
        int height;
        Bitmap original_Image { get; set; }
        Bitmap edges2 { get; set; }
        public static Bitmap MatToBitmap(Mat image)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        } // end of MatToBitmap function

        public static Mat BitmapToMat(Bitmap image)
        {
            return OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = new Bitmap(dlg.FileName);
                    System.Drawing.Image img = System.Drawing.Image.FromFile(dlg.FileName);
                    System.Drawing.Size size = img.Size;
                    pictureBox1.Size = size;
                    //pictureBox2.Size = size;
                    //MessageBox.Show("Width: " + img.Width + ", Height: " + img.Height);
                    // Add the new control to its parent's controls collection
                    //this.Controls.Add(pictureBox1);
                }
            }
            target_image = pictureBox1.Image;
            //target_image = Image.FromFile(@"D:\cv4sensorhub\onlab\DSC_0980.jpg");
            width = (target_image.Width)/20;
            height = (target_image.Height)/20;

            //pictureBox1.Image = target_image;
            Bitmap original_Image = (Bitmap)pictureBox1.Image.Clone();
            pictureBox2.Image = original_Image;
            pictureBox1.Image = ResizeNow(width, height);
        }

        private Bitmap ResizeNow(int target_width, int target_height)
        {
            Rectangle dest_rect = new Rectangle(0, 0, target_width, target_height);
            Bitmap destImage = new Bitmap(target_width, target_height);
            destImage.SetResolution(target_image.HorizontalResolution, target_image.VerticalResolution);
            using (var g = Graphics.FromImage(destImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapmode = new ImageAttributes())
                {
                    wrapmode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(target_image, dest_rect, 0, 0, target_image.Width, target_image.Height, GraphicsUnit.Pixel, wrapmode);
                }
            }
            return destImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap original_Image = (Bitmap)pictureBox2.Image;
            Mat mat = BitmapToMat(original_Image); //This is your Image converted to Mat
            Mat edges = new Mat();
            Cv2.Canny(mat, edges, 50, 100, 3, false);
            int count = edges.CountNonZero();
            //Console.WriteLine("Nonzero elements the edges:");
            //Console.WriteLine(count);
            Bitmap edges2 = MatToBitmap(edges);
            pictureBox2.Image = edges2;
            //PictureBox pictureBox3 = new PictureBox();
            //pictureBox3.Image = edges2;
            textBox2.Text = count.ToString();
            target_image = pictureBox2.Image;
            pictureBox2.Image = ResizeNow(width, height);
            //return edges2;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap edges2 = (Bitmap)pictureBox2.Image;
            //Mat mat = BitmapToMat(bmp_2);
            Mat mat = BitmapToMat(edges2);
            Cv2.ImWrite(@"D:\cv4sensorhub\onlab\DSC_0980_mod3.png",  mat);
        }

    }

        
}
