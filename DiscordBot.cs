
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace AmongUsBot
{
	class DiscordBot
	{
		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private DiscordSocketClient m_client;
		private SocketVoiceChannel m_listenTo = null;
		private ISocketMessageChannel m_textChannel = null;

		public async void Start(string _token)
		{
			m_client = new DiscordSocketClient();

			m_client.Log += Log;

			//  You can assign your bot token to a string, and pass that in to connect.
			//  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
			var token = _token;

			// Some alternative options would be to keep your token in an Environment Variable or a standalone file.
			// var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
			// var token = File.ReadAllText("token.txt");
			// var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

			await m_client.LoginAsync(TokenType.Bot, token);
			await m_client.StartAsync();

			m_client.MessageReceived += ClientOnMessageReceived;

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		public void Mute()
		{
			if (m_listenTo == null)
			{
				return;
			}

			foreach (var user in m_listenTo.Users)
			{
				user.ModifyAsync((a) => { a.Mute = true; });
			}

			m_textChannel.SendMessageAsync("Muting");
		}

		public void UnMute()
		{
			if (m_listenTo == null)
			{
				return;
			}

			foreach (var user in m_listenTo.Users)
			{
				user.ModifyAsync((a) => { a.Mute = false; });
			}

			m_textChannel.SendMessageAsync("UnMuting");
		}

		private Task ClientOnMessageReceived(SocketMessage arg)
		{
			if (arg.Content.StartsWith("!StartGame"))
			{
				foreach (SocketGuild guild in m_client.Guilds)
				{

					foreach (var voice in guild.VoiceChannels)
					{
						if (voice.Users.FirstOrDefault((x) => { return string.Compare(x.AvatarId, arg.Author.AvatarId, StringComparison.InvariantCultureIgnoreCase) == 0; }) != null)
						{
							m_listenTo = voice;
							arg.Channel.SendMessageAsync("Muting everyone in " + m_listenTo.Name);
							m_textChannel = arg.Channel;
							Mute();
							break;
						}
					}
				}
			}

			if (arg.Content.StartsWith("!EndGame"))
			{
				arg.Channel.SendMessageAsync("Unmuting everyone in " + m_listenTo.Name);
				UnMute();
				m_listenTo = null;
				m_textChannel = null;

			}
			return Task.CompletedTask;
		}
	}
}
