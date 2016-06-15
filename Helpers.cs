/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adv
{
    public enum AlmanacStatus
    {
        Uncertain = 0,
        Good = 1,
        Certain = 2
    }

    public enum FixStatus
    {
        NoFix = 0,
        InternalTimeKeeping = 1,
        GFix = 2,
        PFix = 3
    }

    public enum AdvImageData
    {
        PixelDepth16Bit,
        PixelData12Bit,
        PixelDepth8Bit,
        PixelData24BitColor
    }

    /// <summary>
    /// Represents the ADV system time which is the number of milliseconds elapsed since: 1 Jan 2010, 00:00:00 GMT
    /// </summary>
    public struct AdvTimeStamp
    {
        public const long ADV_EPOCH_ZERO_TICKS = 633979008000000000;

        public long MillisecondsAfterAdvZeroEpoch;

        public static AdvTimeStamp FromWindowsTicks(long windowsTicks)
        {
            return new AdvTimeStamp()
            {
                MillisecondsAfterAdvZeroEpoch = (windowsTicks - ADV_EPOCH_ZERO_TICKS) / 10000
            };
        }

        public static AdvTimeStamp FromDateTime(DateTime dateTime)
        {
            return AdvTimeStamp.FromWindowsTicks(dateTime.Ticks);
        }

        public static AdvTimeStamp FromDateTime(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds)
        {
            return AdvTimeStamp.FromWindowsTicks(new DateTime(year, month, day, hours, minutes, seconds, milliseconds).Ticks);
        }
    }
}