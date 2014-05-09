using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonTools
{
    public class BusinessTools
    {
        /// <summary>
        /// 截取字节数组的元素;
        /// </summary>
        /// <param name="bytes">源字节数组</param>
        /// <param name="index">起始位置</param>
        /// <param name="count">截取的个数</param>
        /// <returns></returns>
        public static byte[] SubBytesArray(byte[] bytes, int index, int count)
        {
            byte[] b = new byte[count];
            for (int i = index; i < count + index; i++)
            {
                b[i - index] = bytes[i];
            }

            return b;

        }

        /// <summary>
        /// 初始化字节数组;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="length"></param>
        public static void InitializeByteArray(byte[] a, int length)
        {
            byte[] t = Encoding.UTF8.GetBytes(" ");
            for (int i = 0; i < length; i++)
            {
                a[i] = t[0];
            }
        }

        /// <summary>
        /// 使用字符串给字节数组赋值;
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void SetByteArray(byte[] a, string b)
        {
            byte[] t = Encoding.UTF8.GetBytes(b);
            for (int i = 0; i < t.Length; i++)
            {
                a[i] = t[i];
            }
        }

        /// <summary>
        /// 获取文件来源或保存路径
        /// </summary>
        /// <param name="whichBank"></param>
        /// <returns></returns>
        public static string GetFilePath(string whichBank)
        {
            string fileFromPath = "";
            switch (whichBank)
            {
                case "01":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_01"];
                    break;
                case "02":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_02"];
                    break;
                case "03":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_03"];
                    break;
                case "04":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_04"];
                    break;
                case "05":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_05"];
                    break;
                case "11":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_11"];
                    break;
                case "19":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_19"];
                    break;
                case "31":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_31"];
                    break;
                case "32":
                    fileFromPath = ConfigurationManager.AppSettings["gjjFilePath_32"];
                    break;
                default:
                    break;
            }

            return fileFromPath;
        }

        /// <summary>
        /// 模拟产生批次号
        /// </summary>
        /// <param name="codeHead"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GenerateBatchCode(string codeHead, int i)
        {
            if (i < 10)
            {
                return codeHead + "0" + i.ToString();
            }
            else
            {
                return codeHead + i.ToString();
            }

        }

        /// <summary>
        /// 模拟产生姓名
        /// </summary>
        /// <param name="nameHead"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public  static string GenerateName(string nameHead, int i)
        {
            string[] nameDetail = {"一","二","三","四","五","六","七","八","九","十","十一","十二","十三","十四",
                                      "十五","十六","十七","十八","十九","二十" };
            return nameHead + nameDetail[i];
        }

        /// <summary>
        /// 模拟产生银行账号
        /// </summary>
        /// <param name="head"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GenerateBankCount(string head, int i)
        {
            if (i < 10)
            {
                return head + "0" + i.ToString();
            }
            else
            {
                return head + i.ToString();
            }
        }

        /// <summary>
        /// 模拟产生银行流水号
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string GenerateBankSerialNum(int i)
        {
            if (i < 1)
                return "0" + i.ToString();

            return i.ToString();
        }
    }
}
