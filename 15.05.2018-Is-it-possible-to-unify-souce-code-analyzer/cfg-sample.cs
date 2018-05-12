using System;
using System.Text;
using System.Web.UI;


namespace PocExample
{
    public partial class Default : Page
    {
        protected void Page_Load(object s, EventArgs e)
        {
            if (name == "admin")
            {
                if (key1 == "validkey")
                {
                    str1 = UTF8.GetString(data);
                }
                else
                {
                    str1 = "Wrong key!";
                }
            }
            else
            {
                str1 = "Wrong role!";
            }

            Response.Write(str1);
        }
    }
}