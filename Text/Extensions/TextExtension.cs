using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VSSystem.Text.Extensions
{
    public static class TextExtension
    {
        public static string ToAlphaNumericOnly(this string input)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(input, "");
        }
        public static string ReplaceUnicodeString(this string input)
        {
            try
            {
                string[][] unicodeChars =
                {
                    new string[]{"á","à","ạ","ã","ả","ă","ắ","ằ","ặ","ẵ","ẳ","â","ấ","ầ","ậ","ẫ","ẩ"},
                    new string[]{"Á","À","Ạ","Ã","Ả","Ă","Ắ","Ằ","Ặ","Ẵ","Ẳ","Â","Ấ","Ầ","Ậ","Ẫ","Ẩ"},
                    new string[]{"é","è","ẹ","ẽ","ẻ","ê","ế","ề","ệ","ễ","ể"},
                    new string[]{"É","È","Ệ","Ễ","Ể","Ê","Ế","Ề","Ệ","Ễ","Ể"},
                    new string[]{"í","ì","ị","ĩ","ỉ"},
                    new string[]{"Í","Ì","Ị","Ĩ","Ỉ"},
                    new string[]{"ó","ò","ọ","õ","ỏ","ô","ố","ồ","ộ","ỗ","ổ","ơ","ớ","ờ","ợ","ỡ","ở"},
                    new string[]{"Ó","Ò","Ọ","Õ","Ỏ","Ô","Ố","Ồ","Ộ","Ỗ","Ổ","Ơ","Ớ","Ờ","Ợ","Ỡ","Ở"},
                    new string[]{"ú","ù","ụ","ũ","ủ","ư","ứ","ừ","ự","ữ","ử"},
                    new string[]{"Ú","Ù","Ụ","Ũ","Ủ","Ư","Ứ","Ừ","Ự","Ữ","Ử"},
                    new string[]{"ý","ỳ","ỵ","ỹ","ỷ"},
                    new string[]{"Ý","Ỳ","Ỵ","Ỹ","Ỷ"},
                    new string[]{"đ"},
                    new string[]{"Đ"}
                };
                string[] UNSIGNED_UNI_char = { "a", "A", "e", "E", "i", "I", "o", "O", "u", "U", "y", "Y", "d", "D" };
                //
                StringBuilder strB = new StringBuilder(input);
                for (int i = 0; i < unicodeChars.Length; i++)
                {
                    for (int j = 0; j < unicodeChars[i].Length; j++)
                        strB.Replace(unicodeChars[i][j], UNSIGNED_UNI_char[i]);
                }
                return strB.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
