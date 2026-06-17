using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BarcodeLib;

using MessagingToolkit.QRCode.Codec;

using ZXing;

namespace FirstOhm
{
    class BarCodeLib
    {
        #region Bar Code
        /* 參考來源
        // http://einboch.pixnet.net/blog/post/264902951-%E5%88%A9%E7%94%A8barcodelib%E7%B0%A1%E5%96%AE%E5%89%B5%E5%BB%BA%E5%A4%9A%E7%A8%AE%E4%B8%80%E7%B6%AD%E6%A2%9D%E7%A2%BC%E5%9C%96%E5%BD%A2
        */

        public static string searchFromList(List<string> myList, string searchStr)
        {
            string result = myList.FirstOrDefault(x => x == searchStr);
            return result;
        }
        //開啟 winForm
        //使用方法 ： CommonClass.CheckWindowOpened(this, "_2_1APPS", MdiChildren)
        //使用前， 可將主menu Form 的 IsMDIContenner 改為 true, 可同時顯示多個子Form
        public static bool CheckWindowOpened(Form winForm, string ChildWindowName, Form[] MdiChildren)
        {
            bool Opened = true;
            winForm.LayoutMdi(MdiLayout.Cascade);
            for (int iChildren = 0; iChildren < MdiChildren.Length; iChildren++)
            {
                if (MdiChildren[iChildren].Name == ChildWindowName)
                {
                    //將視窗帶到到最上層
                    MdiChildren[iChildren].BringToFront();
                    Opened = false;
                }
            }
            return Opened;
        }

        public static Image genBarcode(String barcodeContent, int width = 0, int hight = 0, bool IncludeLabel = false)
        {
            Barcode bc = new Barcode();

            bc.IncludeLabel = IncludeLabel;//是否顯示標籤Label
            bc.LabelFont = new Font("Verdana", 8f);//標籤字型與大小
            bc.Width = width;//標籤寬度
            bc.Height = hight;//標籤高度
                              //編碼產生影像
            if (width == 0 || hight == 0)
            {
                bc.Width = barcodeContent.Length <= 4 ? 80 : barcodeContent.Length * 18;
                bc.Height = 23;
            }

            return bc.Encode(TYPE.CODE128, barcodeContent, bc.Width, bc.Height);
        }

