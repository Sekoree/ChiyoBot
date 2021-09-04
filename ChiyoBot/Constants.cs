using DisCatSharp.Entities;
using DisCatSharp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiyoBot
{
    public class Constants
    {
        public static DiscordMessageBuilder GetMainMessage()
        {
            var dmb = new DiscordMessageBuilder();
            var embld = new DiscordEmbedBuilder();
            embld.WithTitle("Now Playing");
            embld.AddField($"{Bot.CurrentSong.Track.Title}",
                $"{Bot.CurrentSong.Track.Author} [{Bot.CurrentSong.Track.Length.ToString(@"mm\:ss")}]");
            embld.AddField("Playback", $"Volume: {Bot.Volume}%\n" +
                $"Paused? {Bot.Paused}\n" +
                $"Modes enabled: {Bot.PlaybackOpts}");
            dmb.AddEmbed(embld.Build());
            return dmb;
        }

        public static DiscordMessageBuilder GetVolumeMessage()
        {
            var dmb = new DiscordMessageBuilder();
            var embld = new DiscordEmbedBuilder();
            embld.WithTitle("Volume");
            embld.WithDescription($"Current volume: {Bot.Volume}");
            dmb.AddEmbed(embld.Build());
            return dmb;
        }

        public static DiscordMessageBuilder GetModesMessage()
        {
            var dmb = new DiscordMessageBuilder();
            var embld = new DiscordEmbedBuilder();
            embld.WithTitle("Modes");
            embld.WithDescription($"Currently enabled modes: {Bot.PlaybackOpts}");
            dmb.AddEmbed(embld.Build());
            return dmb;
        }

        public static DiscordComponent[] GetMainNavButtons()
        {
            var btn1 = new DiscordButtonComponent(ButtonStyle.Secondary, "playpause", "Play/Pause");
            var btn2 = new DiscordButtonComponent(ButtonStyle.Secondary, "skip", "Skip");
            var btn3 = new DiscordButtonComponent(ButtonStyle.Secondary, "volume", "Volume");
            var btn4 = new DiscordButtonComponent(ButtonStyle.Secondary, "queue", "Queue");
            var btn5 = new DiscordButtonComponent(ButtonStyle.Secondary, "more", "More");
            return new[] { btn1, btn2, btn3, btn4, btn5 };
        }

        public static DiscordComponent[] GetVolNavButtons()
        {
            var btn1 = new DiscordButtonComponent(ButtonStyle.Secondary, "volMenu_VolDown5", "-5");
            var btn2 = new DiscordButtonComponent(ButtonStyle.Secondary, "volMenu_VolDown1", "-1");
            var btn3 = new DiscordButtonComponent(ButtonStyle.Secondary, "volMenu_VolUp1", "+1");
            var btn4 = new DiscordButtonComponent(ButtonStyle.Secondary, "volMenu_VolUp5", "+5");
            var btn5 = new DiscordButtonComponent(ButtonStyle.Secondary, "back", "Back");
            return new[] { btn1, btn2, btn3, btn4, btn5 };
        }

        public static DiscordComponent[] GetMoreNavButtons()
        {
            var btn1 = new DiscordButtonComponent(ButtonStyle.Secondary, "moreMenu_repeat", "Repeat");
            var btn2 = new DiscordButtonComponent(ButtonStyle.Secondary, "moremenu_shuffle", "Shuffle");
            var btn3 = new DiscordButtonComponent(ButtonStyle.Secondary, "back", "Back");
            return new[] { btn1, btn2, btn3 };
        }

        public static DiscordComponent[] GetQueueNavButtons()
        {
            var btn1 = new DiscordButtonComponent(ButtonStyle.Secondary, "queueMenu_First", "<<");
            var btn2 = new DiscordButtonComponent(ButtonStyle.Secondary, "queueMenu_Previous", "<");
            var btn3 = new DiscordButtonComponent(ButtonStyle.Secondary, "queueMenu_Next", ">");
            var btn4 = new DiscordButtonComponent(ButtonStyle.Secondary, "queueMenu_Last", ">>");
            var btn5 = new DiscordButtonComponent(ButtonStyle.Secondary, "back", "Back");
            return new[] { btn1, btn2, btn3 };
        }
    }
}
