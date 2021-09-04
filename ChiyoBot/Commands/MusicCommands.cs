using DisCatSharp;
using DisCatSharp.ApplicationCommands;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;
using DisCatSharp.Enums;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiyoBot.Commands
{
    public class MusicCommands : ApplicationCommandsModule
    {
        [SlashCommand("join", "Join the VC you are in")]
        public async Task JoinAsync(InteractionContext ctx)
        {
            Bot.CurrentSong = default;
            Bot.PlaybackOpts = Bot.PlaybackOptions.None;
            Bot.SongQueue.Clear();
            Bot.Skip = false; 
            Bot.Volume = 100;
            if (Bot.InitMessage != default)
            {
                try
                {
                    await Bot.InitMessage.DeleteAsync();
                }
                catch { }
            }
            Bot.InitMessage = default;
            var ll = ctx.Client.GetLavalink();
            var node = ll.GetIdealNodeConnection();
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var embedbld = new DiscordEmbedBuilder();
            if (ctx.Member.VoiceState?.Channel == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.WithDescription("You're not in a VC").Build()));
                return;
            }
            await node.ConnectAsync(ctx.Member.VoiceState?.Channel);
            embedbld.WithTitle("Connected");
            var msg = await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.Build()).AddComponents(Constants.GetMainNavButtons()));
            Bot.InitMessage = msg;
        }

        [SlashCommand("leave", "Leave the VC")]
        public async Task LeaveAsync(InteractionContext ctx)
        {
            var ll = ctx.Client.GetLavalink();
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var guild = ll.GetGuildConnection(ctx.Guild);
            await guild.DisconnectAsync();
            var embedbld = new DiscordEmbedBuilder();
            embedbld.WithTitle("Bye!");
            await ctx.DeleteResponseAsync();
            await Bot.InitMessage.ModifyAsync(new DiscordMessageBuilder().AddEmbed(embedbld.Build()));
            Bot.CurrentSong = default;
            Bot.PlaybackOpts = Bot.PlaybackOptions.None;
            Bot.SongQueue.Clear();
            await Task.Delay(5000);
            await Bot.InitMessage.DeleteAsync();
            Bot.InitMessage = default;
        }

        [SlashCommand("play", "Play a song via URL")]
        public async Task LeaveAsync(InteractionContext ctx,
            [Option("URL", "Video URL")] string url)
        {
            var ll = ctx.Client.GetLavalink();
            var guild = ll.GetGuildConnection(ctx.Guild);
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var embedbld = new DiscordEmbedBuilder();
            bool rootMessage = false;
            if (guild == null && ctx.Member.VoiceState?.Channel != null)
            {
                Bot.CurrentSong = default;
                Bot.PlaybackOpts = Bot.PlaybackOptions.None;
                Bot.SongQueue.Clear();
                Bot.Skip = false;
                Bot.Volume = 100;
                if (Bot.InitMessage != default)
                {
                    try
                    {
                        await Bot.InitMessage.DeleteAsync();
                    }
                    catch {}
                }
                Bot.InitMessage = default;
                var node = ll.GetIdealNodeConnection();
                await node.ConnectAsync(ctx.Member.VoiceState?.Channel);
                embedbld.WithTitle("Connected");

                Bot.InitMessage = await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.Build()).AddComponents(Constants.GetMainNavButtons()));
                rootMessage = true;
            }
            else if (guild == null && ctx.Member.VoiceState?.Channel == null)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.WithDescription("You're not in a VC").Build()));
                return;
            }
            embedbld.WithTitle("Results!");
            guild = ll.GetGuildConnection(ctx.Guild);
            var s = await guild.Node.Rest.GetTracksAsync(new Uri(url));
            var fmMsg = (DiscordMessage)default;
            if (!s.Tracks.Any())
            {
                if (!rootMessage)
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.WithDescription("Nothing found :c" +
                        "\nReason:" +
                        $"\n{s.Exception}").Build()));
                else
                    fmMsg = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedbld.WithDescription("Nothing found :c" +
                        "\nReason:" +
                        $"\n{s.Exception}").Build()));
                await Task.Delay(2500);
                if (!rootMessage)
                    await ctx.DeleteResponseAsync();
                else
                    await ctx.DeleteFollowupAsync(fmMsg.Id);
                return;
            }
            var track = s.Tracks.First();
            if (Bot.CurrentSong == default)
            {
                await guild.PlayAsync(track);
                Bot.CurrentSong = new SongEntry(ctx.Member.Id, track.TrackString);
                if (!rootMessage)
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.WithDescription($"Playing:" +
                            $"\n{track.Title} [{track.Length.ToString(@"mm\:ss")}]" +
                            $"\n by {track.Author} [Link]({track.Uri})").Build()));
                else
                    fmMsg = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedbld.WithDescription($"Playing:" +
                            $"\n{track.Title} [{track.Length.ToString(@"mm\:ss")}]" +
                            $"\n by {track.Author} [Link]({track.Uri})").Build()));

            }
            else
            {
                Bot.SongQueue.Add(new SongEntry(ctx.Member.Id, track.TrackString));
                if (!rootMessage)
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.WithDescription($"Added:" +
                            $"\n{track.Title} [{track.Length.ToString(@"mm\:ss")}]" +
                            $"\n by {track.Author} [Link]({track.Uri})").Build()));
                else
                    fmMsg = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedbld.WithDescription($"Added:" +
                            $"\n{track.Title} [{track.Length.ToString(@"mm\:ss")}]" +
                            $"\n by {track.Author} [Link]({track.Uri})").Build()));
            }
            await Task.Delay(5000);
            if (!rootMessage)
                await ctx.DeleteResponseAsync();
            else
                await ctx.DeleteFollowupAsync(fmMsg.Id);
        }

        //[SlashCommand("pause", "Pause playback")]
        //public async Task PauseAsync(InteractionContext ctx)
        //{
        //    var ll = ctx.Client.GetLavalink();
        //    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        //    var guild = ll.GetGuildConnection(ctx.Guild);
        //    await guild.PauseAsync();
        //    var embedbld = new DiscordEmbedBuilder();
        //    embedbld.WithTitle("Paused!");
        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.Build()).AddComponents(Constants.GetMainNavButtons()));
        //}
        //
        //[SlashCommand("resume", "Resume playback")]
        //public async Task ResumeAsync(InteractionContext ctx)
        //{
        //    var ll = ctx.Client.GetLavalink();
        //    await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        //    var guild = ll.GetGuildConnection(ctx.Guild);
        //    await guild.ResumeAsync();
        //    var embedbld = new DiscordEmbedBuilder();
        //    embedbld.WithTitle("Resumed!");
        //    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embedbld.Build()).AddComponents(Constants.GetMainNavButtons()));
        //}
    }
}
