using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Explorer

{
    class IconConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = (FileSystemObjectViewModel)value;
            if (vm.ParentPath == "")  // for general file type
            {
                return IconExtractor.GetIcon(vm.Path, true, vm.Type == FileSystemObjectType.Folder);
            }
            else // for gdb file type
            {
                return IconExtractor.GetIcon("/gdb", true, vm.Type == FileSystemObjectType.GDB);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
