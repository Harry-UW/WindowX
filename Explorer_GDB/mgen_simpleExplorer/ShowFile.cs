using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esri.FileGDB;

namespace Explorer
{
    class ShowFile
    {
        public static string mex1 = "shp"; //合并后被现实的后缀名（三个必须存在的后缀名）
        public static string mex2 = "dbf";
        public static string mex3 = "shx";
        private static string[] extension_in = { "cpg", "prj", "sbn", "sbx" }; //需要被合并显示的后缀名
        private static  int ex_in_num = 4; //需要被合并显示的后缀名数量
        public static string special = "xml";

        //查看是否需要被合并
        public static bool check_extension(string ex) 
        {
            for(int i = 0; i < ex_in_num; i++)
            {
                if(extension_in[i] == ex)
                {
                    return true;
                }
            }
            return false;
        }


        //查看是否是geodatabase类型
        public static bool check_gdb(string path)
        {
            if (FileSystemObjectViewModel.getextension(FileSystemObjectViewModel.GetFileName(path)) != "gdb")
            {
            return false;
            }
            
            try
            {
                Geodatabase geo = Geodatabase.Open(@path);
                geo.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        
   


        








        //public string[] check_samename(string[] sl, int len)
       // {
       //     string[] sn;
        //    int pos = 0;
        //    for (int i = 0; i < len - 1; i++)
        //    {
           //     for (int j = i + 1; j < len; j++)
        ////        {
         //           if(sl[i] == sl[j])
        //            {
          //              bool exist = false;
        //                for (int k = 0; k < pos; k++)
         //               {
        //                    if (sn[k] == sl[i])
       //                     {
          //                      exist = true;
            //                    break;
              //              }
                //        }
                  //      if (!exist)
                    //    {
                      ///      sn[pos] = sl[i];
                         //   pos++;
                        //}
     //               }
       //         }
         //   }
           // return sn;
     //   }

       // public int[] check_aex(string[] se, string name, int len)
    //    {
        //    for (int i = 0; i < len; i++)
      //      {

        //    }
       // }
    }
}
