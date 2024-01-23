using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottomSheetMaui
{
    public class BottomSheet
    {
         public static Task<T> Open<T>(BottomSheetView view) where T : new()
         {
             return Popup.Open<T>(new BaseBottomSheet(view));
         }

         public static Task<string> Open(BottomSheetView view)
         {
             return Popup.Open(new BaseBottomSheet(view));
         }

         public static Task Close(object returnValue = null)
         {
             return Popup.Close(returnValue);
         }
     }

        public class BottomSheetView : ContentView
        {
            public Command CallBackReturn;

            public BottomSheetView()
            {
            }
    }
}

