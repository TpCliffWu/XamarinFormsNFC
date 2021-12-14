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
    /// https://cardwerk.com/smart-card-standard-iso7816-4-section-5-basic-organizations/
    /// https://en.wikipedia.org/wiki/Smart_card_application_protocol_data_unit
    /// </summary>
    public class APDUCommand
    {

        public static readonly byte[] Hello = new byte[]
        {
            0x60
        };


        /// <summary>
        /// CLA - (1) -  Class of instruction
        /// INS - (1) - Instruction code
        /// P1 -  (1) - Instruction parameter 1
        /// P2 -  (1) - Instruction parameter 2 
        /// Lc - (0,1,3) - field	-Number of bytes present in the data field of the command
        /// Data field- (Lc) -	 -String of bytes sent in the data field of the command
        /// Le field - (0,1,3) --Maximum number of bytes expected in the data field of the response to the command
        /// </summary>

        public static readonly byte[] SELECT_APDU_HEADER = new byte[] {
            0x00,  //CLA
            0xA4,  //INS
            0x04,  //P1
            0x00   //P2
        };

        /// <summary>
        /// D2 
        /// 76
        /// 00
        /// 00
        /// 85
        /// 01
        /// 01
        /// </summary>
        public static readonly byte[] APDU_SELECT = new byte[] {
           0x00, // CLA	- Class - Class of instruction
           0xA4, // INS	- Instruction - Instruction code
           0x04, // P1	- Parameter 1 - Instruction parameter 1
           0x00, // P2	- Parameter 2 - Instruction parameter 2
           0x07, // Lc field	- Number of bytes present in the data field of the command
           0xD2,
           0x76,
           0x00,
           0x00,
           0x85,
           0x01,
           0x01, // NDEF Tag Application name
           0x00  // Le field	- Maximum number of bytes expected in the data field of the response to the command
        };

        public static readonly byte[] CAPABILITY_CONTAINER_OK = new byte[] {
            0x00, // CLA	- Class - Class of instruction
            0xA4, // INS	- Instruction - Instruction code
            0x00, // P1	- Parameter 1 - Instruction parameter 1
            0x0C, // P2	- Parameter 2 - Instruction parameter 2
            0x02, // Lc field	- Number of bytes present in the data field of the command
            0xE1, 0x03 // file identifier of the CC file
        };


        public static readonly byte[] READ_CAPABILITY_CONTAINER = new byte[] {
            0x00, // CLA	- Class - Class of instruction
            0xB0, // INS	- Instruction - Instruction code
            0x00, // P1	- Parameter 1 - Instruction parameter 1
            0x00, // P2	- Parameter 2 - Instruction parameter 2
            0x0F  // Lc field	- Number of bytes present in the data field of the command
        };



        public static readonly byte[] NDEF_READ_BINARY = new byte[] {
            0x00, // Class byte (CLA)
            0xb0 // Instruction byte (INS) for ReadBinary command
        };

        public static readonly byte[] NDEF_READ_BINARY_NLEN = new byte[] {
            0x00, // Class byte (CLA)
            0xB0, // Instruction byte (INS) for ReadBinary command
            0x00, 
            0x00, // Parameter byte (P1, P2), offset inside the CC file
            0x02  // Le field
        };

        public static readonly byte[] NDEF_ID = new byte[] {
            0xE1,
            0x04
        };




        public static readonly byte[] NDEF_SELECT_OK = new byte[] {
            0x00, // CLA	- Class - Class of instruction
            0xA4, // Instruction byte (INS) for Select command
            0x00, // Parameter byte (P1), select by identifier
            0x0C, // Parameter byte (P1), select by identifier
            0x02, // Lc field	- Number of bytes present in the data field of the command
            0xE1, 
            0x04 // file identifier of the NDEF file retrieved from the CC file
        };
    }
}