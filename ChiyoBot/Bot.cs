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
        }

        private async Task Ready(DiscordClient sender, GuildDownloadCompletedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                var node = await _lavalink.ConnectAsync(new LavalinkConfiguration());
            });
            await Task.Delay(0);
        }

        public async Task RunAsync()
        {
            await _client.ConnectAsync();
        }

        public async Task StopAsync()
        {
            await _client.DisconnectAsync();
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
