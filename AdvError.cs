using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Adv
{
    public static class AdvError
    {
        public static int E_ADV_NOFILE                                     = unchecked((int)0x81000001);

        public static int E_ADV_STATUS_ENTRY_ALREADY_ADDED                 = unchecked((int)0x81001001);
        public static int E_ADV_INVALID_STATUS_TAG_ID                      = unchecked((int)0x81001002);
        public static int E_ADV_INVALID_STATUS_TAG_TYPE                    = unchecked((int)0x81001003);
        public static int E_ADV_STATUS_TAG_NOT_FOUND_IN_FRAME              = unchecked((int)0x81001004);
        public static int E_ADV_FRAME_STATUS_NOT_LOADED                    = unchecked((int)0x81001005);

        public static int S_OK                                             = 0;
        public static int E_FAIL                                           = unchecked((int)0x80004005);
        public static int E_NOTIMPL                                        = unchecked((int)0x80004001);


        public static void Check(int errorCode)
        {
            if (errorCode < 0)
                RaiseError(errorCode);
        }

        private static void RaiseError(int errorCode)
        {
            try
            {
                string errorMessage = string.Format("Error {0}. {1}", errorCode, ResolveErrorMessage(errorCode));

                try
                {
                    if (System.Windows.Forms.Application.OpenForms.Count > 0)
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
            // TODO:
            return string.Empty;
        }
    }
}