        //Save as png File
        public static string genBarcodeAfJpg(String barcodeContent, string barcodeFilePath, int width = 0, int hight = 0, bool IncludeLabel = false, string extension = "png")
        {
            bool rtnBool = false;
            try
            {
                barcodeFilePath += DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + extension;
                Image barcodeImg = genBarcode(barcodeContent, width, hight, IncludeLabel);
                barcodeImg.Save(barcodeFilePath, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "genBarcodeAfJpg()");
                barcodeFilePath = null;
            }
            return barcodeFilePath;
        }
        ///===========================================================================================================
        //http://colinchu.pixnet.net/blog/post/118225374-qrcode-%E8%AE%80%E5%8F%96%E8%88%87%E5%AF%AB%E5%85%A5-%5Bc%23%5D
        public static System.Drawing.Bitmap genBarCode(String barcodeContent, int width = 20, int hight = 100)
        {
            //Use bitmap to storage qr-code
            System.Drawing.Bitmap bitmap = null;
            //let string to qr-code
            string strQrCodeContent = barcodeContent;

            ZXing.BarcodeWriter writer = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    //Create Photo 
                    Height = 30,
                    Margin = 5,
                    //Width = width,
                    CharacterSet = "UTF-8",
                    PureBarcode = true,
                    //錯誤修正容量
                    //L水平    7%的字碼可被修正
                    //M水平    15%的字碼可被修正
                    //Q水平    25%的字碼可被修正
                    //H水平    30%的字碼可被修正
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
                }
            };
            //Create Qr-code , use input string
            bitmap = writer.Write(strQrCodeContent);
            //Storage bitmpa
            //string strDir;
            //strDir = Directory.GetCurrentDirectory();
            //strDir += "\\temp.png";
            //bitmap.Save(strDir, System.Drawing.Imaging.ImageFormat.Png);
            ////Display to picturebox
            //pictureBox1.Image = bitmap;
            return bitmap;
        }

        //http://colinchu.pixnet.net/blog/post/118225374-qrcode-%E8%AE%80%E5%8F%96%E8%88%87%E5%AF%AB%E5%85%A5-%5Bc%23%5D
        public static System.Drawing.Bitmap genQRCode(String barcodeContent, int width = 100, int hight = 100)
        {
            //Use bitmap to storage qr-code
            System.Drawing.Bitmap bitmap = null;
            //let string to qr-code
            string strQrCodeContent = barcodeContent;

            ZXing.BarcodeWriter writer = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    //Create Photo 
                    Margin = 1,
                    Height = hight,
                    Width = width,
                    CharacterSet = "UTF-8",
                    PureBarcode = true, 
                    //錯誤修正容量
                    //L水平    7%的字碼可被修正
                    //M水平    15%的字碼可被修正
                    //Q水平    25%的字碼可被修正
                    //H水平    30%的字碼可被修正
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
                }
            };
            
            //Create Qr-code , use input string
            bitmap = writer.Write(strQrCodeContent);
            //Storage bitmpa
            //string strDir;
            //strDir = Directory.GetCurrentDirectory();
            //strDir += "\\temp.png";
            //bitmap.Save(strDir, System.Drawing.Imaging.ImageFormat.Png);
            ////Display to picturebox
            //pictureBox1.Image = bitmap;
            return bitmap;
        }

        //CodeType:QR or BAR
        //strDir -- preFix of the result file, 例如: C:\外銷\tempBarcode ==> 不要副檔名
        public static string generateBARCode(String CodeType, String barcodeContent, string strDir, float qrSize=1)
        {
            System.Drawing.Bitmap bitmap = null;
            int width = (int)(qrSize * 20);
            int hight = (int)(qrSize * 20);
            string extension = "bmp";
            try
            {
                if (CodeType == "QR")
                {
                    bitmap = genQRCode(barcodeContent, width, hight);
                }
                else
                {
                    bitmap = genBarCode(barcodeContent);
                }

                //Storage bitmpa
                strDir += DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + extension;
                bitmap.Save(strDir, System.Drawing.Imaging.ImageFormat.Png);
                return strDir;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool generateQRCode(String barcodeContent, string strDir, int width = 50, int hight = 50)
        {
            try
            {
                System.Drawing.Bitmap bitmap = genQRCode(barcodeContent, width, hight);
                //Storage bitmpa
                bitmap.Save(strDir, System.Drawing.Imaging.ImageFormat.Png);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string qrDecode(string qrFile)
        {
            //like before , create a bitmap 
            System.Drawing.Bitmap bitmap = null;
            //Create QRCode Reader Object
            ZXing.IBarcodeReader reader = new ZXing.BarcodeReader();

            FileStream file = new FileStream(qrFile, FileMode.Open);
            Byte[] data = new Byte[file.Length];
            file.Read(data, 0, data.Length);
            file.Close();
            MemoryStream ms = new MemoryStream(data);
            bitmap = (Bitmap)Image.FromStream(ms);
            //decode
            ZXing.Result decodeResult = reader.Decode(bitmap);

            return decodeResult.Text; //若無法讀取, 回傳 Null
        }
        #endregion

        #region Ronny Version 用這一版 QRcode 比較清晰
        //NUGET MessagingToolkit.QRCode.Codec
        //qrstring = QRCODE內文字、saveFile = 儲存路徑 、 qrcodeName = 檔案名稱
        public string getQrCode(string qrstring, string saveFile, string qrcodeName)
        {
            //第三方先產生bitmap
            QRCodeEncoder chtEncoder = new QRCodeEncoder();
            Bitmap qrBitmap1 = chtEncoder.Encode(qrstring);
            string qrsavefile = saveFile + @"\" + qrcodeName + ".bmp";
            qrBitmap1.Save(qrsavefile);
            return qrsavefile;
        } 
        #endregion
    }
}
