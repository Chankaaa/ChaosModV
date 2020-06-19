﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Shared; 

using static ConfigApp.Effects;

namespace ConfigApp
{
    public partial class MainWindow : Window
    {
        private OptionsFile m_configFile = new OptionsFile("config.ini");
        private OptionsFile m_twitchFile = new OptionsFile("twitch.ini");
        private OptionsFile m_effectsFile = new OptionsFile("effects.ini");

        private Dictionary<EffectType, TreeMenuItem> m_treeMenuItemsMap;
        private Dictionary<EffectType, EffectData> m_effectDataMap;

        public MainWindow()
        {
            Init();
        }

        private void Init()
        {
            InitializeComponent();

            ParseConfigFile();
            ParseTwitchFile();

            InitEffectsTreeView();

            ParseEffectsFile();

            InitTwitchTab();
        }

        private EffectData GetEffectData(EffectType effectType)
        {
            // Create EffectData in case effect wasn't saved yet
            if (!m_effectDataMap.TryGetValue(effectType, out EffectData effectData))
            {
                effectData = new EffectData(EffectsMap[effectType].IsShort ? EffectTimedType.TIMED_SHORT : EffectTimedType.TIMED_NORMAL, -1, 5, false, false, null);
            }

            return effectData;
        }

        private void ParseConfigFile()
        {
            m_configFile.ReadFile();

            misc_user_effects_spawn_dur.Text = m_configFile.ReadValue("NewEffectSpawnTime", "30");
            misc_user_effects_timed_dur.Text = m_configFile.ReadValue("EffectTimedDur", "90");
            misc_user_effects_random_seed.Text = m_configFile.ReadValue("Seed");
            misc_user_effects_timed_short_dur.Text = m_configFile.ReadValue("EffectTimedShortDur", "30");
            misc_user_effects_clear_enable.IsChecked = m_configFile.ReadValueBool("EnableClearEffectsShortcut", true);
            misc_user_effects_twice_disable.IsChecked = m_configFile.ReadValueBool("DisableEffectTwiceInRow", false);
            misc_user_effects_drawtimer_disable.IsChecked = m_configFile.ReadValueBool("DisableTimerBarDraw", false);
            misc_user_effects_drawtext_disable.IsChecked = m_configFile.ReadValueBool("DisableEffectTextDraw", false);
            misc_user_toggle_mod_shortcut.IsChecked = m_configFile.ReadValueBool("EnableToggleModShortcut", true);
            if (m_configFile.HasKey("EffectTimerColor"))
            {
                misc_user_effects_timer_color.SelectedColor = (Color)ColorConverter.ConvertFromString(m_configFile.ReadValue("EffectTimerColor"));
            }
            if (m_configFile.HasKey("EffectTextColor"))
            {
                misc_user_effects_text_color.SelectedColor = (Color)ColorConverter.ConvertFromString(m_configFile.ReadValue("EffectTextColor"));
            }
            if (m_configFile.HasKey("EffectTimedTimerColor"))
            {
                misc_user_effects_effect_timer_color.SelectedColor = (Color)ColorConverter.ConvertFromString(m_configFile.ReadValue("EffectTimedTimerColor"));
            }
        }

        private void WriteConfigFile()
        {
            m_configFile.WriteValue("NewEffectSpawnTime", misc_user_effects_spawn_dur.Text);
            m_configFile.WriteValue("EffectTimedDur", misc_user_effects_timed_dur.Text);
            m_configFile.WriteValue("Seed", misc_user_effects_random_seed.Text);
            m_configFile.WriteValue("EffectTimedShortDur", misc_user_effects_timed_short_dur.Text);
            m_configFile.WriteValue("EnableClearEffectsShortcut", misc_user_effects_clear_enable.IsChecked.Value);
            m_configFile.WriteValue("DisableEffectTwiceInRow", misc_user_effects_twice_disable.IsChecked.Value);
            m_configFile.WriteValue("DisableTimerBarDraw", misc_user_effects_drawtimer_disable.IsChecked.Value);
            m_configFile.WriteValue("DisableEffectTextDraw", misc_user_effects_drawtext_disable.IsChecked.Value);
            m_configFile.WriteValue("EnableToggleModShortcut", misc_user_toggle_mod_shortcut.IsChecked.Value);
            m_configFile.WriteValue("EffectTimerColor", misc_user_effects_timer_color.SelectedColor.ToString());
            m_configFile.WriteValue("EffectTextColor", misc_user_effects_text_color.SelectedColor.ToString());
            m_configFile.WriteValue("EffectTimedTimerColor", misc_user_effects_effect_timer_color.SelectedColor.ToString());

            m_configFile.WriteFile();
        }

