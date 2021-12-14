using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace XamarinFormsNFC.Droid
{
    public static class Helpers
    {
		public static byte[] StringToByteArray(this string hex)
		{
			return Enumerable.Range(0, hex.Length / 2).Select(x => Byte.Parse(hex.Substring(2 * x, 2), NumberStyles.HexNumber)).ToArray();
		}
	}
}