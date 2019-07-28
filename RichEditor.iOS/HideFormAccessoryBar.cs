using System;
using System.Runtime.InteropServices;
using ObjCRuntime;

namespace Plugin.RichEditor.iOS
{
    public class HideFormAccessoryBar
    {
        static IntPtr _uiOriginalImp;
        static IntPtr _wkOriginalImp;
        static bool _hideFormAccessoryBar;

        [DllImport("/usr/lib/libobjc.dylib")]
        static extern IntPtr class_getInstanceMethod(IntPtr classHandle, IntPtr Selector);

        [DllImport("/usr/lib/libobjc.dylib")]
        static extern IntPtr method_getImplementation(IntPtr method);

        [DllImport("/usr/lib/libobjc.dylib")]
        static extern IntPtr imp_implementationWithBlock(ref BlockLiteral block);

        [DllImport("/usr/lib/libobjc.dylib")]
        static extern void method_setImplementation(IntPtr method, IntPtr imp);

        public static void SetHideFormAccessoryBar(bool hide)
        {
            if (hide == _hideFormAccessoryBar)
                return;
            var uiMethod = class_getInstanceMethod(Class.GetHandle("UIWebBrowserView"), new Selector("inputAccessoryView").Handle);
            var wkMethod = class_getInstanceMethod(Class.GetHandle("WKContentView"), new Selector("inputAccessoryView").Handle);

            if (hide)
            {
                _uiOriginalImp = method_getImplementation(uiMethod);
                _wkOriginalImp = method_getImplementation(wkMethod);

                var block_value = new BlockLiteral();
                CaptureDelegate d = MyCapture;
                block_value.SetupBlock(d, null);
                var nilimp = imp_implementationWithBlock(ref block_value);
                method_setImplementation(uiMethod, nilimp);
                method_setImplementation(wkMethod, nilimp);
            }
            else
            {
                method_setImplementation(uiMethod, _uiOriginalImp);
                method_setImplementation(wkMethod, _wkOriginalImp);
            }
            _hideFormAccessoryBar = hide;
        }

        delegate IntPtr CaptureDelegate();

        [MonoPInvokeCallback(typeof(CaptureDelegate))]
        static IntPtr MyCapture()
        {
            return IntPtr.Zero;
        }
    }
}