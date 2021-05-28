using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Abot2.Tintin247
{
    public class StringHelper
    {
        public static string RemoveMultiSpace(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                const RegexOptions options = RegexOptions.None;
                var regex = new Regex("[ ]{2,}", options);
                str = regex.Replace(str, " ");
                return str.Trim();
            }
            return null;
        }

        public static string ConvertShortName(string strVietNamese)
        {
            if (string.IsNullOrEmpty(strVietNamese)) return "";
            char[] delimiter = { ':', '?', '"', '/', '!', ',', '-', '=', '%', '$', '&', '*', '.' };
            strVietNamese = RemoveMultiSpace(strVietNamese);
            strVietNamese = strVietNamese.Replace("'", "");
            string[] subString = strVietNamese.Split(delimiter);
            strVietNamese = "";
            foreach (var t in subString)
            {
                strVietNamese += t;
            }
            //Loại bỏ tiếng việt
            const string textToFind =
                " áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string textToReplace =
                "-aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index;
            while ((index = strVietNamese.IndexOfAny(textToFind.ToCharArray())) != -1)
            {
                int index2 = textToFind.IndexOf(strVietNamese[index]);
                strVietNamese = strVietNamese.Replace(strVietNamese[index], textToReplace[index2]);
            }

            return strVietNamese.ToLower();
        }

        public static string StripHTML(string htmlString)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }
    }
}