        private void ParseTwitchFile()
        {
            m_twitchFile.ReadFile();

            twitch_user_agreed.IsChecked = m_twitchFile.ReadValueBool("EnableTwitchVoting", false);
            twitch_user_channel_name.Text = m_twitchFile.ReadValue("TwitchChannelName");
            twitch_user_user_name.Text = m_twitchFile.ReadValue("TwitchUserName");
            twitch_user_channel_oauth.Password = m_twitchFile.ReadValue("TwitchChannelOAuth");
            twitch_user_poll_passphrase.Text = m_twitchFile.ReadValue("TwitchVotingPollPass");
            twitch_user_effects_chance_no_voting_round.Text = m_twitchFile.ReadValue("TwitchVotingNoVoteChance", "50");
            twitch_user_effects_secs_before_chat_voting.Text = m_twitchFile.ReadValue("TwitchVotingSecsBeforeVoting", "0");
            twitch_user_voter_indicator_enabled.IsChecked = m_twitchFile.ReadValueBool("TwitchVotingVoterIndicator", false);
            twitch_user_chat_no_vote_msg_disable.IsChecked = m_twitchFile.ReadValueBool("TwitchVotingDisableNoVoteRoundMsg", false);
            twitch_user_show_voteables_onscreen_enable.IsChecked = m_twitchFile.ReadValueBool("TwitchVotingShowVoteablesOnscreen", false);
        }

        private void WriteTwitchFile()
        {
            m_twitchFile.WriteValue("EnableTwitchVoting", twitch_user_agreed.IsChecked.Value);
            m_twitchFile.WriteValue("TwitchChannelName", twitch_user_channel_name.Text);
            m_twitchFile.WriteValue("TwitchUserName", twitch_user_user_name.Text);
            m_twitchFile.WriteValue("TwitchChannelOAuth", twitch_user_channel_oauth.Password);
            m_twitchFile.WriteValue("TwitchVotingPollPass", twitch_user_poll_passphrase.Text);
            m_twitchFile.WriteValue("TwitchVotingNoVoteChance", twitch_user_effects_chance_no_voting_round.Text);
            m_twitchFile.WriteValue("TwitchVotingSecsBeforeVoting", twitch_user_effects_secs_before_chat_voting.Text);
            m_twitchFile.WriteValue("TwitchVotingVoterIndicator", twitch_user_voter_indicator_enabled.IsChecked.Value);
            m_twitchFile.WriteValue("TwitchVotingDisableNoVoteRoundMsg", twitch_user_chat_no_vote_msg_disable.IsChecked.Value);
            m_twitchFile.WriteValue("TwitchVotingShowVoteablesOnscreen", twitch_user_show_voteables_onscreen_enable.IsChecked.Value);

            m_twitchFile.WriteFile();
        }

        private void ParseEffectsFile()
        {
            m_effectsFile.ReadFile();

            foreach (string key in m_effectsFile.GetKeys())
            {
                string value = m_effectsFile.ReadValue(key);

                string[] values = value.Split(',');

                // Find EffectType from ID
                EffectType effectType = EffectType._EFFECT_ENUM_MAX;
                foreach (KeyValuePair<EffectType, EffectInfo> pair in EffectsMap)
                {
                    if (pair.Value.Id == key)
                    {
                        effectType = pair.Key;
                        break;
                    }
                }

                if (!EffectsMap.TryGetValue(effectType, out EffectInfo effectInfo))
                {
                    continue;
                }

                EffectTimedType effectTimedType = effectInfo.IsShort ? EffectTimedType.TIMED_SHORT : EffectTimedType.TIMED_NORMAL;
                int effectTimedTime = -1;
                int effectWeight = 5;
                bool effectPermanent = false;
                bool effectExcludedFromVoting = false;
                string effectCustomName = null;

                // Compatibility checks, previous versions had less options
                if (values.Length >= 4)
                {
                    Enum.TryParse(values[1], out effectTimedType);
                    int.TryParse(values[2], out effectTimedTime);
                    int.TryParse(values[3], out effectWeight);

                    if (values.Length >= 5)
                    {
                        int tmp;

                        int.TryParse(values[4], out tmp);
                        effectPermanent = tmp != 0;

                        if (values.Length >= 6)
                        {
                            int.TryParse(values[5], out tmp);
                            effectExcludedFromVoting = tmp != 0;

                            if (values.Length >= 7)
                            {
                                effectCustomName = values[6] == "0" ? null : values[6];
                            }
                        }
                    }
                }

                if (effectType != EffectType._EFFECT_ENUM_MAX)
                {
                    int.TryParse(values[0], out int enabled);
                    m_treeMenuItemsMap[effectType].IsChecked = enabled == 0 ? false : true;

                    m_effectDataMap.Add(effectType, new EffectData(effectTimedType, effectTimedTime, effectWeight, effectPermanent, effectExcludedFromVoting, effectCustomName));
                }
            }
        }

