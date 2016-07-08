using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace Adv
{
    [Serializable]
    public class AdvCoreException : Exception
    {
        public AdvCoreException()
        { }

        public AdvCoreException(string message)
            : base(message)
        { }

        public AdvCoreException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    public static class AdvError
    {
        public static bool ShowMessageBoxErrorMessage = false;
        public static bool ThrowError = true;

        public const int E_ADV_NOFILE                                           = unchecked((int)0x81000001);
        public const int E_ADV_IO_ERROR                                         = unchecked((int)0x81000002);

        public const int E_ADV_STATUS_ENTRY_ALREADY_ADDED                       = unchecked((int)0x81001001);
        public const int E_ADV_INVALID_STATUS_TAG_ID                            = unchecked((int)0x81001002);
        public const int E_ADV_INVALID_STATUS_TAG_TYPE                          = unchecked((int)0x81001003);
        public const int E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME                    = unchecked((int)0x81001004);
        public const int E_ADV_FRAME_STATUS_NOT_LOADED                          = unchecked((int)0x81001005);
        public const int E_ADV_FRAME_NOT_STARTED                                = unchecked((int)0x81001006);
        public const int E_ADV_IMAGE_NOT_ADDED_TO_FRAME                         = unchecked((int)0x81001007);
        public const int E_ADV_INVALID_STREAM_ID                                = unchecked((int)0x81001008);
        public const int E_ADV_IMAGE_SECTION_UNDEFINED                          = unchecked((int)0x81001009);
        public const int E_ADV_STATUS_SECTION_UNDEFINED                         = unchecked((int)0x8100100A);
        public const int E_ADV_IMAGE_LAYOUTS_UNDEFINED                          = unchecked((int)0x8100100B);
        public const int E_ADV_INVALID_IMAGE_LAYOUT_ID                          = unchecked((int)0x8100100C);
        public const int E_ADV_CHANGE_NOT_ALLOWED_RIGHT_NOW                     = unchecked((int)0x8100100D);
        public const int E_ADV_IMAGE_SECTION_ALREADY_DEFINED                    = unchecked((int)0x8100100E);
        public const int E_ADV_STATUS_SECTION_ALREADY_DEFINED                   = unchecked((int)0x8100100F);
        public const int E_ADV_IMAGE_LAYOUT_ALREADY_DEFINED                     = unchecked((int)0x81001010);
        public const int E_ADV_INVALID_IMAGE_LAYOUT_TYPE                        = unchecked((int)0x81001011);
        public const int E_ADV_INVALID_IMAGE_LAYOUT_COMPRESSION                 = unchecked((int)0x81001012);
        public const int E_ADV_INVALID_IMAGE_LAYOUT_BPP                         = unchecked((int)0x81001013);

        public const int S_OK                                                   = 0;
        public const int S_ADV_TAG_REPLACED                                     = 0x71000001;

        public const int E_FAIL                                                 = unchecked((int)0x80004005);
        public const int E_NOTIMPL                                              = unchecked((int)0x80004001);


        public static bool Check(int errorCode)
        {
            if (errorCode < 0)
            {
                string errorMessage;
                if (errorCode == E_ADV_IO_ERROR)
                {
                    int systemErrorCode = AdvLib.GetLastSystemSpecificFileError();
                    errorMessage = string.Format("I/O Error {0}.", systemErrorCode);
                }
                else
                    errorMessage = string.Format("Error {0}. {1}", errorCode, ResolveErrorMessage(errorCode));

                RaiseError(new StackFrame(1).ToString() + "\r\n" + errorMessage);

                if (ThrowError)
                    throw new AdvCoreException(errorMessage);

                return false;
            }
            else
                return true;
        }

        public static void HandleException(Exception ex)
        {
            RaiseError(ex.GetFullStackTrace());

            if (ThrowError)
                throw new AdvCoreException("Unhandled error has occured in AdvLib.", ex);
        }

        private static void RaiseError(string errorMessage)
        {
            try
            {
                try
                {
                    if (ShowMessageBoxErrorMessage && 
                        System.Windows.Forms.Application.OpenForms.Count > 0)
                    {
                        // Windows forms application
                        System.Windows.Forms.MessageBox.Show(
                            errorMessage, "AdvLib",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error);

                        return;
                    }
                }
                catch { }

                //try
                //{
                //    if (System.Windows.Application.Current != null)
                //    {
                //        // WPF application
                //        System.Windows.MessageBox.Show(
                //            errorMessage, "AdvLib",
                //            System.Windows.MessageBoxButton.OK,
                //            System.Windows.MessageBoxImage.Error);

                //        return;
                //    }
                //}
                //catch { }

                Trace.WriteLine("AdvLib: " + errorMessage);
                Console.WriteLine("AdvLib: " + errorMessage);
                Console.Error.WriteLine("AdvLib: " + errorMessage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

        private static string ResolveErrorMessage(int errorCode)
        {
            switch (errorCode)
            {
                case E_ADV_STATUS_ENTRY_ALREADY_ADDED:
                    return "This status TagId has been already added to the current frame.";
               case E_ADV_INVALID_STATUS_TAG_ID:
                    return "Unknown status TagId";         
               case E_ADV_INVALID_STATUS_TAG_TYPE:
                    return "The type of the status TagId doesn't match the currently called method.";
               case E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME:
                    return "The requested status TagId is not present in the current frame.";
               case E_ADV_FRAME_STATUS_NOT_LOADED:
                    return "No status has been loaded. Call GetFramePixels() first.";
               case E_ADV_FRAME_NOT_STARTED:
                    return "Frame not started. Call BeginFrame() first.";
               case E_ADV_IMAGE_NOT_ADDED_TO_FRAME:
                    return "No image has been added to the started frame.";
               case E_ADV_INVALID_STREAM_ID:
                    return "Invalid StreamId. Must be 0 for MAIN or 1 for CALIBRATION.";
               case E_ADV_IMAGE_SECTION_UNDEFINED:
                    return "No Image Section has been defined.";
               case E_ADV_STATUS_SECTION_UNDEFINED:
                    return "No Status Section has been defined.";
               case E_ADV_IO_ERROR:
                    return "An I/O error has occured.";
               case E_ADV_INVALID_IMAGE_LAYOUT_ID:
                    return "Invalid image LayoutId.";
               case E_ADV_CHANGE_NOT_ALLOWED_RIGHT_NOW:
                    return "This change is not allowed on an existing file or once a frame insertion has started.";
               case E_ADV_IMAGE_SECTION_ALREADY_DEFINED:
                    return "The Image Section can be only defined once per file";
               case E_ADV_STATUS_SECTION_ALREADY_DEFINED:
                    return "The Status Section can be only defined once per file";
               case E_ADV_IMAGE_LAYOUT_ALREADY_DEFINED:
                    return "An Image Layout with this LayoutId has been already defined.";
               case E_ADV_INVALID_IMAGE_LAYOUT_TYPE:
                    return "Invalid Image Layout type. Accepted values are FULL-IMAGE-RAW, 12BIT-IMAGE-PACKED and 8BIT-COLOR-IMAGE.";
               case E_ADV_INVALID_IMAGE_LAYOUT_COMPRESSION:
                    return "Invalid Image Layout compression. Accepted values are UNCOMPRESSED, LAGARITH16 and QUICKLZ.";
               case E_ADV_INVALID_IMAGE_LAYOUT_BPP:
                    return "Invalid Image Layout bits per pixel value. Accepted range is from 1 to 32.";

               case E_FAIL:
                    return "Error.";
               case E_NOTIMPL:
                    return "Not Implemented.";


               case S_OK:
                    return "Success.";                          
               case S_ADV_TAG_REPLACED:
                    return "An existing tag with the same name has been replaced."; 


                default:
                    return null;
            }
        }
    }
}
