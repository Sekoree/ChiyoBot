using ChiyoBot.Commands;
using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.ApplicationCommands.EventArgs;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Lavalink;
using DisCatSharp.Lavalink.EventArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiyoBot
{
    public class Bot
    {
        //Only serving 1 guild so eh...
        public static SongEntry CurrentSong { get; set; } = default;
        public static DiscordMessage InitMessage { get; set; } = default;
        public static List<SongEntry> SongQueue { get; set; } = new();
        public static PlaybackOptions PlaybackOpts { get; set; } = PlaybackOptions.None;
        public static bool Paused { get; set; } = false;
        public static bool Skip { get; set; } = false;
        public static int Volume { get; set; } = 100;
        
        static Random _rng = new Random();

        private DiscordClient _client { get;set; }
        private CommandsNextExtension _commandsNext { get; set; }
        private InteractivityExtension _interactivity { get; set; }
        private LavalinkExtension _lavalink {  get; set; }  
        private ApplicationCommandsExtension _applicationCommands { get; set; }

        public Bot()
        {
            _client = new DiscordClient(new DiscordConfiguration
            {
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug,
                Token = "",
                TokenType = TokenType.Bot
            });

            //Kinda useless cause only using slashcommands now
            _commandsNext = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDefaultHelp = false,
                StringPrefixes =  new[] { "!" }
            });

            _interactivity = _client.UseInteractivity();

            _lavalink = _client.UseLavalink();
            
            _applicationCommands = _client.UseApplicationCommands(new());

            //_applicationCommands.RegisterCommands<_Clear>(882677524990140427);
            //_applicationCommands.RegisterCommands<DebugCommands>(676502231377510420);
            //_applicationCommands.RegisterCommands<_Clear>(676502231377510420);
            _applicationCommands.RegisterCommands<MusicCommands>(676502231377510420);

            _applicationCommands.SlashCommandErrored += Errored;
            _client.ComponentInteractionCreated += ComponentInteraction;

            _client.GuildDownloadCompleted += Ready;
        }

        private async Task ComponentInteraction(DiscordClient sender, ComponentInteractionCreateEventArgs e)
        {
            if (e.Message.Id != InitMessage.Id)
                return;
            var con = this._lavalink.GetGuildConnection(e.Guild);
            if (con == default)
                return;
            switch (e.Id)
            {
                case "playpause":
                    _ = Task.Run(async () => await PlayPauseAsync(con));
                    await e.Interaction.CreateResponseAsync( InteractionResponseType.ChannelMessageWithSource);
                    break;
                case "skip":
                    _ = Task.Run(async () => await SkipAsync(con));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource);
                    break;
                case "volume":
                    await InitMessage.ModifyAsync(Constants.GetVolumeMessage()
                        .AddComponents(Constants.GetVolNavButtons()));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource);
                    break;
                case "more":
                    await InitMessage.ModifyAsync(Constants.GetModesMessage()
                        .AddComponents(Constants.GetMoreNavButtons()));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource);
                    break;
                case "back":
                    await InitMessage.ModifyAsync(Constants.GetMainMessage()
                        .AddComponents(Constants.GetMainNavButtons()));
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource);
                    break;
                default:
                    break;
            }
        }

        public async Task PlayPauseAsync(LavalinkGuildConnection con)
        {
            if (Paused)
                await con.ResumeAsync();
            else
                await con.PauseAsync();
            //TODO: Message
            Paused = !Paused;
        }

        public async Task SkipAsync(LavalinkGuildConnection con)
        {
            Skip = true;
            //TODO: Message
            await con.StopAsync();
        }

        private async Task Ready(DiscordClient sender, GuildDownloadCompletedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                var node = await _lavalink.ConnectAsync(new LavalinkConfiguration());
                node.PlaybackFinished += LL_PlaybackFinished;
            });
            await Task.Delay(0);
        }

        private async Task LL_PlaybackFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            var next = SongQueue.FirstOrDefault();
            if (e.Reason == TrackEndReason.Stopped && CurrentSong != default && !Skip)
                return;

            if (next == default && (PlaybackOpts != PlaybackOptions.RepeatAll || PlaybackOpts != PlaybackOptions.RepeatOne) && CurrentSong != default)
            {
                CurrentSong = default;
                return;
            }
            switch (PlaybackOpts)
            {
                case PlaybackOptions.None:
                    CurrentSong = next;
                    SongQueue.Remove(next);
                    await e.Player.PlayAsync(CurrentSong.Track);
                    break;
                case PlaybackOptions.RepeatOne:
                    await e.Player.PlayAsync(CurrentSong.Track);
                    break;
                case PlaybackOptions.RepeatAll:
                    SongQueue.Add(CurrentSong);
                    CurrentSong = next;
                    SongQueue.RemoveAt(0);
                    await e.Player.PlayAsync(CurrentSong.Track);
                    break;
                case (PlaybackOptions)6:
                    next = SongQueue[_rng.Next(0, SongQueue.Count)];
                    CurrentSong = next;
                    await e.Player.PlayAsync(CurrentSong.Track);
                    break;
                case PlaybackOptions.Shuffle:
                    next = SongQueue[_rng.Next(0, SongQueue.Count)];
                    CurrentSong = next;
                    SongQueue.Remove(next);
                    await e.Player.PlayAsync(CurrentSong.Track);
                    break;
                default:
                    CurrentSong = default;
                    break;
            }
            Skip = false;
            //TODO: Update Message
        }

        private async Task Errored(ApplicationCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);
            await e.Context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder().WithTitle("Error").WithDescription(e.Exception.ToString())));
            //return Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            await _client.ConnectAsync();
        }

        public async Task StopAsync()
        {
            await _client.DisconnectAsync();
        }

        [Flags]
        public enum PlaybackOptions
        {
            None = 0,
            RepeatOne = 1,
            RepeatAll = 2,
            Shuffle = 4
        }

        /*
         * Stuff:
         * Wenn es das war, GG try da müsste er sein
         * rip, gehts? Nope
         * irgendnen fehler?
         * wait ja ich könnt ja mal die events hooken hahahahahaha, gib mir mal bitte die bot id, ich überprüf mal mit postman ID:882482120549892097
         * was nu XD hab ne vermutung
         * eben das hab ich vermuted. ich lösch den mal manuell
         * Um also
         * Es geht aber der 48H command ist dann auch endlich angekommen, und den hatte ich ausgewählt, da isses natürlich dran gescheitert
         * lass ma laufen. geht oder. Client neustarten ;) der is gelöscht. aber ja, das braucht auch
         * danke, ich mach mir aber auch mal n hahahahahae clear klasse einfach für den fall
         * Der global ist noch da, denke das braucht einfach wieder etwas
         * welp danke nachmal :3 immer gern :3 jetzt brauch ich erstma energy, bin mal einkaufen, aber mobil erreichbar
         * okidoki aber ich glaub soweit komm ich jetzt auch allein weiter. guti
         */
    }
}
