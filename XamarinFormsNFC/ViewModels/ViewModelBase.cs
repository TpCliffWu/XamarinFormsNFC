using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace XamarinFormsNFC.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, IApplicationLifecycleAware
    {
        protected INavigationService NavigationService { get; private set; }

        [Dependency]
        public IPageDialogService PageDialogService { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public async Task ShowAlert(string message, string title = "")
        {

            await this.PageDialogService.DisplayAlertAsync(title, message, "確認");
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        public virtual void OnResume()
        {
            //Restore the state of your ViewModel.
        }

        public virtual void OnSleep()
        {
            //Save the state of your ViewModel.
        }
    }
}
