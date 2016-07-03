using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adv
{
    public static class AdvErrorCodes
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
    }
}
