
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

		public virtual async void Start(string _token)
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

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		public void MuteInChannel(SocketVoiceChannel _channel)
		{
			if (_channel == null)
			{
				return;
			}

			foreach (var user in _channel.Users)
			{
				Mute(user);
			}
		}

		public void UnmuteInChannel(SocketVoiceChannel _channel)
		{
			if (_channel == null)
			{
				return;
			}

			foreach (var user in _channel.Users)
			{
				Unmute(user);
			}
		}

		public void Mute(SocketGuildUser _user)
		{
			_user.ModifyAsync((a) => { a.Mute = true; });
		}

		public void Unmute(SocketGuildUser _user)
		{
			_user.ModifyAsync((a) => { a.Mute = false; });
		}

		protected DiscordSocketClient m_client;
	}
}
