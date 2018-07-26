using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Esri.FileGDB;


namespace Explorer
{
    /// <summary>
    /// 文件系统对象类型
    /// </summary>
    public enum FileSystemObjectType
    {
        Folder, File, GDB
    }

    /// <summary>
    /// 文件系统对象ViewModel
    /// </summary>
    public class FileSystemObjectViewModel : INotifyPropertyChanged
    {
        #region 静态成员

        /// <summary>
        /// 列举当前系统的所有磁盘目录
        /// </summary>
        /// <returns></returns> 
        public static FileSystemObjectViewModel GetSystemDrives()
        {
            var top = new FileSystemObjectViewModel();
            top._Children = new ObservableCollection<FileSystemObjectViewModel>();
            foreach (var drv in DriveInfo.GetDrives())
            {
                top._Children.Add(new FileSystemObjectViewModel(drv.RootDirectory.FullName, drv.ToString(), FileSystemObjectType.Folder, ""));
            }
            return top;
        }
        #endregion

        #region 构造函数


        /// <summary>
        /// 初始化一个标准ViewModel
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public FileSystemObjectViewModel(string path, string displayName, FileSystemObjectType type, string ppath)
        {
            Path = path;
            DisplayName = displayName; //显示出来的名字，加到整个的（包括类型扩展）的最后面
            Type = type;
            ParentPath = ppath;
            isSpecial = false;
            Initialize();
        }

        /// <summary>
        /// 初始化特殊ViewModel节点，用来代表一个文件夹拥有子成员
        /// </summary>
        private FileSystemObjectViewModel()
        {
            isSpecial = true;
        }

        #endregion

        #region 属性和字段
        bool isSpecial;

        public string Path { get; private set; }
        public string DisplayName { get; private set; }
        public string ParentPath { get; private set; }

        public FileSystemObjectType Type { get; private set; }
        public bool HasSpecialChild
        {
            get
            {
                return Children != null && Children.Count == 1 && Children[0].isSpecial;
            }
        }

