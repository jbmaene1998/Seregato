using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace Seregato_Moderation
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration config)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
        }
        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            //client event
            _client.MessageReceived += OnMessageReceived;
            _client.JoinedGuild += OnJoinedGuild;
            _client.ChannelCreated += OnChannelCreated;
            _client.ChannelDestroyed += OnChannelDestroyed;
            _client.ReactionAdded += VerifiedOnReaction;
            _client.UserJoined += OnUserJoined;
            //service events
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        private async Task OnUserJoined(SocketGuildUser arg)
        {
            var notVerifiedRole = arg.Guild.Roles.FirstOrDefault(x => x.Id == 819928043232821281);
            await arg.AddRoleAsync(notVerifiedRole);
        }
        private async Task VerifiedOnReaction(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var verificationChannel = _client.GetChannel(819927666395840522) as ITextChannel;
            var verificationMessages = await verificationChannel.GetMessagesAsync(10).FlattenAsync();
            ulong verificationMessageId = 0;
            foreach (var verificationMessage in verificationMessages)
            {
                verificationMessageId = verificationMessage.Id;
            }
            if (arg3.MessageId != verificationMessageId) return;
            if (arg3.Emote.Name != "✅") return;
            var verifiedRole = (arg2 as SocketGuildChannel).Guild.Roles.FirstOrDefault(x => x.Name.ToString() == "Verified");
            var notVerifiedRole = (arg2 as SocketGuildChannel).Guild.Roles.FirstOrDefault(x => x.Name.ToString() == "Not-verified");
            //in bot instellingen moet presence intent aanstaan
            await (arg3.User.Value as SocketGuildUser).AddRoleAsync(verifiedRole);
            await (arg3.User.Value as SocketGuildUser).RemoveRoleAsync(notVerifiedRole);
        }
        private async Task OnJoinedGuild(SocketGuild arg)
        {
            var builder = new EmbedBuilder()
                .WithTitle($"Message from bot.")
                .WithColor(new Color(255, 184, 231))
                .WithDescription($"Hi, thank you so much for adding me to your server!!")
                .WithFooter("Bot created by: @jbmaene#4088")
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await arg.DefaultChannel.SendMessageAsync(null, false, embed);
        }

        private async Task OnChannelDestroyed(SocketChannel arg)
        {
            ulong id = 819688514865856522;
            var channel = _client.GetChannel(id) as ITextChannel;

            if (arg is SocketGuildChannel guildChannel)
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Channel deleted")
                    .WithColor(new Color(255, 184, 231))
                    .WithDescription($"Channel < {guildChannel.Name} > got deleted.")
                    .WithFooter("Bot created by: @jbmaene#4088")
                    .WithCurrentTimestamp();
                var embed = builder.Build();
                await channel.SendMessageAsync(null, false, embed);
            }
        }

        private async Task OnChannelCreated(SocketChannel arg)
        {
            if ((arg as IGuildChannel) == null) return;
            ulong id = 819688514865856522;
            var channel = _client.GetChannel(id) as ITextChannel;
            var newChannel = _client.GetChannel(arg.Id) as ITextChannel;
            var builder = new EmbedBuilder()
                .WithTitle("Channel created")
                .WithColor(new Color(255, 184, 231))
                .WithDescription($"Channel < {newChannel.Name} > got added.")
                .WithFooter("Bot created by: @jbmaene#4088")
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await channel.SendMessageAsync(null, false, embed);
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess)
            {
                var builder = new EmbedBuilder()
                    .WithTitle("Error")
                    .WithColor(new Color(255, 4, 0))
                    .WithDescription($"The command is invalid. Please try again. \n Use !Help to get a list of commands.")
                    .WithFooter("Bot created by: @jbmaene#4088")
                    .WithCurrentTimestamp();
                var embed = builder.Build();
                await context.Channel.SendMessageAsync(null, false, embed);
            }
            
        }
    }
}