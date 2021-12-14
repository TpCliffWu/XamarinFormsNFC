using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinFormsNFC.Droid.NFC
{
    /// <summary>
    /// https://www.eftlab.com/knowledge-base/complete-list-of-apdu-responses/
    /// </summary>
    public class APDUResponse
    {
        /// <summary>
        /// [Info] Command successfully executed (OK).
        /// </summary>
        public static readonly byte[] A_OKAY = new byte[] { 0x90, 0x00 };

        public static readonly byte[] READ_CAPABILITY_CONTAINER_RESPONSE = new byte[] {
      0x00, 0x11, // CCLEN length of the CC file
      0x20, // Mapping Version 2.0
      0xFF, 0xFF, // MLe maximum
      0xFF, 0xFF, // MLc maximum
      0x04, // T field of the NDEF File Control TLV
      0x06, // L field of the NDEF File Control TLV
      0xE1, 0x04, // File Identifier of NDEF file
      0xFF, 0xFE, // Maximum NDEF file size of 65534 bytes
      0x00, // Read access without any security
      0xFF, // Write access without any security
      0x90, 0x00 // A_OKAY
       };


        /// <summary>
        /// File not found
        /// </summary>
        public static readonly byte[] A_ERROR = new byte[] {
            0x6A, // SW1	Status byte 1 - Command processing status
            0x82   // SW2	Status byte 2 - Command processing qualifier
         };

        public static readonly byte[] UNKNOWN_CMD_SW = new byte[] {
            0x00, // SW1	Status byte 1 - Command processing status
            0x00   // SW2	Status byte 2 - Command processing qualifier
         };


    }
}