using System;
using System.Windows;
using Memory;

namespace AmongUsBot
{
	class AmongUsScraper
	{
		public AmongUsScraper(string _pid)
		{
			m_mem = new Mem();
			bool opened = m_mem.OpenProcess(int.Parse(_pid));

			if (!opened)
			{
				throw new Exception("We should have opened an Among Us Process");
			}
		}

		public bool GetIsInMeeting()
		{
			return m_mem.ReadByte("UnityPlayer.dll+012A7A14,0x64,0x34,0x8,0xC,0x3C,0x18") == 1;
		}

		public Vector GetPlayer1Position()
		{
			var posX = m_mem.ReadFloat("UnityPlayer.dll+01277F00,0x20,0x2C,0x58,0x0,0x4,0x5C,0x2C");
			var posY = m_mem.ReadFloat("UnityPlayer.dll+01277F00,0x20,0x2C,0x58,0x0,0x4,0x5C,0x30");
			return new Vector(posX, posY);
		}

		private readonly Mem m_mem;
	}
}