        private void WriteEffectsFile()
        {
            for (EffectType effectType = 0; effectType < EffectType._EFFECT_ENUM_MAX; effectType++)
            {
                EffectData effectData = GetEffectData(effectType);

                m_effectsFile.WriteValue(EffectsMap[effectType].Id, $"{(m_treeMenuItemsMap[effectType].IsChecked ? 1 : 0)},{(effectData.EffectTimedType == EffectTimedType.TIMED_NORMAL ? 0 : 1)}"
                    + $",{effectData.EffectCustomTime},{effectData.EffectWeight},{(effectData.EffectPermanent ? 1 : 0)},{(effectData.EffectExcludedFromVoting ? 1 : 0)}"
                    + $",{(string.IsNullOrEmpty(effectData.EffectCustomName) ? "0" : effectData.EffectCustomName)}");
            }

            m_effectsFile.WriteFile();
        }

        private void InitEffectsTreeView()
        {
            m_treeMenuItemsMap = new Dictionary<EffectType, TreeMenuItem>();
            m_effectDataMap = new Dictionary<EffectType, EffectData>();

            TreeMenuItem playerParentItem = new TreeMenuItem("Player");
            TreeMenuItem vehicleParentItem = new TreeMenuItem("Vehicle");
            TreeMenuItem pedsParentItem = new TreeMenuItem("Peds");
            TreeMenuItem timeParentItem = new TreeMenuItem("Time");
            TreeMenuItem weatherParentItem = new TreeMenuItem("Weather");
            TreeMenuItem miscParentItem = new TreeMenuItem("Misc");

            for (EffectType effectType = 0; effectType < EffectType._EFFECT_ENUM_MAX; effectType++)
            {
                EffectInfo effectInfo = EffectsMap[effectType];
                string effectName = effectInfo.Name;
                TreeMenuItem menuItem = new TreeMenuItem(effectName);
                m_treeMenuItemsMap.Add(effectType, menuItem);

                switch (effectInfo.EffectCategory)
                {
                    case EffectCategory.PLAYER:
                        playerParentItem.AddChild(menuItem);
                        break;
                    case EffectCategory.VEHICLE:
                        vehicleParentItem.AddChild(menuItem);
                        break;
                    case EffectCategory.PEDS:
                        pedsParentItem.AddChild(menuItem);
                        break;
                    case EffectCategory.TIME:
                        timeParentItem.AddChild(menuItem);
                        break;
                    case EffectCategory.WEATHER:
                        weatherParentItem.AddChild(menuItem);
                        break;
                    case EffectCategory.MISC:
                        miscParentItem.AddChild(menuItem);
                        break;
                }
            }

            effects_user_effects_tree_view.Items.Clear();
            effects_user_effects_tree_view.Items.Add(playerParentItem);
            effects_user_effects_tree_view.Items.Add(vehicleParentItem);
            effects_user_effects_tree_view.Items.Add(pedsParentItem);
            effects_user_effects_tree_view.Items.Add(timeParentItem);
            effects_user_effects_tree_view.Items.Add(weatherParentItem);
            effects_user_effects_tree_view.Items.Add(miscParentItem);
        }

        void InitTwitchTab()
        {
            if (File.Exists(".twitchpoll"))
            {
                twitch_user_poll_passphrase_label.Visibility = Visibility.Visible;
                twitch_user_poll_passphrase.Visibility = Visibility.Visible;

                twitch_user_channel_name_label.Visibility = Visibility.Hidden;
                twitch_user_channel_name.Visibility = Visibility.Hidden;
                twitch_user_user_name_label.Visibility = Visibility.Hidden;
                twitch_user_user_name.Visibility = Visibility.Hidden;
                twitch_user_channel_oauth_label.Visibility = Visibility.Hidden;
                twitch_user_channel_oauth.Visibility = Visibility.Hidden;
                twitch_user_effects_chance_no_voting_round_label.Visibility = Visibility.Hidden;
                twitch_user_effects_chance_no_voting_round.Visibility = Visibility.Hidden;
                twitch_user_chat_no_vote_msg_disable_label.Visibility = Visibility.Hidden;
                twitch_user_chat_no_vote_msg_disable.Visibility = Visibility.Hidden;
                twitch_user_show_voteables_onscreen_enable_label.Visibility = Visibility.Hidden;
                twitch_user_show_voteables_onscreen_enable.Visibility = Visibility.Hidden;
            }

            TwitchTabHandleAgreed();
        }

