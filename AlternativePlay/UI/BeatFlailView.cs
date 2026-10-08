using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatFlailView : BaseModeSelectView
    {
        protected override void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.LeftFlailMode));
            this.NotifyPropertyChanged(nameof(this.RightFlailMode));
            this.NotifyPropertyChanged(nameof(this.LeftFlailLength));
            this.NotifyPropertyChanged(nameof(this.RightFlailLength));
            this.NotifyPropertyChanged(nameof(this.Gravity));
            this.NotifyPropertyChanged(nameof(this.MoveNotesBack));
        }

        [UIValue(nameof(FlailIcon))]
        public string FlailIcon => IconNames.BeatFlail;

        [UIValue(nameof(LeftFlailModeIcon))]
        public string LeftFlailModeIcon 
        {
            get
            {
                switch (this.settings.LeftFlailMode)
                {
                    default:
                    case BeatFlailMode.Flail:
                        return IconNames.LeftFlail;
                    case BeatFlailMode.Sword:
                        return IconNames.LeftSaber;
                    case BeatFlailMode.None:
                        return IconNames.Empty;
                }
            }
        }

        [UIValue(nameof(LeftFlailMode))]
        private string LeftFlailMode
        {
            get => this.settings.LeftFlailMode.ToString();
            set
            {
                this.settings.LeftFlailMode = (BeatFlailMode)Enum.Parse(typeof(BeatFlailMode), value);
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.LeftFlailModeIcon));
            }
        }

        [UIValue(nameof(LeftFlailModeList))]
        private List<object> LeftFlailModeList = new List<object> { BeatFlailMode.Flail.ToString(), BeatFlailMode.Sword.ToString(), BeatFlailMode.None.ToString() };

        [UIValue(nameof(RightFlailModeIcon))]
        public string RightFlailModeIcon
        {
            get
            {
                switch (this.settings.RightFlailMode)
                {
                    default:
                    case BeatFlailMode.Flail:
                        return IconNames.RightFlail;
                    case BeatFlailMode.Sword:
                        return IconNames.RightSaber;
                    case BeatFlailMode.None:
                        return IconNames.Empty;
                }
            }
        }

        [UIValue(nameof(RightFlailMode))]
        private string RightFlailMode
        {
            get => this.settings.RightFlailMode.ToString();
            set
            {
                this.settings.RightFlailMode = (BeatFlailMode)Enum.Parse(typeof(BeatFlailMode), value);
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.RightFlailModeIcon));
            }
        }

        [UIValue(nameof(RightFlailModeList))]
        private List<object> RightFlailModeList = new List<object> { BeatFlailMode.Flail.ToString(), BeatFlailMode.Sword.ToString(), BeatFlailMode.None.ToString() };

        [UIValue(nameof(LeftFlailLength))]
        private int LeftFlailLength
        {
            get => this.settings.LeftFlailLength;
            set
            {
                this.settings.LeftFlailLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(RightFlailLength))]
        private int RightFlailLength
        {
            get => this.settings.RightFlailLength;
            set
            {
                this.settings.RightFlailLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(LeftHandleLength))]
        private int LeftHandleLength
        {
            get => this.settings.LeftHandleLength;
            set
            {
                this.settings.LeftHandleLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(RightHandleLength))]
        private int RightHandleLength
        {
            get => this.settings.RightHandleLength;
            set
            {
                this.settings.RightHandleLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(Gravity))]
        private float Gravity
        {
            get => this.settings.Gravity;
            set
            {
                this.settings.Gravity = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(MoveNotesBack))]
        private int MoveNotesBack
        {
            get => this.settings.MoveNotesBack;
            set
            {
                this.settings.MoveNotesBack = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIAction(nameof(OnResetGravity))]
        private void OnResetGravity()
        {
            this.settings.Gravity = 3.5f;
            this.configuration.SaveConfiguration();
            this.NotifyPropertyChanged(nameof(this.Gravity));
        }

        [UIAction(nameof(LengthFormatter))]
        private string LengthFormatter(int value)
        {
            return $"{value} cm";
        }
    }
}