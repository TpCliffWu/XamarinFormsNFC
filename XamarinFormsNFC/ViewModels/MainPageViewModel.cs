using Plugin.NFC;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinFormsNFC.NFC;

namespace XamarinFormsNFC.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {

        public DelegateCommand ReadCommand { get; set; }
        public DelegateCommand WritingCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        public DelegateCommand SetCommand { get; set; }

        public const string MIME_TYPE = "application/com.tp.nfc";

        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            ReadCommand = new DelegateCommand(ReadNfcTag);
            WritingCommand = new DelegateCommand(WritingNfc);
            StopCommand = new DelegateCommand(Stop);
            SetCommand = new DelegateCommand(Set);
        }

        private void Stop()
        {
            NFCReader.Stop();
            NFCWriter.Stop();
        }


        /// <summary>
        /// 讀取NFC
        /// </summary>
        private async void ReadNfcTag()
        {
            if (await NFCSubscribe())
            {
                NFCReader.Start();
            }
        }

        /// <summary>
        /// 寫入NFC
        /// </summary>
        private async void WritingNfc()
        {
            if (await NFCSubscribe())
            {
                NFCWriter.Start();
            }
        }

        /// <summary>
        /// 模擬卡片
        /// </summary>
        private void Set()
        {
            Preferences.Set("nfc_text", NFCGlobal.Current.NfcContent);
        }

        private async Task<bool> NFCSubscribe()
        {
            if (CrossNFC.IsSupported)
            {
                if (!CrossNFC.Current.IsAvailable)
                {
                    await ShowAlert("NFC is not available");
                    return false;
                }


                var enable = CrossNFC.Current.IsEnabled;
                if (!enable)
                {
                    await ShowAlert("NFC is disabled");
                    NFCGlobal.Current.NfcFunctionRunning = true;
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}