        void TwitchTabHandleAgreed()
        {
            bool agreed = twitch_user_agreed.IsChecked.GetValueOrDefault();

            twitch_user_channel_name.IsEnabled = agreed;
            twitch_user_channel_oauth.IsEnabled = agreed;
            twitch_user_user_name.IsEnabled = agreed;
            twitch_user_poll_passphrase.IsEnabled = agreed;
            twitch_user_effects_chance_no_voting_round.IsEnabled = agreed;
            twitch_user_effects_secs_before_chat_voting.IsEnabled = agreed;
            twitch_user_voter_indicator_enabled.IsEnabled = agreed;
            twitch_user_chat_no_vote_msg_disable.IsEnabled = agreed;
            twitch_user_show_voteables_onscreen_enable.IsEnabled = agreed;
        }

        private void OnlyNumbersPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Utils.HandleOnlyNumbersPreviewTextInput(e);
        }

        private void NoSpacePreviewKeyDown(object sender, KeyEventArgs e)
        {
            Utils.HandleNoSpacePreviewKeyDown(e);
        }

        private void NoCopyPastePreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Utils.HandleNoCopyPastePreviewExecuted(e);
        }

        private void user_save_Click(object sender, RoutedEventArgs e)
        {
            WriteConfigFile();
            WriteTwitchFile();
            WriteEffectsFile();

            MessageBox.Show("Saved Config!", "ChaosModV", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void user_reset_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to reset your Config?", "ChaosModV",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                m_configFile.ResetFile();

                m_effectsFile.ResetFile();

                result = MessageBox.Show("Do you want to reset your Twitch settings too?", "ChaosModV",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    m_twitchFile.ResetFile();
                    ParseTwitchFile();

                    // Ensure all options are disabled in twitch tab again
                    TwitchTabHandleAgreed();
                }

                Init();

                MessageBox.Show("Config has been set back to default!", "ChaosModV", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void twitch_user_agreed_Clicked(object sender, RoutedEventArgs e)
        {
            TwitchTabHandleAgreed();
        }

        private void effect_user_config_Click(object sender, RoutedEventArgs e)
        {
            TreeMenuItem curTreeMenuItem = (TreeMenuItem)((TreeViewItem)((Grid)((Border)((ContentPresenter)((StackPanel)((Button)sender).Parent).TemplatedParent).Parent).Parent).TemplatedParent).DataContext;

            EffectType effectType = EffectType._EFFECT_ENUM_MAX;
            foreach (var pair in m_treeMenuItemsMap)
            {
                if (pair.Value == curTreeMenuItem)
                {
                    effectType = pair.Key;

                    break;
                }
            }

            if (effectType != EffectType._EFFECT_ENUM_MAX)
            {
                EffectInfo effectInfo = EffectsMap[effectType];
                EffectData effectData = GetEffectData(effectType);

                EffectConfig effectConfig = new EffectConfig(effectData, effectInfo);
                effectConfig.ShowDialog();

                if (effectConfig.IsSaved)
                {
                    effectData.EffectTimedType = effectConfig.effectconf_timer_type_enable.IsChecked.Value ? (EffectTimedType)effectConfig.effectconf_timer_type.SelectedIndex
                        : effectInfo.IsShort ? EffectTimedType.TIMED_SHORT : EffectTimedType.TIMED_NORMAL;
                    effectData.EffectCustomTime = effectConfig.effectconf_timer_time_enable.IsChecked.Value
                        ? effectConfig.effectconf_timer_time.Text.Length > 0 ? int.Parse(effectConfig.effectconf_timer_time.Text) : -1 : -1;
                    effectData.EffectPermanent = effectConfig.effectconf_timer_permanent_enable.IsChecked.Value;
                    effectData.EffectWeight = effectConfig.effectconf_effect_weight.SelectedIndex + 1;
                    effectData.EffectExcludedFromVoting = effectConfig.effectconf_exclude_voting_enable.IsChecked.Value;
                    effectData.EffectCustomName = effectConfig.effectconf_effect_custom_name.Text.Trim();
                }
            }
        }

        private void contribute_github_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/gta-chaos-mod/ChaosModV");
        }

        private void contribute_donate_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://paypal.me/EmrCue");
        }
    }
}