        private ObservableCollection<FileSystemObjectViewModel> _Children;
        public ObservableCollection<FileSystemObjectViewModel> Children
        {
            get
            {
                return _Children;
            }
        }

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (value != _IsExpanded)
                {
                    _IsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                    OnExpanded();
                }
            }
        }

        #endregion

        #region 私有方法


        /// <summary>
        /// 初始化：检查文件夹是否有子成员，有的话加入特殊节点
        /// </summary>
        void Initialize()
        {
            if (Type == FileSystemObjectType.Folder && CheckChildObject())
            {
                if (ShowFile.check_gdb(Path))  //对于gdb文件夹
                {
                    Geodatabase geo = Geodatabase.Open(Path);
                    if (geo.GetChildDatasets(@"\", "Feature Class").Length != 0 || geo.GetChildDatasets(@"\", "Feature Dataset").Length != 0)
                    {
                        AddSpecialChild();
                    }
                    geo.Close();
                }
                else //对于普通文件夹
                {
                    AddSpecialChild();
                }
            }


            if (Type == FileSystemObjectType.GDB) //对于GDB内feature dataset的文件夹
            {
                Geodatabase geo = Geodatabase.Open(ParentPath);
                if (geo.GetChildDatasets(@Path, "Feature Class").Length != 0 || geo.GetChildDatasets(@Path, "Feature Dataset").Length != 0)
                {
                    AddSpecialChild();
                }
                geo.Close();
            }
        }

        /// <summary>
        /// 添加特殊特殊节点
        /// </summary>
        void AddSpecialChild()
        {
            _Children = new ObservableCollection<FileSystemObjectViewModel>();
            _Children.Add(new FileSystemObjectViewModel());
        }
        /// <summary>
        /// 移除特树节点
        /// </summary>
        void RemoveSpecialChild()
        {
            _Children.RemoveAt(0);
        }

        /// <summary>
        /// 枚举子文件，注意某些文件夹可能无法访问，所以用try, catch
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSubFiles()
        {
            try
            {
                return Directory.EnumerateFiles(Path);
            }
            catch
            {
                return new string[0];
            }
        }
        /// <summary>
        /// 枚举子文件夹，注意某些文件夹可能无法访问，所以用try, catch
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetSubDirs()
        {
            try
            {
                return Directory.EnumerateDirectories(Path);
            }
            catch
            {
                return new string[0];
            }
        }



        #endregion

        #region 派生可改写方法

        /// <summary>
        /// 节点被展开后的操作
        /// </summary>
        protected virtual void OnExpanded()
        {

            //是否有特殊节点
            if (HasSpecialChild)
            {
                //将要展开的节点拥有没有列举的子成员（第一次打开）

                //我们需要移除特殊节点，并将子文件夹加入到Children中
                RemoveSpecialChild();
                List<int> li = merge_file();
                if (ShowFile.check_gdb(Path))  //对于geodatabase文件夹的处理
                {
                    Geodatabase geo = Geodatabase.Open(Path);
                    foreach (var fc in geo.GetChildDatasets(@"\", "Feature Class"))
                        _Children.Add(new FileSystemObjectViewModel(fc, GetFileName(fc), FileSystemObjectType.File, Path));

                    foreach (var fds in geo.GetChildDatasets(@"\", "Feature Dataset"))
                    {
                        _Children.Add(new FileSystemObjectViewModel(fds, GetFileName(fds), FileSystemObjectType.GDB, Path));
                    }
                    geo.Close();
                }
                else if (Type == FileSystemObjectType.GDB) //对于geodatabase内feature dataset 的处理
                {
                    Geodatabase geo = Geodatabase.Open(ParentPath);
                    foreach (var fc in geo.GetChildDatasets(@Path, "Feature Class"))
                        _Children.Add(new FileSystemObjectViewModel(fc, GetFileName(fc), FileSystemObjectType.File, Path));

                    foreach (var fds in geo.GetChildDatasets(Path, "Feature Dataset"))
                    {
                        _Children.Add(new FileSystemObjectViewModel(fds, GetFileName(fds), FileSystemObjectType.GDB, Path));
                    }
                    geo.Close();
                }
                else //对于其他普通文件的处理
                {
                    foreach (var dir in GetSubDirs())

                        _Children.Add(new FileSystemObjectViewModel(dir, GetFileName(dir), FileSystemObjectType.Folder, ""));


                    IEnumerable<string> ss = GetSubFiles();
                    for (int i = 0; i < ss.Count(); i++)
                    {
                        bool b = false;
                        for (int pos = 0; pos < li.Count(); pos++)
                        {
                            if (li.ElementAt(pos) == i)
                            {
                                b = true;
                                break;
                            }
                        }
                        if (!b)
                        {
                            _Children.Add(new FileSystemObjectViewModel(ss.ElementAt(i), GetFileName(ss.ElementAt(i)), FileSystemObjectType.File, ""));
                        }
                    }
                }

            }
        }


        /// <summary>
        /// 检查当前文件夹是否有子成员（子文件夹或者子文件）
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckChildObject()
        {
            try
            {
                return Directory.EnumerateFileSystemEntries(Path).Any();
            }
            catch
            {
                return false;
            }
        }


        public static string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);   //保留扩展名


            //string name = System.IO.Path.GetFileName(path);  // 不保留扩展名
            // int ld = name.LastIndexOf(".");
            // if (ld < 0)
            // {
            //     return name;
            // }
            // return name.Substring(0, ld);
        }


        //返回文件名
        static string getname(string filename)
        {
            int ld = filename.LastIndexOf(".");
            if (ld < 0)
            {
                return filename;
            }
            return filename.Substring(0, ld);
        }


        //返回文件扩展名
        public static string getextension(string filename)
        {
            int ld = filename.LastIndexOf(".");
            if (ld < 0)
            {
                return "";
            }
            return filename.Substring(ld + 1, (filename.Length - ld - 1));
        }


        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        #endregion


        //返回指定文件类型的位置
        private static List<int> check_mex1(IEnumerable<string> ll)
        {
            int len = ll.Count();
            List<int> mex1pos = new List<int>();
            for (int i = 0; i < len; i++)
            {
                if (getextension(ll.ElementAt(i)) == ShowFile.mex1)
                {
                    mex1pos.Add(i);
                }

            }
            return mex1pos;
        }


        //如果可以隐藏，返回此filename的相同文件位置
        private static List<int> check_other(IEnumerable<string> ll, string filename)
        {
            bool imex2 = false;
            bool imex3 = false;
            int llen = ll.Count();
            List<int> pos = new List<int>();

            for (int i = 0; i < llen; i++)
            {
                string file = ll.ElementAt(i);
                if (getname(filename) == getname(file))
                {
                    if (ShowFile.check_extension(getextension(file)))
                    {
                        pos.Add(i);
                        continue;
                    }
                    if (getextension(file) == ShowFile.mex2)
                    {
                        imex2 = true;
                        pos.Add(i);
                        continue;
                    }
                    if (getextension(file) == ShowFile.mex3)
                    {
                        imex3 = true;
                        pos.Add(i);
                        continue;
                    }
                }
                if (getextension(file) == ShowFile.special)
                {
                    if (getname(filename) + ".shp" == getname(file))
                    {
                        pos.Add(i);
                        continue;
                    }
                }
            }
            if (!(imex2 && imex3))
            {
                pos.Clear();
            }
            return pos;
        }

        //返回需要隐藏的文件的位置
        private List<int> merge_file()
        {
            IEnumerable<string> fs = GetSubFiles();//Directory.EnumerateFiles(Path);
            int num = fs.Count();
            List<int> dpos = new List<int>();
            List<int> mex1p = check_mex1(fs);
            int mn = mex1p.Count();
            for (int i = 0; i < mn; i++)
            {
                string cfile = fs.ElementAt(mex1p.ElementAt(i));
                List<int> cr = check_other(fs, cfile);
                dpos.AddRange(cr);
            }
            return dpos;
        }
    }
}
