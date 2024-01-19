using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BottomSheetMaui.Sample
{
    public class Popup<T> : Popup where T : Popup<T>, new()
    {
        private static T instance;

        public static T Cache()
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }

    public class Popup : ContentPage
    {
        public bool IsFadeBackground { get; set; } = true;
        public bool RemoveBottomSafeArea { get; set; } = true;

        private ImageButton _backgroundBack;
        private static bool _isBusy;

        public Popup()
        {
            this.BackgroundColor = Color.FromArgb("#01000000");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (IsFadeBackground)
                _backgroundBack.FadeTo(.4, 500);
        }

        protected async override void OnParentChanged()
        {
            base.OnParentChanged();
            var cache = this.Content;

            _backgroundBack = new ImageButton()
            {
                BackgroundColor = IsFadeBackground ? Colors.Black : Colors.Transparent,
                Opacity = IsFadeBackground ? 0 : 1
            };
            var bugdroid = new TapGestureRecognizer();
            cache.GestureRecognizers.Add(bugdroid);

            _backgroundBack.Clicked -= OnCloseBackgroundClicked;
            _backgroundBack.Clicked += OnCloseBackgroundClicked;

            await Task.Delay(100).ConfigureAwait(true);

            var grid = new Grid() { _backgroundBack, cache };

#if IOS || MACCATALYST
            if (RemoveBottomSafeArea)
            {
                var safeAreaInsets = Microsoft.Maui.ApplicationModel.WindowStateManager.Default.GetCurrentUIWindow().SafeAreaInsets;
                grid.Margin = new Thickness(0, 0, 0, -safeAreaInsets.Bottom);
            }
#endif
            this.Content = grid;
        }

        void OnCloseBackgroundClicked(object sender, EventArgs args)
        {
            if (IsCloseOnBackgroundClick)
                Close().ConfigureAwait(true);
        }

        public TaskCompletionSource<object> CallBackResult = new();

        public bool IsCloseOnBackgroundClick { get; set; } = true;

        protected override bool OnBackButtonPressed()
        {
            if (!IsCloseOnBackgroundClick)
                return true;

            Close().ConfigureAwait(true);
            return true;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async virtual Task BeforeOpen()
        {
            //max delay 500
        }

        public async virtual Task AfterOpen()
        {

        }

        public async virtual Task BeforeClose()
        {
            //max delay 400
        }

        public async virtual Task AfterClose()
        {

        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public static async Task<T> Open<T>(Popup page) where T : new()
        {
            if (_isBusy) return default(T); _isBusy = true;
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await page.BeforeOpen().ConfigureAwait(true);
                    try
                    {
                        await Application.Current.MainPage.Navigation.PushModalAsync(page, false).ConfigureAwait(true);
                    }
                    catch (Exception e)
                    {
                        //Config.LogService.Error(e.StackTrace);
                    }

                    await page.AfterOpen().ConfigureAwait(true);
                }

                _isBusy = false;
                return (T)await page.CallBackResult.Task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _isBusy = false;
                //Config.LogService.Error(ex.StackTrace);
                return default(T);
            }
        }

        public static async Task<string> Open(Popup page)
        {
            if (_isBusy) return null; _isBusy = true;
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await page.BeforeOpen().ConfigureAwait(true);
                    try
                    {
                        await Application.Current.MainPage.Navigation.PushModalAsync(page, false).ConfigureAwait(true);
                    }
                    catch (Exception e)
                    {
                        //Config.LogService.Error(e.StackTrace);
                    }

                    await page.AfterOpen().ConfigureAwait(true);
                }

                _isBusy = false;

                return (string)await page.CallBackResult.Task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                _isBusy = false;
                //Config.LogService.Error(ex.StackTrace);
                return string.Empty;
            }

        }

        public static async Task Close(object returnValue = null)
        {
            if (Application.Current?.MainPage != null && Application.Current.MainPage.Navigation.ModalStack.Count > 0)
            {
                var modalStack = Application.Current.MainPage.Navigation.ModalStack;
                Popup currentPage = (Popup)modalStack[modalStack.Count - 1];

                await currentPage.BeforeClose().ConfigureAwait(true);

                if (currentPage.IsFadeBackground)
                    await currentPage._backgroundBack.FadeTo(0, 400).ConfigureAwait(true);

                currentPage?.CallBackResult.TrySetResult(returnValue);

                try
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync(false).ConfigureAwait(true);
                }
                catch (Exception e)
                {
                    //Config.LogService.Error(e.StackTrace);
                }

                await currentPage.AfterClose().ConfigureAwait(true);
                _isBusy = false;
            }
        }
    }
}
