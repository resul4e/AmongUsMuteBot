using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace AmongUsBot
{
	class AmongUsDiscordBot : DiscordBot
	{
		public AmongUsDiscordBot(AmongUsScraper _scraper) : base()
		{
			m_scraper = _scraper;
		}

		public override void Start(string _token)
		{
			base.Start(_token);
			m_client.MessageReceived += ClientOnMessageReceived;

			MuteInChannel(m_listenTo);
		}

		public void MuteAll()
		{
			MuteInChannel(m_listenTo);
		}

		public void UnmuteAll()
		{
			UnmuteInChannel(m_listenTo);
		}

		private Task ClientOnMessageReceived(SocketMessage arg)
		{
			if (arg.Content.StartsWith("!StartGame"))
			{
				foreach (SocketGuild guild in m_client.Guilds)
				{

					foreach (var voice in guild.VoiceChannels)
					{
						var authorVoiceChannel = voice.Users.FirstOrDefault((x) =>
							string.Compare(x.AvatarId, arg.Author.AvatarId,
								StringComparison.InvariantCultureIgnoreCase) == 0);
						if (authorVoiceChannel != null)
						{
							m_listenTo = voice;
							arg.Channel.SendMessageAsync("Muting everyone in " + m_listenTo.Name);
							m_textChannel = arg.Channel;
							MuteInChannel(m_listenTo);
							break;
						}
					}
				}
			}
			else if (arg.Content.StartsWith("!EndGame"))
			{
				arg.Channel.SendMessageAsync("Unmuting everyone in " + m_listenTo.Name);
				UnmuteInChannel(m_listenTo);
				m_listenTo = null;
				m_textChannel = null;

			}
			else if (arg.Content.StartsWith("!Talk"))
			{
				var guildUser = m_listenTo.Users.FirstOrDefault(x => x.Id == arg.Author.Id);
				Unmute(guildUser);
			}
			else if (arg.Content.StartsWith("!UnmuteAll"))
			{
				foreach (var guild in m_client.Guilds)
				{
					var author = guild.Users.FirstOrDefault(x => x.Id == arg.Author.Id);
					SocketRole roleWithMuteCapability = author?.Roles.FirstOrDefault(x => x.Guild == guild && x.Permissions.MuteMembers);

					if (author == null || roleWithMuteCapability == null)
					{
						continue;
					}

					foreach (var user in guild.Users)
					{
						Unmute(user);
					}
				}
			}
			else if(arg.Content.StartsWith("!"))
			{
				arg.Channel.SendMessageAsync("Unknown command, please try again");
			}
			return Task.CompletedTask;
		}

		private AmongUsScraper m_scraper;

		private SocketVoiceChannel m_listenTo = null;
		private ISocketMessageChannel m_textChannel = null;
	}

}
