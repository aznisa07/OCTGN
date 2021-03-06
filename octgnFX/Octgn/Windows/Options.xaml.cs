﻿namespace Octgn.Windows
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;

    using Octgn.Core;
    using Octgn.Library;
    using Octgn.Library.Exceptions;

    public partial class Options
    {
        public Options()
        {
            InitializeComponent();
            TextBoxDataDirectory.Text = Prefs.DataDirectory;
            TextBoxWindowSkin.Text = Prefs.WindowSkin;
            CheckBoxTileWindowSkin.IsChecked = Prefs.TileWindowSkin;
            CheckBoxLightChat.IsChecked = Prefs.UseLightChat;
            CheckBoxUseHardwareRendering.IsChecked = Prefs.UseHardwareRendering;
            CheckBoxUseWindowTransparency.IsChecked = Prefs.UseWindowTransparency;
            foreach (ComboBoxItem item in TextBoxWindowBorderDecorator.Items) {
                if (string.Equals(Prefs.WindowBorderDecorator, (string)item.Tag, StringComparison.Ordinal))
                {
                    item.IsSelected = true;
                }
            }
            CheckBoxIgnoreSSLCertificates.IsChecked = Prefs.IgnoreSSLCertificates;
            CheckBoxEnableChatImages.IsChecked = Prefs.EnableChatImages;
            //CheckBoxEnableChatGifs.IsChecked = Prefs.EnableChatGifs;
            CheckBoxEnableWhisperSound.IsChecked = Prefs.EnableWhisperSound;
            CheckBoxEnableNameSound.IsChecked = Prefs.EnableNameSound;
            CheckBoxUseWindowsForChat.IsChecked = Prefs.UseWindowsForChat;
            MaxChatHistory.Value = Prefs.MaxChatHistory;
            ChatFontSize.Value = Prefs.ChatFontSize;
            CheckBoxUseInstantSearch.IsChecked = Prefs.InstantSearch;
            CheckBoxEnableGameSounds.IsChecked = Prefs.EnableGameSound;
            ComboBoxZoomOptions.SelectedIndex = (int)Prefs.ZoomOption;
            ComboBoxJoinSound.SelectedIndex = (int)Prefs.SoundOption;
            CheckBoxEnableGameFonts.IsChecked = Prefs.UseGameFonts;
            CheckBoxEnableAdvancedOptions.IsChecked = Prefs.EnableAdvancedOptions;
            ComboBoxCardMoveNotification.SelectedIndex = (int)Prefs.CardMoveNotification;
            CheckBoxUseTestReleases.IsChecked = File.Exists(Path.Combine(Config.Instance.Paths.ConfigDirectory, "TEST"));
            HandDensitySlider.Value = Prefs.HandDensity;

            this.MinMaxButtonVisibility = Visibility.Collapsed;
            this.MinimizeButtonVisibility = Visibility.Collapsed;

            this.CanResize = false;
            this.ResizeMode = ResizeMode.CanMinimize;

        }

        void SetError(string error = "")
        {
            Dispatcher.Invoke(new Action(() =>
                {
                    if (string.IsNullOrWhiteSpace(error))
                    {
                        LabelError.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        LabelError.Visibility = Visibility.Visible;
                        LabelError.Text = error;
                    }
                }));
        }

        void ValidateFields(ref string dataDirectory,
            bool useLightChat, bool useHardwareRendering,
            bool useTransparentWindows, bool ignoreSSLCertificates, int maxChatHistory,
            bool enableChatImages, bool enableWhisperSound,
            bool enableNameSound, string windowSkin, 
            bool tileWindowSkin, bool useWindowsForChat, int chatFontSize, bool useInstantSearch, bool enableGameSounds, bool enableAdvancedOptions,
            bool useGameFonts, double handDensity, bool useTestReleases)
        {
            try
            {
                var dir = new DirectoryInfo(dataDirectory);
                if(!dir.Exists)Directory.CreateDirectory(dataDirectory);
                dataDirectory = dir.FullName;
            }
            catch (Exception)
            {
                throw new UserMessageException("The data directory value is invalid");
            }
            if (maxChatHistory < 50) throw new UserMessageException("Max chat history can't be less than 50");
            try
            {
                if (!String.IsNullOrWhiteSpace(windowSkin))
                {
                    if(!File.Exists(windowSkin))throw new UserMessageException("Window skin file doesn't exist");
                }
            }
            catch (Exception)
            {
                
                throw;
            }

        }

        void SaveSettings()
        {
            SetError();
            if (MaxChatHistory.Value == null) MaxChatHistory.Value = 100;
            var dataDirectory = TextBoxDataDirectory.Text;
            var windowSkin = TextBoxWindowSkin.Text;
            var tileWindowSkin = CheckBoxTileWindowSkin.IsChecked ?? false;
            var useLightChat = CheckBoxLightChat.IsChecked ?? false;
            var useHardwareRendering = CheckBoxUseHardwareRendering.IsChecked ?? false;
            var useTransparentWindows = CheckBoxUseWindowTransparency.IsChecked ?? false;
            var windowBorderDecorator = (string)(TextBoxWindowBorderDecorator.SelectedItem as ComboBoxItem)?.Tag;
            var ignoreSSLCertificates = CheckBoxIgnoreSSLCertificates.IsChecked ?? false;
            var maxChatHistory = MaxChatHistory.Value ?? 100;
            var enableChatImages = CheckBoxEnableChatImages.IsChecked ?? false;
            //var enableChatGifs = CheckBoxEnableChatGifs.IsChecked ?? false;
            var enableWhisperSound = CheckBoxEnableWhisperSound.IsChecked ?? false;
            var enableNameSound = CheckBoxEnableNameSound.IsChecked ?? false;
            var useWindowsForChat = CheckBoxUseWindowsForChat.IsChecked ?? false;
            var chatFontSize = ChatFontSize.Value ?? 12;
            var useInstantSearch = CheckBoxUseInstantSearch.IsChecked ?? false;
            var enableGameSounds = CheckBoxEnableGameSounds.IsChecked ?? false;
            var enableAdvancedOptions = CheckBoxEnableAdvancedOptions.IsChecked ?? false;
            var useGameFonts = CheckBoxEnableGameFonts.IsChecked ?? false;
            var handDensity = HandDensitySlider.Value;
            var useTestReleases = CheckBoxUseTestReleases.IsChecked ?? false;
            Prefs.ZoomType zoomOption = (Prefs.ZoomType)ComboBoxZoomOptions.SelectedIndex;
            Prefs.SoundType soundOption = (Prefs.SoundType)ComboBoxJoinSound.SelectedIndex;
            Prefs.CardAnimType animOption = (Prefs.CardAnimType)ComboBoxCardMoveNotification.SelectedIndex;
            var task = new Task(
                () => 
                    this.SaveSettingsTask(
                    ref dataDirectory, 
                    useLightChat, 
                    useHardwareRendering, 
                    useTransparentWindows,
                    ignoreSSLCertificates,
                    maxChatHistory,
                    enableChatImages,
                    enableWhisperSound,
                    enableNameSound,
                    windowSkin,
                    windowBorderDecorator,
                    tileWindowSkin,
                    useWindowsForChat,
                    chatFontSize,
                    useInstantSearch,
                    enableGameSounds,
                    zoomOption,
                    soundOption,
                    animOption,
                    enableAdvancedOptions,
                    useGameFonts,
                    handDensity,
                    useTestReleases)
                    );
            task.ContinueWith((t) =>
                                  {
                                      Dispatcher
                                          .Invoke(new Action(
                                              () => this.SaveSettingsComplete(t)));
                                  });
            task.Start();
        }

        void SaveSettingsTask(
            ref string dataDirectory, 
            bool useLightChat,
            bool useHardwareRendering, 
            bool useTransparentWindows,
            bool ignoreSSLCertificates,
            int maxChatHistory,
            bool enableChatImages,
            bool enableWhisperSound,
            bool enableNameSound,
            string windowSkin,
            string windowBorderDecorator,
            bool tileWindowSkin,
            bool useWindowsForChat,
            int chatFontSize,
            bool useInstantSearch,
            bool enableGameSounds,
            Prefs.ZoomType zoomOption,
            Prefs.SoundType soundOption,
            Prefs.CardAnimType animOption,
            bool enableAdvancedOptions,
            bool useGameFonts,
            double handDensity,
            bool useTestReleases)
        {
            this.ValidateFields(
                ref dataDirectory, 
                useLightChat, 
                useHardwareRendering, 
                useTransparentWindows,
                ignoreSSLCertificates,
                maxChatHistory,
                enableChatImages,
                enableWhisperSound,
                enableNameSound,
                windowSkin,
                tileWindowSkin,
                useWindowsForChat,
                chatFontSize,
                useInstantSearch,
                enableGameSounds,
				enableAdvancedOptions,
                useGameFonts,
                handDensity,
                useTestReleases
                );

            Prefs.DataDirectory = dataDirectory;
            Prefs.UseLightChat = useLightChat;
            Prefs.UseHardwareRendering = useHardwareRendering;
            Prefs.UseWindowTransparency = useTransparentWindows;
            Prefs.IgnoreSSLCertificates = ignoreSSLCertificates;
            Prefs.MaxChatHistory = maxChatHistory;
            Prefs.EnableChatImages = enableChatImages;
            Prefs.EnableWhisperSound = enableWhisperSound;
            Prefs.EnableNameSound = enableNameSound;
            Prefs.WindowSkin = windowSkin;
            Prefs.WindowBorderDecorator = windowBorderDecorator;
            Prefs.TileWindowSkin = tileWindowSkin;
            Prefs.UseWindowsForChat = useWindowsForChat;
            Prefs.ChatFontSize = chatFontSize;
            Prefs.InstantSearch = useInstantSearch;
            Prefs.EnableGameSound = enableGameSounds;
            Prefs.SoundOption = soundOption;
            Prefs.ZoomOption = zoomOption;
            Prefs.CardMoveNotification = animOption;
            Prefs.EnableAdvancedOptions = enableAdvancedOptions;
            Prefs.UseGameFonts = useGameFonts;
            Prefs.HandDensity = handDensity;
            if (useTestReleases && !File.Exists(Path.Combine(Config.Instance.Paths.ConfigDirectory, "TEST")))
                File.Create(Path.Combine(Config.Instance.Paths.ConfigDirectory, "TEST"));
            else if (!useTestReleases && File.Exists(Path.Combine(Config.Instance.Paths.ConfigDirectory, "TEST")))
                File.Delete(Path.Combine(Config.Instance.Paths.ConfigDirectory, "TEST"));
            //Prefs.EnableChatGifs = enableChatGifs;
        }

        void SaveSettingsComplete(Task task)
        {
            if (task.IsFaulted)
            {
                if (task.Exception != null)
                {
                    var ex = task.Exception.InnerExceptions.OfType<UserMessageException>().FirstOrDefault();
                    if (ex != null)
                    {
                        this.SetError(ex.Message);
                        return;
                    }
                }
                this.SetError("There was an error. Please exit OCTGN and try again.");
                return;
            }
            Program.FireOptionsChanged();
            this.Close();
        }

        private void ButtonCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSaveClick(object sender, RoutedEventArgs e)
        {
            this.SaveSettings();
        }

        private void ButtonPickDataDirectoryClick(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Config.Instance.Paths.DataDirectory;
            var result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK) return;
            TextBoxDataDirectory.Text = dialog.SelectedPath;
        }

        private void ButtonPickWindowSkinClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter =
                "All Images|*.BMP;*.JPG;*.JPEG;*.PNG|BMP Files: (*.BMP)|*.BMP|JPEG Files: (*.JPG;*.JPEG)|*.JPG;*.JPEG|PNG Files: (*.PNG)|*.PNG";
            dialog.CheckFileExists = true;
            var res = dialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxWindowSkin.Text = dialog.FileName;
            }
        }
    }
}
